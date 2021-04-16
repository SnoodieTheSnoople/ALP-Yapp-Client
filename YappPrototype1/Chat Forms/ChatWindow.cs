using Org.BouncyCastle.Utilities.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
//using System.Net.Sockets.Shutdown;

namespace YappPrototype1.Chat_Forms
{
    public partial class ChatWindow : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default;
        string readData = null;
        System.Net.IPAddress IP;
        int portNum;
        string username;
        string email;

        public ChatWindow(string username, string email)
        {
            InitializeComponent();
            this.username = username;
            this.email = email;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void ConnectWindow()
        {
            ConnectForm connect = new ConnectForm();
            connect.ShowDialog();

            if (connect.connectedFlag)
            {
                IP = connect.IP;
                portNum = connect.port;
                clientSocket = connect.clientSocket;

                Chat_TextBox1.Clear();
                readData = $"Connected to server {IP}:{portNum}";
                ServerMsg();

                serverStream = clientSocket.GetStream();

                byte[] outStream = Encoding.UTF8.GetBytes(username + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(GetMsg);
                ctThread.Start();
            }
            else
            {
                readData = $"Failed to connect...";
                ServerMsg();
                readData = null;
            }
        }

        private void GetMsg()
        {
            while (true)
            {
                try
                {
                    serverStream = clientSocket.GetStream();
                    int buffSize = clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[buffSize];
                    int bytesRead = serverStream.Read(inStream, 0, buffSize);

                    if (bytesRead == 0)
                    {
                        throw new IOException("Lost connection to server");
                    }

                    string returnData = Encoding.UTF8.GetString(inStream);
                    readData = "" + returnData;

                    if (readData.Contains("/namelist/"))
                    {
                        string names = readData.Remove(0, readData.LastIndexOf("/") + 2);
                        string[] nameList = names.Split(", ");

                        foreach (string name in nameList)
                        {
                            Invoke((MethodInvoker) (() => userListBox.Items.Add(name) ));
                        }
                        readData = readData.Remove(readData.IndexOf("/"), readData.Length - readData.IndexOf("/"));
                        ServerMsg();
                    }

                    else if (readData.Contains("/join/")) 
                    {
                        string name = readData.Remove(0, readData.LastIndexOf("/") + 2);
                        Invoke((MethodInvoker) ( () => userListBox.Items.Insert(userListBox.Items.Count - 1, name) ) );
                    }

                    else if (readData.Contains("/leave/"))
                    {
                        string name = readData.Remove(0, readData.LastIndexOf("/") + 2);
                        
                        Invoke((MethodInvoker)(() =>
                        {
                            readData = userListBox.Items.IndexOf(name).ToString();
                            int index = userListBox.FindStringExact(name);
                            userListBox.Items.RemoveAt(index);
                        }));
                    }
                    else
                    {
                        ServerMsg();
                    }
                }
                catch (IOException)
                {
                    Disconnect();
                    break;
                }
            }
        }

        private void ServerMsg()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ServerMsg));
            }
            else
            {
                string time = GetTime();
                Chat_TextBox1.AppendText($"{Environment.NewLine}[{time}] {readData}");
                Chat_TextBox1.ScrollToCaret();
            }
        }

        private string GetTime()
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            return time;
        }

        private void Disconnect()
        {
            try
            {
                if (serverStream == null)
                {
                    return;
                }
                serverStream.Close();
                clientSocket.Close();

                readData = $"Disconnected from server...";
                Invoke((MethodInvoker)(() => userListBox.Items.Clear()));
                ServerMsg();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Not connected to server.", "Yapp! Error");
            }
            catch (ArgumentException) { }
            catch (Exception) { }
        }


        private void Chat_TextBox1_TextChanged(object sender, EventArgs e) { }

        private void SendMsg_Btn1_Click(object sender, EventArgs e)
        {
            if (!Msg_TextBox2.Text.Equals(""))
            {
                try
                {
                    serverStream = clientSocket.GetStream();
                    byte[] outStream = Encoding.UTF8.GetBytes(Msg_TextBox2.Text + "$");
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    Msg_TextBox2.Clear();
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Cannot send message...", "Yapp! Error");
                    Msg_TextBox2.Clear();
                }
            }
        }

        private void C_ConnectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConnectWindow();
        }

        private void C_DisconnectToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void BmarkToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            BookmarksForm bookmarksForm = new BookmarksForm();
            bookmarksForm.Show();
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e) { }

        private void C_QuitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Disconnect();
            Application.Exit();
        }

        private void SendMsg_Btn1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMsg_Btn1_Click(this, new EventArgs());
                e.SuppressKeyPress = true;
            }
        }

        private void T_AccDetailsMenuItem1_Click(object sender, EventArgs e)
        {
            AccDetails acc = new AccDetails(username, email);
            acc.Show();
        }

        private void T_OutCLogMenuItem1_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Would you like to log the chat in a text file?", "Yapp!", MessageBoxButtons.OKCancel);

            if (dialogResult == DialogResult.OK)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\chatLog.txt";
                File.WriteAllText(path, Chat_TextBox1.Text);
            }
        }
    }
}
