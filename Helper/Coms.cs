using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class Coms
    {
        private Socket socket;
        private Queue rx = new Queue();
        Dictionary<long, dynamic> tx = new Dictionary<long, dynamic>();
        Mutex mutex = new Mutex();

        public enum Dir
        {
            Request  = 1,
            Response,
        }

        public Coms(Socket socket)
        {
            this.socket = socket;
            Thread serverThrd = new Thread(Main);
            serverThrd.Start();
        }

        public bool Connected()
        {
            return socket.Connected;
        }

        public void Close()
        {
            socket.Close();
        }

        private void Main()
        {
            while (socket.Connected)
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
                    //Console.WriteLine("Main {0}", builder.ToString());
                    dynamic output = Newtonsoft.Json.JsonConvert.DeserializeObject(builder.ToString());
                    if (output.dir == Dir.Request)
                    {
                        rx.Enqueue(output);
                    }
                    else if (output.dir == Dir.Response)
                    {
                        mutex.WaitOne();
                        if (tx.ContainsKey((int)output.sn) == true)
                        {
                            tx[(int)output.sn] = output;
                        }
                        mutex.ReleaseMutex();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public dynamic Receive()
        {
            while (rx.Count <= 0)
            {
                if (socket.Connected == false)
                {
                    throw new Exception("Socket close");
                }
                Thread.Sleep(50);
            }

            return rx.Dequeue();
        }

        public void Response(int sn, dynamic buff = null)
        {
            try
            {
                dynamic toSend;
                if (buff == null)
                {
                    toSend = new { sn = sn, dir = Dir.Response};
                }
                else
                {
                    toSend = new { sn = sn, dir = Dir.Response, buff = buff};
                }
                
                byte[] txbuff = Encoding.Unicode.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(toSend));
                socket.Send(txbuff);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public dynamic Send(dynamic buff, int timeout = 30000)
        {
            mutex.WaitOne();
            Random rnd = new Random();
            dynamic output = null;
 
            var toSend = new
            {
                sn = rnd.Next(int.MaxValue),
                dir = Dir.Request,
                buff = buff,
            };

            tx.Add((int)toSend.sn, null);
            mutex.ReleaseMutex();
            try
            {
                //Console.WriteLine("Request {0}", Newtonsoft.Json.JsonConvert.SerializeObject(toSend));
                byte[] txbuff = Encoding.Unicode.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(toSend));
                socket.Send(txbuff);
                while (timeout > 0)
                {
                    if (tx[(int)toSend.sn] != null)
                    {
                        output = tx[(int)toSend.sn];
                        tx.Remove((int)toSend.sn);
                        //Console.WriteLine("Response {0}", Newtonsoft.Json.JsonConvert.SerializeObject(toSend));
                        return output.buff;
                    }
                    Thread.Sleep(50);
                    timeout -= 50;
                }
            }
            catch (Exception ex)
            {
                tx.Remove(toSend.sn);
                Console.WriteLine(ex.Message);
            }

            if (tx.ContainsKey((int)toSend.sn))
            {
                tx.Remove((int)toSend.sn);
            }

            return null;
        }
    }
}
