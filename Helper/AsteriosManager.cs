using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class AsteriosManager
    {

        //Console.WriteLine("pids {0}  try {1} ", pid, i);

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

        private static int opendelay = 1000;
        private static int maxdaley = 3000;

        static public bool OpenWindow(int pid)
        {
            try
            {
                Process proc = Process.GetProcessById(pid);
                for (int i = 0; i < maxdaley/opendelay; i++)
                {
                    if (IsIconic(proc.MainWindowHandle))
                    {
                        ShowWindow(proc.MainWindowHandle, 9);
                    }
                    else
                    {
                        SetForegroundWindow(proc.MainWindowHandle);
                    }
                    Thread.Sleep(opendelay);

                    uint processID = 0;
                    IntPtr hWnd = GetForegroundWindow();
                    uint threadID = GetWindowThreadProcessId(hWnd, out processID);
                    Process fgProc = Process.GetProcessById(Convert.ToInt32(processID));

                    if (fgProc.Id == pid)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { }
            return false;
        }
    }
}
