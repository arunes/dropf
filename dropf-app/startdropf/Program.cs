using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace startdropf
{

    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr
        FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            Console.Title = "startdropf";

            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 0); // 0 = SW_HIDE
                //ShowWindow(hWnd, 1); //1 = SW_SHOWNORMA
            }

            var haveArg = args.Length > 0;

            if (haveArg)
            {
                switch (args[0])
                {
                    case "wait":
                        System.Threading.Thread.Sleep(10000);
                        string progFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        ProcessStartInfo dropfProcess = new ProcessStartInfo(Path.Combine(progFolder, "dropf.exe"));
                        dropfProcess.WorkingDirectory = progFolder;
                        Process.Start(dropfProcess);
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    Console.WriteLine("try-" + i);
                    if (System.Diagnostics.Process.GetProcessesByName("dropf").Length == 0)
                    {
                        string progFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        ProcessStartInfo dropfProcess = new ProcessStartInfo(Path.Combine(progFolder, "dropf.exe"));
                        dropfProcess.WorkingDirectory = progFolder;
                        Process.Start(dropfProcess);
                        Environment.Exit(0);
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }

            Environment.Exit(0);
        }
    }
}
