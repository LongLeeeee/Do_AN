using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APP
{
    public partial class UserFriend : UserControl
    {
        public UserFriend()
        {
            InitializeComponent();
        }
      
        private string _username;
        private Image _image;
        [Category("custom")]
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                Name.Text = value;
            }
        }
        [Category("custom")]
        public Image userimage
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                PictureBox.Image = value;
            }
        }
    }
}
