// See https://aka.ms/new-console-template for more information


public static class Program
{
    public static volatile int Interrupt = 0;

    public static volatile int AcceptExit = 0;

    public static int Main(string[] args)
    {
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss"));

        // 添加退出事件
        AppDomain appd = AppDomain.CurrentDomain;
        appd.ProcessExit += (s, e) =>
        {
            if (Interrupt == 0)
            {
                Interrupt = 1;
            }

            while (AcceptExit == 0)
            {
                // 等待主线程同意退出
            }

            Console.WriteLine("退出了");

            File.WriteAllText("1.txt", "aaa");
        };
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // true: 不导致退出；false: 会导致退出

            Interrupt = 1;

            Console.WriteLine("Ctrl+C 中断被触发");
        };


        while (true)
        {
            Console.WriteLine(DateTime.Now);
            Thread.Sleep(1000);

            if (Interrupt > 0)
            {
                break;
            }
        }

        Console.WriteLine("主线程退出");
        AcceptExit = 1;

        return 0;
    }
}