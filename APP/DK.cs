using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APP
{
    public partial class DK : Form
    {
        public class Data
        {
            public string email { get; set; }
            public string password { get; set; }
            public string userName { get; set; }
        }
        public DK(TcpClient tcpClient)
        {
            InitializeComponent();
            this.tcpClient = tcpClient;
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;
        }
        TcpClient tcpClient;
        StreamReader reader;
        StreamWriter writer;
        private void ConnectToServer()
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 8080);

            }
            catch
            {
                MessageBox.Show("Sever isn't running!");
                return;
            }
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;
        }
        private void baoloi()
        {
            label7.Visible = true;
        }

        private void DK_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LogIn form = new LogIn();
            form.Visible = true;
            this.Close();

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            LogIn lg = new LogIn();
            lg.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            // kiểm các textbox
            if (string.IsNullOrEmpty(bunifuTextBox1.Text) || string.IsNullOrEmpty(bunifuTextBox2.Text) ||
                string.IsNullOrEmpty(bunifuTextBox3.Text) || string.IsNullOrEmpty(bunifuTextBox4.Text)
                || (bunifuTextBox2.Text != bunifuTextBox4.Text))
            {
                baoloi();
                bunifuTextBox1.Clear();
                bunifuTextBox2.Clear();
                bunifuTextBox3.Clear();
                bunifuTextBox4.Clear();
                return;
            }
            // tạo thread để giao tiếp với server
            Thread threadSignin = new Thread(Register);
            threadSignin.Start();
            threadSignin.IsBackground = true;
        }
        private void Register()
        {
            Data signup = new Data()
            {
                userName = bunifuTextBox1.Text,
                email = bunifuTextBox3.Text,
                password = bunifuTextBox4.Text,
            };
            // gửi thông điệp yêu cầu đăng kí và thông tin đắng kí
            string signupString = JsonConvert.SerializeObject(signup);
            writer.WriteLine("Register");
            writer.WriteLine(signupString);

            //Nhận phản hồi từ server
            string responseFromServer = reader.ReadLine();
            string username = responseFromServer.Substring(0, responseFromServer.IndexOf(":"));
            string response = responseFromServer.Substring(responseFromServer.IndexOf(":") + 1);
            if (response.CompareTo("Register successfully") == 0)
            {
                Invoke(new Action(() =>
                {
                    Chat chat = new Chat(tcpClient,username);
                    chat.Show();
                    this.Hide();
                }));
            }
            else
            {
                //MessageBox.Show("Register failed");
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Khong thanh cong");
                    bunifuTextBox1.Clear();
                    bunifuTextBox2.Clear();
                    bunifuTextBox3.Clear();
                    bunifuTextBox4.Clear();
                }));
            }
        }
    }
}
