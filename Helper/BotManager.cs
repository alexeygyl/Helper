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
            Types.Action heal = Config.GetAction("heal");
            Types.Action petheal = Config.GetAction("petheal");

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
                            Console.WriteLine("myStats: {0} {1}", myStats.hp.current, conditions.myhp);
                            if (AsteriosManager.IsOpened() == true)
                            {
                                Keyboard.PressKey(heal.key);
                            }
                            toHeal = true;    
                        }

                        if (conditions.pethp > 0 && myStats.pet > 0 && myStats.pet < conditions.pethp)
                        {
                            Console.WriteLine("Pet: {0} {1}", myStats.pet, conditions.pethp);
                            if (AsteriosManager.IsOpened() == true)
                            {
                                Keyboard.PressKey(petheal.key);
                            }
                            toHeal = true;
                            break;
                        }

                        if (toHeal == true)
                        {
                            break;
                        }

                        List<Types.Stats> party = AsteriosManager.GetPartyStats();
                        foreach (Types.Stats member in party)
                        {
                            if (conditions.partyhp > 0 && member.hp.current > 0 && member.hp.current < conditions.partyhp)
                            {
                                Console.WriteLine("Member: {0} {1}", member.hp.current, conditions.partyhp);
                                toHeal = true;
                                break;
                            }

                            if (conditions.pethp > 0 && member.pet > 0 && member.pet < conditions.pethp)
                            {
                                Console.WriteLine("Member Pet: {0} {1}", member.pet, conditions.pethp);
                                toHeal = true;
                                break;
                            }
                        }

                    }
                    while (false);

                    if (toHeal == true)
                    {
                        toHeal = false;
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
            Types.Config config = Config.GetConfig();
            //Thread.Sleep(2000);
            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                    if (botThrd == null)
                    {
                        continue;
                    }

                    mutex.WaitOne();
                    DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;

                    for (int i = 0; i < Config.buffs.Count; i++)
                    {
                        if (now.ToUnixTimeSeconds() - Config.timeouts[i] > Config.buffs.ElementAt(i).period)
                        {
                            Config.timeouts[i] = now.ToUnixTimeSeconds();
                            if (config.name == Config.buffs.ElementAt(i).name)
                            {
                                if (AsteriosManager.OpenWindow() == false)
                                {
                                    continue;
                                }

                                Keyboard.PressKey(Config.buffs.ElementAt(i).key);
                                Thread.Sleep(Config.buffs.ElementAt(i).delay);
                            }
                            else 
                            {
                                MemberManager.Buff(Config.buffs.ElementAt(i));
                            }
                        }
                    
                    }

                    mutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    mutex.ReleaseMutex();
                    Console.WriteLine(ex.Message);
                }
            }
          
        }

        private void BotThread()
        {
            //CreateGroup();
            Types.State state = Types.State.Pre;
            Types.Conditions conditions = Config.GetConditions();
            List<Types.Action> preattack = Config.GetPreActions();
            List<Types.Action> preattackfail = Config.GetPreFailActions();
            List<Types.Action> attack = Config.GetAttackActions();
            List<Types.Action> postattack = Config.GetPostActions();

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
                            {
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
                            }

                        case Types.State.PreFail:
                            {
                                Console.WriteLine("Types.State.PreFail");
                                foreach (Types.Action action in preattackfail)
                                {
                                    Keyboard.PressKey(action.key);
                                    Thread.Sleep(action.delay);
                                }
                                state = Types.State.Pre;
                                break;
                            }

                        case Types.State.Support:
                            {
                                Support();
                                state = Types.State.Attack;
                                break;
                            }
                         
                        case Types.State.Attack:
                            {
                                DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
                                //Console.WriteLine("Types.State.Attack {0} {1}", start, now.ToUnixTimeMilliseconds());

                                foreach (Types.Action action in attack)
                                {
                                    Keyboard.PressKey(action.key);
                                    Thread.Sleep(action.delay);
                                }

                                if (now.ToUnixTimeMilliseconds() - start > conditions.maxtime)
                                {
                                    Console.WriteLine("MAX time");
                                    state = Types.State.Pre;
                                    break;
                                }

                                if (AsteriosManager.GetTargetHp() <= 0)
                                {
                                    state = Types.State.Post;
                                }

                                break;
                            }
                          
                        case Types.State.Post:
                            {
                                Console.WriteLine("Types.State.Post");
                                foreach (Types.Action action in postattack)
                                {
                                    Keyboard.PressKey(action.key);
                                    Thread.Sleep(action.delay);
                                }
                                state = Types.State.Pre;
                                break;
                            }
                      
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
                return;
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


        private void Support()
        {
            try
            {
                Console.WriteLine("Types.State.Support");
                foreach (var support in Config.supports)
                {
                    if (support.Value == true)
                    {
                        Console.WriteLine("Support {0}", support.Key);
                        MemberManager.Support(support.Key);
                    }
                }
                Console.WriteLine("Types.State.Support dOne");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateGroup() 
        {
            try
            {
                mutex.WaitOne();
                foreach (Types.MemberInfo memberInfo in MemberManager.GetMembersInfo())
                {
                    if (memberInfo.party == true)
                    {
                        MemberManager.Invite(memberInfo);
                    }

                }
                mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public bool Running() 
        {
            if (botThrd == null)
            {
                return false;
            }

            return true;
        }
    }
}
