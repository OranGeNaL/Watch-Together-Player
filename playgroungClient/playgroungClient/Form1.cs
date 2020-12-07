using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace playgroungClient
{
    public partial class Form1 : Form
    {

        ClientObject client;

        public Form1()
        {
            InitializeComponent();
            client = new ClientObject("127.0.0.1", 5505);
            client.parent = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //client.SendMessage(textBox1.Text);
            SendInfo sendInfo = new SendInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
                label1.Text = fileDialog.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(label1.Text != "")
            {
                try
                {
                    Thread fileSendThread = new Thread(new ParameterizedThreadStart(client.SendFile));
                    fileSendThread.Start(label1.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void SendCompleteMessage(string message)
        {
            MessageBox.Show(message);
        }

    }
}
