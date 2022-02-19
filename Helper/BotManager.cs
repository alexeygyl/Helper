using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            Thread healThrd = new Thread(HealThread);
            healThrd.Start();

            AsteriosManager.Start();
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

        private void HealThread()
        {
            Thread.Sleep(2000);

            bool toHeal = false;

            Types.Conditions conditions = Config.GetConditions();
            //Console.WriteLine(conditions.healwait);

            int healwait = 0;

            try
            {
                while (true)
                {

                    Thread.Sleep(400);

                    if (botThrd == null || healwait > 0)
                    {
                        healwait -= 400;
                        continue;
                    }

                    do
                    {
                        Types.Stats myStats = AsteriosManager.GetMyStats();
                        if (myStats.hp.total != 0 && (myStats.hp.current * 100 / myStats.hp.total) < conditions.myhp)
                        {
                            toHeal = true;
                            break;
                        }

                        if (conditions.pethp > 0 && myStats.pet > 0 && myStats.pet < conditions.pethp)
                        {
                            toHeal = true;
                            break;
                        }

                        List<Types.Stats> party = AsteriosManager.GetPartyStats();
                        foreach (Types.Stats member in party)
                        {
                            if (conditions.partyhp > 0 && member.hp.current > 0 && member.hp.current < conditions.partyhp)
                            {
                                toHeal = true;
                                break;
                            }

                            if (conditions.pethp > 0 && myStats.pet > 0 && member.pet < conditions.pethp)
                            {
                                toHeal = true;
                                break;
                            }
                        }

                    }
                    while (false);

                    if (toHeal == true)
                    {
                        mutex.WaitOne();
                        Console.WriteLine("Server GroupHeal");
                        MemberManager.GroupHeal();
                        healwait = conditions.healwait;
                        mutex.ReleaseMutex();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void BuffThread()
        {
            Thread.Sleep(2000);
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
            Types.State state = Types.State.Pre;
            Types.Conditions conditions = Config.GetConditions();

            List<Types.Action> preattack = Config.GetPreActions();
            List<Types.Action> preattackfail = Config.GetPreFailActions();
            List<Types.Action> attack = Config.GetAttackActions();
            List<Types.Action> postattack = Config.GetPostActions();
            List<Types.Support> supports = Config.GetSupports();

            long start = 0;

            try
            {
                while (true)
                {
                    Thread.Sleep(300);
                  
                    mutex.WaitOne();
                    if (AsteriosManager.OpenWindow() == false)
                    {
                        mutex.ReleaseMutex();
                        continue;
                    }

                    switch (state)
                    {
                        case Types.State.Pre:
                            Console.WriteLine("Types.State.Pre");

                            foreach (Types.Action action in preattack)
                            {
                                Keyboard.PressKey(action.key);
                                Thread.Sleep(action.delay);
                                //Console.WriteLine("{0} {1}", action.key, action.delay);
                            }

                            if (AsteriosManager.GetTargetHp() > 0)
                            {
                                start = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                                state = Types.State.Support;
                            }
                            else 
                            {
                                state = Types.State.PreFail;
                            }
                            break;

                        case Types.State.PreFail:
                            Console.WriteLine("Types.State.PreFail");
                            foreach (Types.Action action in preattackfail)
                            {
                                Keyboard.PressKey(action.key);
                                Thread.Sleep(action.delay);
                            }
                            state = Types.State.Pre;
                            break;

                        case Types.State.Support:
                            Console.WriteLine("Types.State.Support");
                            foreach (Types.Support support in supports)
                            {
                                MemberManager.Support(support.prof);
                            }
                            state = Types.State.Attack;
                            break;

                        case Types.State.Attack:
                           
                            foreach (Types.Action action in attack)
                            {
                                Keyboard.PressKey(action.key);
                                Thread.Sleep(action.delay);
                            }
                            
                            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
                            //Console.WriteLine("Types.State.Attack {0} {1}", start, now.ToUnixTimeMilliseconds());
                            if (now.ToUnixTimeMilliseconds() - start > conditions.maxtime)
                            {
                                //Console.WriteLine("MAX time");
                                state = Types.State.Pre;
                                break;
                            }

                            if (AsteriosManager.GetTargetHp() <= 0)
                            {
                                state = Types.State.Post;
                            }
  
                            break;
                        case Types.State.Post:
                            Console.WriteLine("Types.State.Post");
                            foreach (Types.Action action in postattack)
                            {
                                Keyboard.PressKey(action.key);
                                Thread.Sleep(action.delay);
                            }
                            state = Types.State.Pre;
                            break;
                        default:
                            state = Types.State.Pre;
                            break;

                    }
                    
                    mutex.ReleaseMutex();
       
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                botThrd = null;
            }

            try
            {
                mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
