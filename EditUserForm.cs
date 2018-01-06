using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace IntegralSystem
{
    public partial class EditUserForm : Form
    {
        private int type;

        public EditUserForm()
        {
            InitializeComponent();
        }

        public EditUserForm(int type, string userName)
        {
            InitializeComponent();
            this.type = type;
            if (type == 0)
            {
                List<string> users = DbHelper.Instance.GetUsers();
                foreach (string user in users)
                    comboBoxUser.Items.Add(user);
                comboBoxUser.Text = userName;
                comboBoxUser.Enabled = true;
            }
            else
            {
                comboBoxUser.Items.Add(userName);
                comboBoxUser.SelectedIndex = 0;
                comboBoxUser.Enabled = false;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.Text == "")
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            if (textBoxPassword.Text != textBoxPassword2.Text)
            {
                MessageBox.Show("两次密码输入不一致");
                return;
            }
            if (!DbHelper.Instance.UpdateUser(comboBoxUser.Text, type, textBoxPassword.Text))
            {
                MessageBox.Show("更改用户密码失败");
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.UserUpdate, string.Format("更改用户信息：{0},{1}", type, comboBoxUser.Text));
            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
