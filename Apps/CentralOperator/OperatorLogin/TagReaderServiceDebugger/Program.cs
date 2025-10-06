using System;
using System.Configuration;
using System.Threading;
using TPL.TagReaders;

namespace TagReaderServiceDebugger
{
    class Program
    {
        private static TagReaderServiceFramework serviceFramework;
        private static bool running = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Tag Reader Service Debugger");
            Console.WriteLine("===========================");
            Console.WriteLine();

            // You can also read from config if you prefer
            string connectionString = "Data Source=SWC20231112;Initial Catalog=JEGR_DB;Persist Security Info=True;User ID=sa;Password=Jen&excellent";

            try
            {
                serviceFramework = new TagReaderServiceFramework(connectionString, ConsoleFeedback);
                serviceFramework.DebugMode = true;  // Enable debug mode
                serviceFramework.DBDebugMode = false; // Disable DB debug to reduce noise

                // Configure optional features
                serviceFramework.UseOperatorPermissions = false;
                serviceFramework.UseShiftManagement = true;
                serviceFramework.AutoLogout = false;

                Console.WriteLine("Starting service...");
                Console.WriteLine();

                var state = serviceFramework.Start();

                if (state == ServiceState.Running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Service is RUNNING");
                    Console.WriteLine();
                    Console.WriteLine("Commands:");
                    Console.WriteLine("  'p' or 'ping'  - Ping all readers");
                    Console.WriteLine("  's' or 'stop'  - Stop service");
                    Console.WriteLine("  'r' or 'restart' - Restart service");
                    Console.WriteLine("  'q' or 'quit'  - Exit application");
                    Console.WriteLine();

                    // Command loop
                    while (running)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true);
                            ProcessCommand(key.KeyChar.ToString().ToLower());
                        }
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to start service");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (serviceFramework != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Stopping service...");
                    serviceFramework.Stop();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ProcessCommand(string command)
        {
            switch (command)
            {
                case "p":
                case "ping":
                    Console.WriteLine();
                    Console.WriteLine(">>> Pinging readers...");
                    serviceFramework.Ping();
                    break;

                case "s":
                case "stop":
                    Console.WriteLine();
                    Console.WriteLine(">>> Stopping service...");
                    serviceFramework.Stop();
                    Console.WriteLine("Service stopped. Press 'r' to restart or 'q' to quit.");
                    break;

                case "r":
                case "restart":
                    Console.WriteLine();
                    Console.WriteLine(">>> Restarting service...");
                    serviceFramework.Stop();
                    Thread.Sleep(500);
                    serviceFramework.Start();
                    break;

                case "q":
                case "quit":
                    Console.WriteLine();
                    Console.WriteLine(">>> Quitting...");
                    running = false;
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private static void ConsoleFeedback(string message)
        {
            // Add timestamp to messages
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine($"[{timestamp}] {message}");
        }
    }
}
