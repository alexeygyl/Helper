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
        private bool loop = true;
        private bool authorized = false;
        private ClientWindow handler = null;
        private Socket socket;
        private Thread mainThrd;
        private Thread authThrd;

        public TCPClient(ClientWindow handler)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainThrd = new Thread(Listen);
            mainThrd.Start();
            this.handler = handler;
        }

        private void Connect()
        {
            while (socket.Connected == false)
            {
                try
                {
                    socket.Connect(Config.GetServer(), 9999);
                    Console.WriteLine("Connected");
                   
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Disconnect()
        {
            loop = false;
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public void Send(string data)
        {
            try
            {
                byte[] txbuff = Encoding.Unicode.GetBytes(data);
                socket.Send(txbuff);
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
                    bytes = socket.Receive(rxbuff, rxbuff.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(rxbuff, 0, bytes));
                }
                while (socket.Available > 0);

                return builder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        private void Listen()
        {
            Connect();
            Thread authThrd = new Thread(Authrize);
            authThrd.Start();

            while (socket.Connected)
            {
                Thread.Sleep(100);
                string data = Receive();
                if (data.Length <= 0)
                {
                    continue;
                }

                dynamic output = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

                switch ((Types.Action)output.action)
                {
                    case Types.Action.MembersInfo:
                        handler.UpdateMembersInfo(output.membersInfo);
                        authorized = true;
                        break;
                    default:
                        break;
                }

 
                //Console.WriteLine(data);
                
            }
            System.Environment.Exit(-1);
        }
        private void Authrize()
        {
            Types.MemberInfo memberInfo = new Types.MemberInfo();
            memberInfo.name = Config.GetName();
            memberInfo.prof = Config.GetProf();

            var myData = new
            {
                action = Types.Action.Auth,
                memberInfo = memberInfo,
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(myData);

            while (authorized == false)
            {
                Send(json);
                Thread.Sleep(5000);
            }
        }
    }
}
