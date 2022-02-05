using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public partial class ServerWindow : Form
    {

        private static bool loop = true;
        private static Socket listenSocket;
        

        public ServerWindow()
        {
            InitializeComponent();
            Thread serverThrd = new Thread(ServerThrd);
            serverThrd.Start();
        }

        public  void UpdateMembersList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    List<Types.MemberInfo> members = MemberManager.GetMembersInfo();
                    int pos = 0;
                    //Console.WriteLine(members.Count);
                    foreach (dynamic member in members)
                    {
                        UpdateMemberInfo(member, pos++);
                    }

                    for (; pos <= 8; pos++)
                    {
                        RemoveMemberInfo(pos);
                    }

                }));
            }
        }

        private void ServerThrd()
        {

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 9999);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            UpdateMembersList();
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(100);

                while (loop)
                {
                    Socket handler = listenSocket.Accept();
                    MemberManager.Add(new ServerMember(handler, this));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void UpdateMemberInfo(dynamic member, int pos)
        {
            switch (pos)
            {
                case 0: Member0.Text = "[ " + member.prof + " ] " + member.name; break;
                case 1: Member1.Text = "[ " + member.prof + " ] " + member.name; break;
                case 2: Member2.Text = "[ " + member.prof + " ] " + member.name; break;
                case 3: Member3.Text = "[ " + member.prof + " ] " + member.name; break;
                case 4: Member4.Text = "[ " + member.prof + " ] " + member.name; break;
                case 5: Member5.Text = "[ " + member.prof + " ] " + member.name; break;
                case 6: Member6.Text = "[ " + member.prof + " ] " + member.name; break;
                case 7: Member7.Text = "[ " + member.prof + " ] " + member.name; break;
                case 8: Member8.Text = "[ " + member.prof + " ] " + member.name; break;
            }
        }

        private void RemoveMemberInfo(int pos)
        {
            switch (pos)
            {
                case 0: Member0.Text = ""; break;
                case 1: Member1.Text = ""; break;
                case 2: Member2.Text = ""; break;
                case 3: Member3.Text = ""; break;
                case 4: Member4.Text = ""; break;
                case 5: Member5.Text = ""; break;
                case 6: Member6.Text = ""; break;
                case 7: Member7.Text = ""; break;
                case 8: Member8.Text = ""; break;
            }
        }

        private void ServerWindow_Load(object sender, EventArgs e)
        {

        }

        private void ServerWindow_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loop = false;

            try
            {
                listenSocket.Shutdown(SocketShutdown.Both);
                listenSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            System.Environment.Exit(-1);
        }
    }
}
