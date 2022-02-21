using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class TCPClient
    {
        private ClientWindow handler = null;
        private Thread mainThrd;
        private Thread infoThrd;
        private Coms coms = null;

        public TCPClient(ClientWindow handler)
        {
            mainThrd = new Thread(Listen);
            mainThrd.Start();
            this.handler = handler;
        }

        private Socket Connect()
        {
            Types.Config config = Config.GetConfig();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (socket.Connected == false)
            {
                try
                {
                    socket.Connect(config.server, 9999);
                    Console.WriteLine("Connected");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return socket;
        }

        public void Disconnect()
        {
            coms.Close();
        }


        private void Listen()
        {
            coms = new Coms(Connect());
            UpdateBuffs();
            Thread infoThrd = new Thread(MemberInfo);
            infoThrd.Start();
            


            while (coms.Connected())
            {
                dynamic request = coms.Receive();
                switch ((Types.Actions)request.buff.action)
                {
                    case Types.Actions.Buff:
                        Buff(request.buff.buff);
                        coms.Response((int)request.sn);
                        break;
                    case Types.Actions.GroupHeal:
                        GroupHeal();
                        coms.Response((int)request.sn);
                        break;
                    case Types.Actions.Support:
                        Support();
                        coms.Response((int)request.sn);
                        break;
                    case Types.Actions.Invite:
                        Console.WriteLine("Client Accept invite");
                        if (AsteriosManager.OpenWindow() == false)
                        {
                            return;
                        }
                        Keyboard.AcceptParty();
                        coms.Response((int)request.sn);
                        break;
                    default:
                        break;
                }
            }
            System.Environment.Exit(-1);
        }

        private void Buff(dynamic buff)
        {
            if (AsteriosManager.OpenWindow() == false)
            {
                return;
            }

            Console.WriteLine(buff.name);
            Console.WriteLine(buff.key);
            Console.WriteLine(buff.delay);

            Keyboard.PressKey((string)buff.key);
            Thread.Sleep((int)buff.delay);

        }

        private void GroupHeal()
        {
            Console.WriteLine("Client GroupHeal");
            if (AsteriosManager.OpenWindow() == false)
            {
                return;
            }

            try
            {
                Types.Action buff = Config.GetAction("groupheal");
                if (buff.key.Length > 0)
                {
                    Keyboard.PressKey(buff.key);
                    Thread.Sleep(buff.delay);
                }
            }
            catch (Exception ex)
            {

            }
           
        }

        private void Support()
        {
            Console.WriteLine("Client Support");
            if (AsteriosManager.OpenWindow() == false)
            {
                return;
            }

            try
            {
                Types.Action buff = Config.GetAction("support");
                if (buff.key.Length > 0)
                {
                    Keyboard.PressKey(buff.key);
                    Thread.Sleep(buff.delay);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void MemberInfo()
        {
            Types.Config config = Config.GetConfig();
            Types.MemberInfo memberInfo = new Types.MemberInfo();
            memberInfo.name = config.name;
            memberInfo.prof = config.prof;
            memberInfo.lang = config.lang;
            memberInfo.party = config.party;
            memberInfo.support = config.support;

            while (coms.Connected())
            {
                var tx = new
                {
                    action = Types.Actions.MemberInfo,
                    memberInfo = memberInfo,
                };

                dynamic response = coms.Send(tx, 2000);
                if (response != null)
                {
                    handler.UpdateMembersInfo(response);
                    Thread.Sleep(1000);
                }

            }
            System.Environment.Exit(-1);
        }

        private void UpdateBuffs() 
        {
            List<Types.Action> buffs = Config.GetBuffs();
            var tx = new
            {
                action = Types.Actions.UpdateBuffs,
                buffs = buffs
            };

            dynamic response = coms.Send(tx, 2000);
        }
    }
}
