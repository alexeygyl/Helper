﻿using System;
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
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

            return socket;
        }

        public void Disconnect()
        {
            coms.Close();
        }


        private void Listen()
        {
            coms = new Coms(Connect());
            Thread infoThrd = new Thread(MemberInfo);
            infoThrd.Start();

            while (coms.Connected())
            {
                dynamic request = coms.Receive();
                switch ((Types.Action)request.action)
                {
                    default:
                        break;
                }
            }
            System.Environment.Exit(-1);
        }

        
        private void MemberInfo()
        {
            Types.MemberInfo memberInfo = new Types.MemberInfo();
            memberInfo.name = Config.GetName();
            memberInfo.prof = Config.GetProf();

            while (coms.Connected())
            {
                var tx = new
                {
                    action = Types.Action.MemberInfo,
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
        
    }
}
