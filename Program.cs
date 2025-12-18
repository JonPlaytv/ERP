using System;
using System.IO;
using System.Linq;
using System.Text;
using Terminal.Gui;

namespace ERP_Fix
{
    class Code
    {
        public static bool HideCredits = false;
        private static bool _terminalGuiInitialized = false;

        static void Main(string[] args)
        {
            // Catch any unhandled exception so the console doesn’t close instantly when launched by double-click.
            AppDomain.CurrentDomain.UnhandledException += (sender, evt) =>
            {
                try
                {
                    var ex = evt.ExceptionObject as Exception;
                    ShowFatalError("An unexpected error occurred.", ex);
                }
                catch { /* best effort */ }
            };

            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                if (args.Contains("--hide-credits"))
                {
                    HideCredits = true;
                }

                if (args.Contains("--show-completed-orders"))
                {
                    TUI.ShowCompletedOrders = true;
                }

                if (args.Contains("--show-cancelled-orders"))
                {
                    TUI.ShowCancelledOrders = true;
                }

                // start actual program
                if (args.Contains("--shell"))
                {
                    Shell shell = new Shell();
                    shell.Start();
                }
                else if (args.Contains("--newshell"))
                {
                    NewShell newShell = new NewShell();
                    newShell.Start();
                }
                else
                {
                    TUI tui = new TUI();
                    _terminalGuiInitialized = true;
                    tui.Start();
                }
            }
            catch (Exception ex)
            {
                ShowFatalError("A fatal error prevented the application from starting.", ex);
            }
            finally
            {
                // Attempt a graceful shutdown of Terminal.Gui if it was initialized
                try
                {
                    if (_terminalGuiInitialized)
                    {
                        Terminal.Gui.Application.Shutdown();
                    }
                }
                catch { /* ignore */ }
            }
        }

        private static void ShowFatalError(string header, Exception? ex)
        {
            try
            {
                var baseDir = AppContext.BaseDirectory;
                var logFile = Path.Combine(baseDir, "crash.log");
                var sb = new StringBuilder();
                sb.AppendLine($"==== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ====");
                sb.AppendLine(header);
                if (ex != null)
                {
                    sb.AppendLine(ex.ToString());
                }
                File.AppendAllText(logFile, sb.ToString(), Encoding.UTF8);

                // Best-effort console output for users launching via double-click
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(header);
                Console.ResetColor();
                if (ex != null)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine($"A log was written to: {logFile}");
                Console.WriteLine("Press Enter to exit...");
                try { Console.ReadLine(); } catch { /* ignore */ }
            }
            catch { /* ignore all */ }
        }
    }
}
