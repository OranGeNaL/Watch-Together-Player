using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace serverProject
{
    class ServerObject
    {
        TcpListener server;
        List<ClientObject> connectedClients = new List<ClientObject>();

        public ServerObject(TcpListener tcpListener)
        {
            server = tcpListener;
        }
        public void Process()
        {
            server.Start();
            Thread onlineInfoThread = new Thread(new ThreadStart(OnlineInfo));
            //onlineInfoThread.Start();

            while (true)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client, this);
                    connectedClients.Add(clientObject);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void OnlineInfo()
        {
            while (true)
            {
                Thread.Sleep(5000);
                string info = String.Format("{0}: {1} клиентов онлайн.", DateTime.Now.ToShortTimeString(), connectedClients.Count);
                foreach (var i in connectedClients)
                {
                    info += String.Format("\n{0}", i.ToString());
                }
                Console.WriteLine(info);
            }
        }

        public void RemoveConnetcedUser(ClientObject client)
        {
            Console.WriteLine("{0} отключился", client.ToString());
            connectedClients.Remove(client);
        }
    }
}
