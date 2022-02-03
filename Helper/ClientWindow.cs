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
    public partial class ClientWindow : Form
    {
        private TCPClient memberClient;
        public ClientWindow()
        {
            InitializeComponent();
            memberClient = new TCPClient(this);
            for (int pos = 0; pos <= 8; pos++)
            {
                RemoveMemberInfo(pos);
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {

        }


        private void ClientWindow_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            memberClient.Disconnect();
            System.Environment.Exit(-1);
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
                case 1: Member1.Text = ""; break;
                case 2: Member2.Text = ""; break;
                case 0: Member0.Text = ""; break;
                case 3: Member3.Text = ""; break;
                case 4: Member4.Text = ""; break;
                case 5: Member5.Text = ""; break;
                case 6: Member6.Text = ""; break;
                case 7: Member7.Text = ""; break;
                case 8: Member8.Text = ""; break;
            }
        }

        public void UpdateMembersInfo(dynamic members)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    int pos = 0;
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


    }
}
