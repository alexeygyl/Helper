using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class BotManager
    {
        private Thread botThrd = null;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int keys);

        private ServerWindow serverWindow = null;
        private long allBuffTime = 0;
        private long povBuffTime = 0;
        private long dcTime = 0;
        Mutex mutex = new Mutex();

        public  BotManager(ServerWindow serverWindow)
        {
            this.serverWindow = serverWindow;
            Thread serverThrd = new Thread(StartStopThread);
            serverThrd.Start();

            Thread buffThrd = new Thread(BuffThread);
            buffThrd.Start();
        }

        private void StartStopThread()
        {
            Types.Action start = Config.GetAction("start");
            Types.Action stop = Config.GetAction("stop");
            Thread.Sleep(1000);
            while (true)
            {
                if (GetAsyncKeyState(Keyboard.GetKeyCodeByString(stop.key)) != 0)
                {
                    if (botThrd != null && botThrd.IsAlive == true)
                    {
                        Console.WriteLine("Stop");
                        botThrd.Abort();
                    }
                   
                    serverWindow.UpdateStatus(false);
                }
                else if (GetAsyncKeyState(Keyboard.GetKeyCodeByString(start.key)) != 0)
                {
                    if (botThrd == null)
                    {
                        Console.WriteLine("Start");
                        botThrd = new Thread(BotThread);
                        botThrd.Start();
                    }
                    serverWindow.UpdateStatus(true);
                }

                Thread.Sleep(300);
            }
        }


        private void BuffThread()
        {
            Thread.Sleep(200000);
            try
            {
                List<string> list = new List<string>();
                while (true)
                {
                    
                    Thread.Sleep(500);
                    if (botThrd == null)
                    {
                        continue;
                    }

                    DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;

                    if (now.ToUnixTimeSeconds() - allBuffTime > 1140)
                    {
                        allBuffTime = now.ToUnixTimeSeconds();
                        list.Add("all");
                    }

                    if (now.ToUnixTimeSeconds() - povBuffTime > 230)
                    {
                        povBuffTime = now.ToUnixTimeSeconds();
                        list.Add("pov");
                    }

                    if (list.Count > 0)
                    {
                        Console.WriteLine("BuffThread Wait");
                        mutex.WaitOne();
                        MemberManager.Buff(list);
                        list.Clear();
                        mutex.ReleaseMutex();
                        Console.WriteLine("BuffThread Release");
                    }

                    if (now.ToUnixTimeSeconds() - dcTime > 110)
                    {
                        Console.WriteLine("DCThread Wait");
                        mutex.WaitOne();
                        dcTime = now.ToUnixTimeSeconds();
                        MemberManager.DC();
                        mutex.ReleaseMutex();
                        Console.WriteLine("DCThread Release");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BotThread()
        {
            try
            {
                List<string> list = new List<string>();
                list.Add("all");
                while (true)
                {
                    Thread.Sleep(300);
                    Console.WriteLine("BotThread Wait");
                    mutex.WaitOne();
                    AsteriosManager.OpenWindow();
                    MemberManager.Buff(list);
                    mutex.ReleaseMutex();
                    Console.WriteLine("BotThread release");
                }   
            }
            catch (Exception ex)
            {
                mutex.ReleaseMutex();
                Console.WriteLine(ex.Message);
                botThrd = null;
            }  
        }
    }
}
