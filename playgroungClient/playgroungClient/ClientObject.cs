using serverProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace playgroungClient
{
    class ClientObject
    {
        public Form1 parent;
        private string IP;
        private int PORT;
        //static TcpClient client = null;
        static NetworkStream networkStream = null;

        public ClientObject(string ip, int port)
        {
            IP = ip;
            PORT = port;
        }

        public void SendMessage(string message)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(IP, PORT);
                networkStream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);

                Stop(client);
            }
            catch
            {

            }
        }

        public void SendFile(object fileNameObj)
        {
            TcpClient client = new TcpClient();
            string fileName = (string)fileNameObj;
            try
            {
                SendInfo si = new SendInfo();
                si.message = Request.SENDING_FILE;

                FileInfo fi = new FileInfo(fileName);
                si.filesize = fi.Length;
                si.filename = fi.Name;



                client.Connect(IP, PORT);
                networkStream = client.GetStream();
                SendRequest(si.message);

                BinaryFormatter bf = new BinaryFormatter();

                if (GetResponse() == Request.OK)
                {
                    //SendRequest(si.filesize.ToString());
                    bf.Serialize(networkStream, si);

                    if (GetResponse() == Request.OK)
                    {
                        FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                        byte[] buff = new byte[fs.Length];
                        fs.Read(buff, 0, buff.Length);
                        networkStream.Write(buff, 0, buff.Length);
                        parent.SendCompleteMessage("Передача файла завершена.");
                    }
                }


                /*if(GetResponse() == Request.OK)
                {
                    SendRequest(si.filename);
                    if (GetResponse() == Request.OK)
                    {
                        SendRequest(si.filesize.ToString());
                        if (GetResponse() == Request.OK)
                        {
                            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                            byte[] buff = new byte[fs.Length];
                            fs.Read(buff, 0, buff.Length);
                            networkStream.Write(buff, 0, buff.Length);
                            parent.SendCompleteMessage("Передача файла завершена.");
                        }
                    }
                }*/
            }
            catch(Exception ex)
            {
                parent.SendCompleteMessage(String.Format("Передача файла не была завершена. {0}", ex.Message));
            }
            finally
            {
                Stop(client);
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

            return builder.ToString();
        }

        private void Stop(TcpClient client)
        {
            if (networkStream != null)
                networkStream.Close();
            client.Close();
            //Environment.Exit(0);
        }

        
    }
}
