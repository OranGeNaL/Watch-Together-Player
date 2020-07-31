using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace serverProject
{
    class Program
    {
        static string IP = "127.0.0.1";
        static int PORT = 5505;
        static ServerObject server;

        static void Main(string[] args)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(IP), PORT);
                server = new ServerObject(listener);
                Thread serverThread = new Thread(new ThreadStart(server.Process));
                serverThread.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
