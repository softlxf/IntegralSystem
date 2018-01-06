using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IntegralSystem
{
    public partial class LoginForm : Form
    {
        public static string Username { get { return username; } }
        public static int UserType { get { return userType; } }

        public static string username = "";
        public static int userType = -1;

        int failCount = 0;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            List<string> users = DbHelper.Instance.GetUsers();
            foreach (string user in users)
                comboBoxUser.Items.Add(user);
            comboBoxUser.SelectedIndex = 0;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (comboBoxUser.Text == "")
            {
                MessageBox.Show("请输入登录用户名");
                return;
            }
            if (textBoxPassword.Text == "")
            {
                MessageBox.Show("请输入用户密码");
                return;
            }
            userType = DbHelper.Instance.UserLogin(comboBoxUser.Text, textBoxPassword.Text);
            if (userType < 0)
            {
                failCount++;
                MessageBox.Show("用户名或密码错误");
                if (failCount >= 3)
                {
                    DbHelper.Instance.InsertLog(DbHelper.LogType.LoginError, "用户" + comboBoxUser.Text + "3次尝试登录失败");
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                return;
            }
            username = comboBoxUser.Text;
            DbHelper.Instance.InsertLog(DbHelper.LogType.Login, "用户" + comboBoxUser.Text + "登录成功");
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
