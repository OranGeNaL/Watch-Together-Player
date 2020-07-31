using playgroungClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace serverProject
{
    class ClientObject
    {
        TcpClient client;
        ServerObject server;
        NetworkStream networkStream = null;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
        }

        public void Process()
        {
            try
            {
                networkStream = client.GetStream();
                Thread getFileThread = new Thread(new ThreadStart(GetFile));
                getFileThread.Start();
            }
            catch
            {
                server.RemoveConnetcedUser(this);
            }
            finally
            {
                server.RemoveConnetcedUser(this);
            }
            
        }

        private void GetFile()
        {
            if(networkStream != null)
            {
                try
                {
                    if (GetResponse() == Request.SENDING_FILE)
                    {
                        SendInfo si = new SendInfo();
                        SendRequest(Request.OK);
                        Console.WriteLine("Начато получение файла.");
                        si.filename = GetResponse();
                        Console.WriteLine("Имя файла: {0}", si.filename);
                        SendRequest(Request.OK);
                        si.filesize = long.Parse(GetResponse());
                        Console.WriteLine("Размер файла: {0}", si.filesize);
                        SendRequest(Request.OK);

                        FileStream fs = new FileStream(si.filename, FileMode.Create, FileAccess.ReadWrite,
                            FileShare.ReadWrite, (int)si.filesize);
                        long bytesGot = 0;
                        do
                        {
                            byte[] temp = new byte[262144];
                            int bytesThisTick = networkStream.Read(temp, 0, temp.Length);
                            bytesGot += bytesThisTick;
                            fs.Write(temp, 0, bytesThisTick);

                            Console.WriteLine("Получено {0}% файла.", Math.Round((double)bytesGot / si.filesize * 100));

                            if (fs.Length == si.filesize)
                            {
                                fs.Close();
                                break;
                            }
                        }
                        while (bytesGot < si.filesize);
                        Console.WriteLine("Передача файла успешно завершена!");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Ошибка при передаче файла.");
                }
                finally
                {
                    Stop();
                }
            }
        }

        private void SendRequest(string request)
        {
            byte[] data = Encoding.UTF8.GetBytes(request);
            networkStream.Write(data, 0, data.Length);
        }

        private string GetResponse()
        {
            byte[] data = new byte[1024];
            int bytes = 0;
            StringBuilder builder = new StringBuilder();
            do
            {
                bytes = networkStream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (networkStream.DataAvailable);
            //Console.WriteLine("Полученная строка: {0}", builder.ToString());
            return builder.ToString();
        }

        private void Stop()
        {
            if (networkStream != null)
                networkStream.Close();
            if (client != null)
                client.Close();
            server.RemoveConnetcedUser(this);
        }

        public override string ToString()
        {
            return String.Format("{0}", client.ToString());
        }
    }
}
