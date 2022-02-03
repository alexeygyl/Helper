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
        private Socket handler = null;
        private bool loop = true;
        private bool authorized = false;
        private Types.MemberInfo memberInfo = new Types.MemberInfo();
        private ServerWindow serverWindow = null;

        public ServerMember(Socket handler, ServerWindow serverWindow)
        {
            this.handler = handler;
            this.serverWindow = serverWindow;
            Thread authThrd = new Thread(Authorize);
            authThrd.Start();
        }

        private void Send(string data)
        {
            try
            {
                byte[] txbuff = Encoding.Unicode.GetBytes(data);
                handler.Send(txbuff);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string Receive()
        {
            try
            {
                int bytes;
                StringBuilder builder = new StringBuilder();
                byte[] rxbuff = new byte[256];
                do
                {
                    bytes = handler.Receive(rxbuff, rxbuff.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(rxbuff, 0, bytes));
                }
                while (handler.Available > 0);

                return builder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        private void Authorize()
        {
            while (handler.Connected)
            {
                string data = Receive();
                if (data == "")
                {
                    continue;
                }
                dynamic output = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (output.action == Types.Action.Auth)
                {
                    memberInfo.name = output.memberInfo.name;
                    memberInfo.prof = output.memberInfo.prof;
                    authorized = true;
                    Thread memberInfoThrd = new Thread(UpdateMemberInfo);
                    memberInfoThrd.Start();
                    serverWindow.UpdateMembersList();
                    break;
                }
            }

            if (handler.Connected == false)
            {
                MemberManager.Delete(this);
            }
            
        }

        private void UpdateMemberInfo()
        {
            while (handler.Connected)
            {
                var data = new
                {
                    action = Types.Action.MembersInfo,
                    membersInfo = MemberManager.GetMembersInfo(),
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                Send(json);
                Thread.Sleep(500);
            }

            memberInfo.name = "";
            serverWindow.UpdateMembersList();
            MemberManager.Delete(this);
        }

        public Types.MemberInfo GetMemberInfo()
        {
            return memberInfo;
        }

    }
}
