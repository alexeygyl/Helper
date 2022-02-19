using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class AsteriosManager
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Types.Rect rectangle);
        

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            [In()] System.IntPtr hdc, int x, int y, int cx, int cy,
            [In()] System.IntPtr hdcSrc, int x1, int y1, uint rop);


        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        private static Thread mainThrd = null;
        private static int opendelay = 1000;
        private static int maxdaley = 3000;
        private static int pid = 0;
        private static IntPtr hWnd;

        private static Rectangle windowRect = new Rectangle();

        private static Bitmap topBitmap = null;
        private static Bitmap leftBitmap = null;
        private static List<Bitmap> buffs = new List<Bitmap>();
        private static List<Types.Stats> party = new List<Types.Stats>();

        private static Types.Stats myStats;
        private static int targetHP;



        public static void SetPid(int pid) 
        {
            AsteriosManager.pid = pid;
        }

        public static bool HasPid()
        {
            return pid > 0;
        }

        public static void Start()
        {
            AsteriosManager.mainThrd = new Thread(MainThrd);
            AsteriosManager.mainThrd.Start();
        }

        public static bool IsOpened()
        {
            uint processID = 0;
            hWnd = GetForegroundWindow();
            uint threadID = GetWindowThreadProcessId(hWnd, out processID);
            Process fgProc = Process.GetProcessById(Convert.ToInt32(processID));

            if (fgProc.Id == pid)
            {
                return true;
            }

            return false;
        }

        static public bool OpenWindow()
        {
            try
            {
                Process proc = Process.GetProcessById(pid);
                for (int i = 0; i < maxdaley/opendelay; i++)
                {
                    if (IsOpened() == true)
                    {
                        return true;
                    }

                    if (IsIconic(proc.MainWindowHandle))
                    {
                        ShowWindow(proc.MainWindowHandle, 9);
                    }
                    else
                    {
                        SetForegroundWindow(proc.MainWindowHandle);
                    }
                    Thread.Sleep(opendelay);
                }
                return false;
            }
            catch { }
            return false;
        }


        private static void MainThrd()
        {
            try
            {
                while (true)
                {
                    if (AsteriosManager.IsOpened() == false)
                    {
                        Thread.Sleep(300);
                        continue;
                    }

                    UpdateMainRect();
                    UpdateBitmaps();

                    myStats = Analyzer.UpdateMyStats(ref topBitmap);
                    myStats.pet = Analyzer.UpdatePetHp(ref leftBitmap);
                    targetHP = Analyzer.UpdateTargetHp(ref topBitmap);
                    buffs = Analyzer.UpdateBuffs(ref topBitmap);;
                    party = Analyzer.UpdatePartyInfo(ref leftBitmap);



                   //Console.WriteLine("myStats hp {0} pet {1}", myStats.hp.current, myStats.pet);
                   //Console.WriteLine(" targetHP {0}", targetHP);

                    //int pos = 1;
                    //foreach (Types.Stats member in party) 
                    //{
                    //    Console.WriteLine(" Member {0}: Hp {1} Pet {2}",pos++, member.hp.current, member.pet);
                    //}


                   // Thread.Sleep(1000);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void UpdateMainRect()
        {
            Types.Rect r = new Types.Rect();
            if (GetWindowRect(hWnd, ref r) == true)
            {
                windowRect.X = r.Left;
                windowRect.Y = r.Top;
                windowRect.Height = (r.Bottom - r.Top - 40);
                windowRect.Width = (r.Right - r.Left - 20);
            }
        }

        private static void UpdateBitmaps() 
        {
            UpdateTopBitmap();
            UpdateLeftBitmap(); 
        }

        private static void UpdateTopBitmap()
        {
            Bitmap bmp = new Bitmap(windowRect.Width, 100);
            Graphics g_dst = Graphics.FromImage(bmp);
            Graphics g_src = Graphics.FromHwnd(hWnd);
            IntPtr hsrcdc = g_src.GetHdc();
            IntPtr hdcCP = g_dst.GetHdc();
            BitBlt(hdcCP, 0, 0, windowRect.Width, 100, hsrcdc, 0, 0, (int)CopyPixelOperation.SourceCopy);
            g_dst.ReleaseHdc();
            g_src.ReleaseHdc();
            topBitmap = bmp;
        }

        private static void UpdateLeftBitmap()
        {
            Bitmap bmp = new Bitmap(400, windowRect.Height);
            Graphics g_dst = Graphics.FromImage(bmp);
            Graphics g_src = Graphics.FromHwnd(hWnd);
            IntPtr hsrcdc = g_src.GetHdc();
            IntPtr hdcCP = g_dst.GetHdc();
            BitBlt(hdcCP, 0, 0, 400, windowRect.Height, hsrcdc, 0, 0, (int)CopyPixelOperation.SourceCopy);
            g_dst.ReleaseHdc();
            g_src.ReleaseHdc();
            leftBitmap = bmp;

        }

        public static List<Types.Stats> GetPartyStats()
        {
            return party;
        }

        public static Types.Stats GetMyStats()
        {
            return myStats;
        }

        public static int GetTargetHp()
        {
            return targetHP;
        }

        public static Rectangle GetWindowRect()
        {
            return windowRect;
        }

        public static void SetLang(string lang)
        {
            string id = "00000409";
            if (lang.ToLower() == "ru")
            {
                id = "00000419";
            }

            PostMessage(hWnd, 0x0050, IntPtr.Zero, LoadKeyboardLayout(id, 1));
            Thread.Sleep(400);
        }
    }

}
