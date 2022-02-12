using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class ServerMember
    {
        private Types.MemberInfo memberInfo = new Types.MemberInfo();
        private ServerWindow serverWindow = null;
        private Coms coms = null;

        public ServerMember(Socket handler, ServerWindow serverWindow)
        {
            coms = new Coms(handler);
            this.serverWindow = serverWindow;
            Thread authThrd = new Thread(RxCallback);
            authThrd.Start();
        }

      
        private void RxCallback()
        {
            while (coms.Connected() == true)
            {
                try
                {
                    dynamic request = coms.Receive();
                    //Console.WriteLine("Request {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
                    switch ((Types.Actions)request.buff.action)
                    {
                        case Types.Actions.MemberInfo:
                            memberInfo.name = request.buff.memberInfo.name;
                            memberInfo.prof = request.buff.memberInfo.prof;
                            memberInfo.hp = request.buff.memberInfo.hp;
                            serverWindow.UpdateMembersList();
                            dynamic response = MemberManager.GetMembersInfo();
                            coms.Response((int)request.sn, response);
                            //Console.WriteLine("Response {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            if (coms.Connected() == false)
            {
                Console.WriteLine("Remove client ");
                MemberManager.Delete(this);
                serverWindow.UpdateMembersList();
            }

        }

        public Types.MemberInfo GetMemberInfo()
        {
            return memberInfo;
        }

        public void Buff(dynamic buffs)
        {
            var tx = new
            {
                action = Types.Actions.Buff,
                buffs = buffs
            };

            coms.Send(tx, 20000);
        }

    }
}
