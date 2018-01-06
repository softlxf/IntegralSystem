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
    public partial class EditVipForm : Form
    {
        private int vipId;
        private string vipName;
        private string tel;

        public EditVipForm()
        {
            InitializeComponent();
            vipId = DbHelper.Instance.GetMaxVipId() + 1;
            textBoxVipId.Text = vipId.ToString();

            buttonReg.Visible = true;
            AcceptButton = buttonReg;
            buttonSave.Visible = false;
            buttonDelete.Visible = false;
            Text = "会员注册";
        }

        public EditVipForm(int vipId, string vipName, string tel)
        {
            InitializeComponent();
            this.vipId = vipId;
            this.vipName = vipName;
            this.tel = tel;
            textBoxVipId.Text = vipId.ToString();
            textBoxVipName.Text = vipName;
            textBoxTel.Text = tel;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.vipName == textBoxVipName.Text.Trim() && this.tel == textBoxTel.Text.Trim())
            {
                Close();
                return;
            }
            string vipName = textBoxVipName.Text.Trim();
            string vipTel = textBoxTel.Text.Trim();
            if (vipName.Length < 1)
            {
                MessageBox.Show("会员名称不能为空");
                return;
            }
            if (vipTel.Length > 0)
            {
                if (!(Regex.IsMatch(vipTel, @"^\+?1\d{10}$") || Regex.IsMatch(vipTel, @"^\+?\d{7,12}$") || Regex.IsMatch(vipTel, @"^\+?\d{3,4}-\d{7,8}$")))
                {
                    MessageBox.Show("电话号码格式错误");
                    return;
                }
            }

            if (!DbHelper.Instance.UpdateVip(vipId, vipName, vipTel))
            {
                MessageBox.Show("更改会员信息失败");
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.MemberUpdate, string.Format("更改会员信息：{0},{1},{2}", vipId, vipName, vipTel));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确认要删除此会员吗？", "会员更改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            int vipId = int.Parse(textBoxVipId.Text);
            if (!DbHelper.Instance.DeleteVip(vipId))
            {
                MessageBox.Show("删除会员失败");
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.MemberDelete, string.Format("删除会员：{0}", vipId));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonReg_Click(object sender, EventArgs e)
        {
            string vipName = textBoxVipName.Text.Trim();
            string vipTel = textBoxTel.Text.Trim();
            if (vipName.Length == 0 && vipTel.Length == 0)
            {
                MessageBox.Show("会员名称和电话号码不能同时为空", "会员注册", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (vipTel.Length > 0)
            {
                if (!(Regex.IsMatch(vipTel, @"^1\d{10}$") || Regex.IsMatch(vipTel, @"^0\d{10,12}$") || Regex.IsMatch(vipTel, @"^\d{7,8}$")))
                {
                    MessageBox.Show("电话号码长度或格式错误");
                    return;
                }
            }
            if (!DbHelper.Instance.InsertVip(vipId, vipName, vipTel))
            {
                MessageBox.Show("注册会员失败，可能已存在相同名称和电话号码的会员", "会员注册", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.MemberNew, string.Format("注册会员信息：{0},{1},{2}", vipId, vipName, vipTel));
            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
