using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntiNim
{
    class Program
    { 
        //全部扑克牌
        static string[] pokers = new string[] { "111", "11111", "1111111" };
        //本轮玩家
        static Players currentPlayer;
        //上次拿牌的行数
        static int preRow = -1;

        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Wellcome();
            RandomPlayer();
            Play();
        }

        static void Wellcome()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("欢迎来到游戏！\r\n" +
                "游戏由Player1和Player2两位玩家轮流拿走任意行任意数量扑克牌，但不能跨行。\r\n" +
                "请在下面3行扑克牌中拿走扑克牌，0表示已经拿走，1表示没有拿走，拿最后一根牙签的人即为输家。\r\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 显示扑克牌状态
        /// </summary>
        static void PrintPokers()
        {
            pokers.ToList().ForEach((poker) =>
            {
                Console.WriteLine(poker + "\r\n");
            });
        }

        /// <summary>
        /// 随机选取先手玩家
        /// </summary>
        static void RandomPlayer()
        {
            Task.Delay(1000).Wait();
            Random rd = new Random();
            int result = rd.Next(1, 100) % 2;
            if (result == 0)
                currentPlayer = Players.Player1;
            else
                currentPlayer = Players.Player2;

            Console.WriteLine("请输入行号和拿牌数量，中间空格隔开，例如:1 3\r\n");
        }

        /// <summary>
        /// 更换玩家
        /// </summary>
        static void ChangePlayer()
        {
            if (currentPlayer == Players.Player1)
                currentPlayer = Players.Player2;
            else
                currentPlayer = Players.Player1;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        static void Play()
        {
            try
            {
                Task.Delay(1000).Wait();
                PrintPokers();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("请 " + currentPlayer + " 拿牌:\r\n");
                string input = Console.ReadLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                string[] result = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //输入数据格式正确
                if (result.Length == 2)
                {
                    int row = int.Parse(result[0]);
                    int amount = int.Parse(result[1]);
                    Play(row, amount);
                }
                else
                    //输入格式错误
                    CatchError(ErrorTypes.General);
            }
            catch(Exception ex)
            {
                //输入格式异常
                CatchError(ErrorTypes.General);
            }
            
            Play();
        }

        /// <summary>
        /// 游戏规则算法
        /// </summary>
        /// <param name="row">输入行数</param>
        /// <param name="amount">拿牌个数</param>
        static void Play(int nextRow, int amount)
        {
            //超出行数的选择范围
            if (nextRow <= 0 || nextRow > 3)
            {
                CatchError(ErrorTypes.Range);
                return;
            }

            //如果第一次拿牌
            if (preRow == -1)
                preRow = nextRow;
            
            //如果当前行数不是上次拿牌的行数,先拿完再拿其他行数的扑克
            if(nextRow != preRow)
            {
                int preRowCount = pokers[preRow - 1].Count(number => number == '1');
                if(preRowCount>=1)
                {
                    CatchError(ErrorTypes.Row);
                    return;
                }
            }

            //当行扑克牌剩余数量
            int pokerCount = pokers[nextRow - 1].Count(number => number == '1');
            if (pokerCount > 0 && pokerCount >= amount)
            {
                int index = pokers[nextRow - 1].IndexOf("1");
                char[] arr = pokers[nextRow - 1].ToArray();
                for (int i = index; i < index + amount; i++)
                {
                    arr.SetValue('0', i);
                }
                pokers[nextRow - 1] = string.Join("", arr);
                CheckWinner();
                ChangePlayer();
                preRow = nextRow;
            }
            else
            {
                CatchError(ErrorTypes.None);
                return;
            }
        }

        /// <summary>
        /// 检查胜负
        /// </summary>
        static void CheckWinner()
        {
            string allPoker = string.Join("", pokers);
            int winCount = allPoker.Count(number => number == '1');
            if(winCount == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write( currentPlayer + " 恭喜您！您赢了，按任意键退出游戏...");
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            int loseCount = allPoker.Count(number => number == '1');
            if(loseCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(currentPlayer + "很遗憾！您输了，按任意键退出...");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="type">错误类型</param>
        static void CatchError(ErrorTypes type)
        {
            string errorMsg = "错误：";
            switch (type)
            {
                case ErrorTypes.None:
                    errorMsg += "这行没有足够的扑克牌...\r\n";
                    break;
                case ErrorTypes.Row:
                    errorMsg += "第"+ preRow + "行还有扑克没有拿，请拿完这行后再拿其他行的扑克牌...\r\n";
                    break;
                case ErrorTypes.Range:
                    errorMsg += "只能选择1-3行，请重新输入行数...\r\n";
                    break;
                default:
                    errorMsg += "输入内容不正确，请重新输入...\r\n";
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMsg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    /// <summary>
    /// 玩家
    /// </summary>
    enum Players
    {
        Player1 = 1,
        Player2 = 2
    }

    /// <summary>
    /// 错误消息类型
    /// </summary>
    enum ErrorTypes
    {
        /// <summary>
        /// 通用错误
        /// </summary>
        General,
        /// <summary>
        /// 没有扑克牌拿了
        /// </summary>
        None,
        /// <summary>
        /// 选择行数错误
        /// </summary>
        Row,
        /// <summary>
        /// 选择范围错误
        /// </summary>
        Range
    }
}
