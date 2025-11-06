namespace AotConsoleApp
{
    class Program
    {
        private static bool _keepRunning = true;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from NativeAOT!");
            Console.WriteLine($"Current time: {DateTime.Now}");
            Console.WriteLine($"Platform: {Environment.OSVersion}");
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            
            if (args.Length > 0)
            {
                Console.WriteLine($"Arguments: {string.Join(", ", args)}");
            }
            else
            {
                Console.WriteLine("No arguments provided.");
            }
            
            // 注册 Ctrl+C 处理
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // 阻止进程立即终止
                _keepRunning = false;
                Console.WriteLine("\n\nReceived shutdown signal. Exiting gracefully...");
            };
            
            Console.WriteLine("\nApplication is running. Press Ctrl+C to exit.");
            Console.WriteLine("-------------------------------------------");
            
            int counter = 0;
            while (_keepRunning)
            {
                counter++;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Heartbeat #{counter}");
                Thread.Sleep(5000); // 每5秒输出一次
            }
            
            Console.WriteLine("\nApplication completed successfully!");
        }
    }
}
