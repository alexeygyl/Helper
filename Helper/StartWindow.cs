using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public partial class StartWindow : Form
    {
        private static List<int> l2s = new List<int>();

        public StartWindow()
        {
            InitializeComponent();
            Thread thrdCPCheck = new Thread(UpdateProcessList);
            thrdCPCheck.Start();
            UpdatecConfigList();
        }

        private void UpdateProcessList()
        {
            while (true)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        this.comboBox1.Items.Clear();
                        Process[] processes = Process.GetProcesses();
                        foreach (Process process in processes)
                        {
                            if (process.ProcessName.Contains("AsteriosGame") == true)
                            {
                                l2s.Add(process.Id);
                                this.comboBox1.Items.Add(process.Id);                            }
                        }
                    }));
                }

                Thread.Sleep(3000);
            }
        }

        private void UpdatecConfigList()
        {
            DirectoryInfo d = new DirectoryInfo(@"./Configs");
            FileInfo[] Files = d.GetFiles("*.xml");
            foreach (FileInfo file in Files)
            {
                this.comboBox2.Items.Add(file.Name);
            }
        }

        private void StartWindow_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(-1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AsteriosManager.OpenWindow(Int32.Parse(comboBox1.SelectedItem.ToString()));
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.Load("./Configs/" + comboBox2.SelectedItem.ToString());
        }

        private void Open_Click(object sender, EventArgs e)
        {
            if (Config.GetType() == "client")
            {
                ClientWindow clientWindow = new ClientWindow();
                clientWindow.Text = Config.GetName();
                clientWindow.Show();
                this.Hide();
            }
            else if (Config.GetType() == "server")
            {
                ServerWindow serverWindow = new ServerWindow();
                serverWindow.Text = Config.GetName();
                serverWindow.Show();
                this.Hide();
            }

        }
    }
}
