using Bunifu.UI.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace APP
{
    public partial class Chat : Form
    {
        private Thread receiveThread;
        private TcpClient client;
        private string username;
        private ChatlistUser[] chatlistUsers;
        private UserFriend[] listFriends;
        private sendMessage seMessages;
        private reMessage reMessages;
        private StreamReader reader;
        private bool isRunning = false;
        private StreamWriter writer;
        //list bạn bè tạm
        string[] friendList;
        string[] userList;
        string roomList;
        // tạo ra 1 list các pair chatlistuser và flowlayoutpenl
        //private Dictionary<ChatlistUser, Panel> chatListUserToFlowLayoutPanelMap = new Dictionary<ChatlistUser, Panel>();
        private Dictionary<ChatlistUser, FlowLayoutPanel> chatListUserToFlowLayoutPanelMap = new Dictionary<ChatlistUser, FlowLayoutPanel>();
        public Chat(TcpClient tcpClient, string username)
        {
            this.client = tcpClient;
            this.username = username;
            this.isRunning = true;
            InitializeComponent();
            bunifuLabel1.Text = username;
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
            //yêu cầu danh sách ds user
            writer.WriteLine("ListUser");

            //Nhận ds user
            string temp1 = reader.ReadLine();
            userList = temp1.Split('|');

            //yêau cầu nhận ds bạn bè
            writer.WriteLine("Listfriend");
            
            //nhận ds bạn bè
            string temp = reader.ReadLine();
            if (temp == "Null")
            {
            }
            else
            {
                friendList = temp.Split('|');
            }
            //convertRoomList();
            //writer.WriteLine("LoadMessage");
            //writer.WriteLine(roomList);
            //
            Thread thread = new Thread(Receive);
            thread.Start();
            thread.IsBackground = true;
            detailAn();
            Colorcolumn1();
            listfriendshow();
            listconversation();
        }
        //cấu trúc 1 tin nhắn được gửi đi 
        class tinNhan
        {
            public string sender { get; set; }
            public string contentMess { get; set; }
            public string receiver { get; set; }
            public string roomkey { get; set; }
        }
        private void detailAn()
        {
            Detail.Visible = false;
            listFriend.Visible = false;
            MoreConversation.Visible = false;
            searchchat.Visible = false;
        }
        private void Colorcolumn1()
        {
           
                conversation.BackColor = Color.LightGray;
            
        }
        private void Chat_Load(object sender, EventArgs e)
        {
            
        }

        private void bunifuPanel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPanel4_Click(object sender, EventArgs e)
        {
           
          
            
        }
        
        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
           
            if (Detail.Visible == false)
            {
                Detail.Visible = true;
                listFriend.Visible = false;
                Chatlist.Visible = false;
                conversation.BackColor = Color.WhiteSmoke;
                add.BackColor = Color.WhiteSmoke;

            }
            else if (Detail.Visible == true)
            {
                Detail.Visible = false;
            }

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {   
           
        }

        private void listFriend_Click(object sender, EventArgs e)
        {
            
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            if (Chatlist.Visible == false)
            {
                Chatlist.Visible = true;
                listFriend.Visible = false;
                Detail.Visible = false;
                add.BackColor = Color.WhiteSmoke;
                conversation.BackColor= Color.LightGray;
            }
            else
            {
                Chatlist.Visible = false;
                conversation.BackColor = Color.WhiteSmoke;
            }
            

          
        }
        private void listfriendshow()
        {
            if (friendList != null)
            {
                if (friendList.Length == 0)
                {
                    return;
                }
                listFriends = new UserFriend[userList.Length];
                for (int i = 0; i < listFriends.Length; i++)
                {
                    bool isExist = false;
                    foreach (var item in friendList)
                    {
                        if (userList[i] == item)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        if (userList[i] != "")
                        {
                            if (userList[i] != username)
                            {
                                listFriends[i] = new UserFriend();
                                Image image = Image.FromFile("E:\\Thử\\Do_AN-master\\Do_AN-master\\APP\\image\\4.jpg");
                                listFriends[i].userimage = image;
                                listFriends[i].username = userList[i];
                                if (flowLayoutPanelListfriend.Controls.Count < 0)
                                {
                                    flowLayoutPanelListfriend.Controls.Clear();
                                }
                                else
                                {
                                    flowLayoutPanelListfriend.Controls.Add(listFriends[i]);
                                }
                            }
                        }
                    }
                }
            }
            else if (friendList == null)
            {
                listFriends = new UserFriend[userList.Length];
                for (int i = 0; i < listFriends.Length; i++)
                {
                    if (userList[i] != "")
                    {
                        if (userList[i] != username)
                        {
                            listFriends[i] = new UserFriend();
                            Image image = Image.FromFile("E:\\Thử\\Do_AN-master\\Do_AN-master\\APP\\image\\4.jpg");
                            listFriends[i].userimage = image;
                            listFriends[i].username = userList[i];
                            if (flowLayoutPanelListfriend.Controls.Count < 0)
                            {
                                flowLayoutPanelListfriend.Controls.Clear();
                            }
                            else
                            {
                                flowLayoutPanelListfriend.Controls.Add(listFriends[i]);
                            }
                        }
                    }
                }
            }
        }
        private void listconversation()
        {
            if (friendList != null)
            {
                chatlistUsers = new ChatlistUser[friendList.Length];
                for (int i = 0; i < chatlistUsers.Length; i++)
                {
                    if (friendList[i] != "")
                    {
                        chatlistUsers[i] = new ChatlistUser();
                        Image image = Image.FromFile("E:\\Thử\\Do_AN-master\\Do_AN-master\\APP\\image\\4.jpg");
                        // tạo ra 1 chatlistuser
                        chatlistUsers[i].username = friendList[i];
                        chatlistUsers[i].userimage = image;
                        // tạo ra 1 flowlayoutpanel tương ứng với chatlistuser ở trên
                        //Panel tempFlowLayoutPanel = createFlowlayoutPanel();
                        FlowLayoutPanel tempFlowLayoutPanel = createFlowlayoutPanel();
                        chatListUserToFlowLayoutPanelMap.Add(chatlistUsers[i], tempFlowLayoutPanel);
                        // gán sự kiện hiển thị flowlayoutpanel khi ấn vào chatlistuser tương ứng
                        chatlistUsers[i].MouseDown += click;
                        ContactNameConversation.Text = "Unknow";
                        ContactNameMore.Text = "Unknow";
                        // thêm chatlistuser vào panel bên trái 
                        ChatlistFlowPanel.Controls.Add(chatlistUsers[i]);
                    }
                }
            }
        }
        private void click(object sender, EventArgs e)
        {
            //bắt sự kiện xem chatlistuser nào được ấn
            ChatlistUser clickedChatListUser = (ChatlistUser)sender;    
            if (chatListUserToFlowLayoutPanelMap.ContainsKey(clickedChatListUser))
            {
                // tìm flowlayoutpanel tương ứng để hiển thị lên
                foreach (var item in chatListUserToFlowLayoutPanelMap)
                {
                    if (item.Key == clickedChatListUser)
                    {
                        item.Value.Visible = true;
                        bunifuPanel12.Visible = true;
                        bunifuTextBox3.Clear();
                        ContactNameConversation.Text = item.Key.username;
                        ContactNameMore.Text = item.Key.username;
                    }
                    else
                    {
                        item.Value.Visible = false;
                    }
                }
            }
        }
        // tạo ra một flowlayoutpanel 
        
        private FlowLayoutPanel createFlowlayoutPanel()
        {

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Location = new Point(0, 183);
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.AllowDrop = true;
           
            flowLayoutPanel.BackColor = Color.White;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.BackColor = Color.White;
            
            panel2.Controls.Add(flowLayoutPanel);
            //flowLayoutPanel.AutoSize = true;
            //flowLayoutPanel.Size = new Size(225, 718);
            flowLayoutPanel.Visible = false;
            return flowLayoutPanel;
        }
        
        private void add_Click(object sender, EventArgs e)
        {
            if (listFriend.Visible == false || Detail.Visible == true)
            {
                Detail.Visible = false;
                Chatlist.Visible = false;
                listFriend.Visible = true;
                add.BackColor = Color.LightGray;
                conversation.BackColor = Color.WhiteSmoke;

            }
            else if (listFriend.Visible == true)
            {
                listFriend.Visible = false;
               add.BackColor = Color.WhiteSmoke;
            }

        }

        private void More_Click(object sender, EventArgs e)
        {
            if(MoreConversation.Visible == false)
            {
                MoreConversation.Visible = true;
            }
            else
            {
                MoreConversation.Visible = false;
            }
            
        }
        private void convertRoomList()
        {
            foreach (var item in friendList)
            {
                string temp = getRoomKey(username, item);
                roomList += temp + "|";
            }
        }
        private string getRoomKey(string username1,string username2 )
        {
            int total = 0;

            foreach (char item in username1)
            {
                total += (int)item;
            }
            foreach (char item in username2)
            {
                total += (int)item;
            }
            return total.ToString();
        }
        // gửi tin nhắn đển server
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            tinNhan newMsg = new tinNhan { sender = username, contentMess = bunifuTextBox3.Text, receiver = ContactNameConversation.Text, roomkey = getRoomKey(username,ContactNameConversation.Text)};
            string stringMess = JsonConvert.SerializeObject(newMsg);
            writer.WriteLine("Message");
            writer.WriteLine(stringMess);
            string messDisplay = $"{username}: " + bunifuTextBox3.Text + "\r\n";
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == $"{newMsg.receiver}")
                {
                    sendMessage Se = new sendMessage();
                    
                    Se.message = messDisplay;

                    Se.Dock = DockStyle.Bottom;
                    //System.Windows.Forms.Label nl1 = new System.Windows.Forms.Label();
                    //nl1.Text = messDisplay;
                    item.Value.Controls.Add(Se);
                    bunifuTextBox3.Clear();
                }
            }
        }

        private void bunifuPanel11_Click(object sender, EventArgs e)
        {

        }

        

        private void bunifuPanel5_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPanel10_Click(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton2_Click_1(object sender, EventArgs e)
        {
            if (searchchat.Visible == false)
            {
                searchchat.Visible = true;

            }
            else
            {
                searchchat.Visible = false;
            }
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            // gửi yêu cầu ngắt kết nối đến server
            string disconnect = "quit";
            writer.WriteLine(disconnect);
            // đóng kết nối tại client
            client.Close();

            // dừng luồng nhận tin nhắn từ server
            Application.Exit();
        }

        // nhận tin nhắn từ server 
        private void Receive()
        {
            try
            {
                while (isRunning)
                {
                    string messageFromServer = reader.ReadLine();

                    if (messageFromServer == "Message")
                    {
                        string newMsg = reader.ReadLine();
                        tinNhan tinNhan = JsonConvert.DeserializeObject<tinNhan>(newMsg);
                        foreach (var item in chatListUserToFlowLayoutPanelMap)
                        {
                            if (item.Key.username == tinNhan.sender)
                            {
                                Invoke(new Action(() =>
                                {
                                    reMessages = new reMessage();
                                    //System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
                                   // lb.Text = tinNhan.sender + ": " + tinNhan.contentMess;
                                   reMessages.message = tinNhan.sender + ": " + tinNhan.contentMess;
                                    //lb.Size = new Size(16, 16);

                                    item.Value.Controls.Add(reMessages);
                                    
                                    
                                }));
                            }
                        }
                    }
                    else if (messageFromServer == "Image")
                    {
                        StreamReader reader = new StreamReader(client.GetStream());
                        string senderName = reader.ReadLine();
                        string imageDataString = reader.ReadLine();
                        Image receivedImage = StringToImage(imageDataString);
                        foreach (var item in chatListUserToFlowLayoutPanelMap)
                        {
                            if (item.Key.username == senderName)
                            {
                                Invoke(new Action(() =>
                                {
                                    PictureBox newPictureBox = new PictureBox();
                                    newPictureBox.Size = new Size(250, 250);
                                    newPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                                    newPictureBox.Image = receivedImage;
                                    item.Value.Controls.Add(newPictureBox);
                                }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private Image StringToImage(string imageDataString)
        {
            Image image;
            byte[] imageBytes = Convert.FromBase64String(imageDataString);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                image = Image.FromStream(ms);
            }
            
            return image;
        }
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            writer.WriteLine("LogOut");
            LogIn lg = new LogIn();
            lg.Show();
            this.Close();
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in username)
            {
                
            }
        }

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuTextBox2.Text))
            {
                foreach (ChatlistUser item in chatlistUsers)
                {
                    if (item != null)
                    {
                        item.Visible = true;
                    }
                }
            }
            foreach (ChatlistUser item in chatlistUsers)
            {
                if (item != null)
                {
                    if (item.username.IndexOf(bunifuTextBox2.Text) != -1)
                    {
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                }
            }
        }
        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuTextBox1.Text))
            {
               if (listFriends.Length != 0)
               {
                    foreach (UserFriend item in listFriends)
                    {
                        if (item != null)
                        {
                            item.Visible = true;
                        }
                    }
               }
               else
                {

                }
            }
            foreach (UserFriend item in listFriends)
            {
                if (item != null)
                {
                    if (item.username.IndexOf(bunifuTextBox1.Text) != -1)
                    {
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        private void bunifuTextBox4_Enter(object sender, EventArgs e)
        {
            FlowLayoutPanel temp = new FlowLayoutPanel();
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == ContactNameConversation.Text)
                {
                    temp = item.Value;
                    break;
                }
            }
            if (temp != null)
            {
                foreach (Control ctl in temp.Controls)
                {
                    if (bunifuButton4.Text.IndexOf(ctl.Text) != -1)
                    {
                        ctl.Visible = true;
                    }
                    else
                    {
                        ctl.Visible = false;
                    }
                }
            }
            if (string.IsNullOrEmpty(bunifuTextBox4.Text))
            {
                foreach (Control ctl in temp.Controls)
                {
                    ctl.Visible = true;
                }
            } 
        }

        private void bunifuTextBox4_TextChange(object sender, EventArgs e)
        {
          
            
            FlowLayoutPanel temp = new FlowLayoutPanel();
            foreach (var item in chatListUserToFlowLayoutPanelMap)
            {
                if (item.Key.username == ContactNameConversation.Text)
                {
                    temp = item.Value;
                    break;
                }
            }
            if (temp != null)
            {


                /*foreach (seMessage se in temp.Controls )
                {
                    if (seMessages.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                            seMessages.Visible = true;
                    }
                    else
                    {
                            seMessages.Visible = false;
                    }
                }
                
                foreach(reMessage re in temp.Controls)
                {
                    if (re.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        re.Visible = true;
                    }
                    else
                    {
                        re.Visible = false;
                    }
                }

            }
            if (string.IsNullOrEmpty(bunifuTextBox4.Text))
            {
                foreach (Control ctl in temp.Controls)
                {
                    ctl.Visible = true;
                }}*/




                foreach (Control control in temp.Controls)
                {
                    if (control is sendMessage se && se.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        se.Visible = true;
                    }
                    else if (control is reMessage re && re.message.IndexOf(bunifuTextBox4.Text) != -1)
                    {
                        re.Visible = true;
                    }
                    else
                    {
                        control.Visible = string.IsNullOrEmpty(bunifuTextBox4.Text);
                    }
                }
            }
        }
        private string ImageToString(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        private void bunifuSendFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in chatListUserToFlowLayoutPanelMap)
                    {
                        if (item.Key.username == $"{ContactNameConversation.Text}")
                        {
                            PictureBox pictureBox = new PictureBox();
                            pictureBox.Size = new Size(250, 250);
                            //pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox.Image = Image.FromFile(ofd.FileName);
                            Invoke(new Action(() =>
                            {
                                item.Value.Controls.Add(pictureBox);
                            }));
                            try
                            {
                                
                                string ImageDataString = ImageToString(pictureBox.Image);
                                StreamWriter writer = new StreamWriter(client.GetStream());
                                writer.AutoFlush = true;
                                writer.WriteLine("Image");
                                writer.WriteLine(username + "|" + ContactNameConversation.Text);
                                writer.WriteLine(ImageDataString);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
