using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;

namespace IntegralSystem
{
    public partial class MainForm : Form
    {
        private VipInfo currVip = null;
        private int totalMinutes = 0;
        enum PageName{
            Main,
            Members,
            BonusChange,
            Goods,
            Log
        }
        PageName currPage = PageName.Main;


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            panelMain.Location = new Point(12, panelMain.Location.Y);
            panelMain.Size = new Size(this.ClientSize.Width - 24, panelMain.Size.Height);
            dataGridViewMembers.Location = panelMain.Location;
            dataGridViewBonus.Location = panelMain.Location;
            dataGridViewGoodsList.Location = panelMain.Location;
            dataGridViewMembers.Size = panelMain.Size;
            dataGridViewBonus.Size = panelMain.Size;
            dataGridViewGoodsList.Size = panelMain.Size;

            buttonMainPage.BackColor = Color.DarkOrange;

            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                Close();
                return;
            }
            linkLabelUser.Text = "当前用户：" + LoginForm.Username;

            dateTimePickerBonusStart.Value = DateTime.Now.Date.AddDays(-1);
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute / 10 * 10, 0);
            dateTimePickerStart.Value = dateTime;
            dateTimePickerEnd.Value = dateTime;
            dataGridViewGoods.DataSource = DbHelper.Instance.GetGoodsList(false);

        }

        private void timerTime_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void switchPage(PageName pageName)
        {
            if (currPage == pageName)
                return;

            panelMain.Visible = false;
            dataGridViewMembers.Visible = false;
            dataGridViewBonus.Visible = false;
            dataGridViewGoodsList.Visible = false;
            panelMembers.Visible = false;
            panelBonus.Visible = false;

            buttonMainPage.BackColor = Color.FromArgb(33, 144, 163);
            buttonVipPage.BackColor = Color.FromArgb(33, 144, 163);
            buttonBonusHistoryPage.BackColor = Color.FromArgb(33, 144, 163);
            buttonGoodsPage.BackColor = Color.FromArgb(33, 144, 163);

            switch (pageName)
            {
                case PageName.Main:
                    buttonMainPage.BackColor = Color.DarkOrange;
                    panelMain.Visible = true;
                    currPage = pageName;
                    updateVipInfo();
                    break;
                case PageName.Members:
                    buttonVipPage.BackColor = Color.DarkOrange;
                    panelMembers.Visible = true;
                    dataGridViewMembers.Visible = true;
                    currPage = pageName;
                    break;
                case PageName.BonusChange:
                    buttonBonusHistoryPage.BackColor = Color.DarkOrange;
                    panelBonus.Visible = true;
                    dataGridViewBonus.Visible = true;
                    currPage = pageName;
                    break;
                case PageName.Goods:
                    buttonGoodsPage.BackColor = Color.DarkOrange;
                    dataGridViewGoodsList.Visible = true;
                    currPage = pageName;
                    break;
                case PageName.Log:
                    break;
            }
        }

        private void buttonMainPage_Click(object sender, EventArgs e)
        {
            switchPage(PageName.Main);
            dataGridViewGoods.DataSource = DbHelper.Instance.GetGoodsList(false);
            textBoxGoodsCound.Text = "";
            textBoxGoodsPrice.Text = "";
        }

        private void dataGridViewGoods_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewGoods.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }
        }


        private void buttonGoodsPage_Click(object sender, EventArgs e)
        {
            switchPage(PageName.Goods);
            dataGridViewGoodsList.DataSource = DbHelper.Instance.GetGoodsList();
        }

        private void buttonVipPage_Click(object sender, EventArgs e)
        {
            switchPage(PageName.Members);
            dataGridViewMembers.DataSource = DbHelper.Instance.GetVipList();
        }


        private void textBoxVip_TextChanged(object sender, EventArgs e)
        {
            dataGridViewMembers.DataSource = DbHelper.Instance.GetVipList(textBoxVip.Text);
            //if (textBoxVip.Text.Length > 0)
            //{
            //    var vips = DbHelper.Instance.FindVips(textBoxVip.Text);
            //    if (vips.Count > 0)
            //    {
            //        listBoxVip.Items.Clear();
            //        listBoxVip.Visible = true;
            //        listBoxVip.Size = new Size(listBoxVip.Size.Width, listBoxVip.ItemHeight * (vips.Count + 1));
            //        foreach (var vip in vips)
            //        {
            //            string text = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", vip.vipId, vip.vipName, vip.tel, vip.maxBonus, vip.bonus);
            //            listBoxVip.Items.Add(vip);
            //        }
            //    }
            //    else
            //    {
            //        listBoxVip.Visible = false;
            //    }
            //}
            //else
            //{
            //    clearVip(false);
            //    listBoxVip.Visible = false;
            //}
        }


        private void dataGridViewMembers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewMembers.Rows.Count)
                return;
            if (e.RowIndex == dataGridViewMembers.Rows.Count - 1)
            {
                EditVipForm regVipForm = new EditVipForm();
                if (regVipForm.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewMembers.DataSource = DbHelper.Instance.GetVipList();
                }
                return;
            }
            long vipId = (long)dataGridViewMembers.Rows[e.RowIndex].Cells["vipId"].Value;
            string vipName = (string)dataGridViewMembers.Rows[e.RowIndex].Cells["vipName"].Value;
            string tel = (string)dataGridViewMembers.Rows[e.RowIndex].Cells["tel"].Value;
            EditVipForm editVipForm = new EditVipForm((int)vipId, vipName, tel);
            if (editVipForm.ShowDialog() == DialogResult.OK)
            {
                dataGridViewMembers.DataSource = DbHelper.Instance.GetVipList();
            }
        }

        private void dataGridViewMembers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewMembers.Rows.Count - 1 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn column = dataGridViewMembers.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    if (column.Name == "addBonus")
                    {
                        long vipId = (long)dataGridViewMembers.Rows[e.RowIndex].Cells["vipId"].Value;
                        textBoxVipId.Text = vipId.ToString("000");
                        switchPage(PageName.Main);
                    }
                    else if (column.Name == "bonusHistory")
                    {
                        long vipId = (long)dataGridViewMembers.Rows[e.RowIndex].Cells["vipId"].Value;
                        textBoxBonusVipId.Text = vipId.ToString("000");
                        switchPage(PageName.BonusChange);
                    }
                }
            }
        }


        private void clearVip(bool all)
        {
            currVip = null;
            if (all)
                textBoxVipId.Text = "";
            textBoxVipName.Text = "";
            textBoxTel.Text = "";
            textBoxCurrBonus.Text = "";
            buttonAddBonus.Enabled = false;
            buttonConsume.Enabled = false;
        }

        private void updateMinMaxDateTime()
        {
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute / 10 * 10, 0);
            dateTimePickerStart.MinDate = dateTime.AddDays(-7);
            dateTimePickerStart.MaxDate = dateTime;
            dateTimePickerEnd.MinDate = dateTime.AddDays(-7);
            dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            dateTimePickerEnd.MaxDate = dateTime.AddHours(1);
        }

        private void dateTimePickerStart_Enter(object sender, EventArgs e)
        {
            updateMinMaxDateTime();
        }


        private void dateTimePickerEnd_Enter(object sender, EventArgs e)
        {
            updateMinMaxDateTime();
        }


        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            updateDurationAndBonus();
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            updateDurationAndBonus();
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDurationAndBonus();
        }

        private void updateDurationAndBonus()
        {
            totalMinutes = (int)(dateTimePickerEnd.Value - dateTimePickerStart.Value).TotalMinutes;
            if (totalMinutes <= 0)
            {
                textBoxDuration.Text = "";
                textBoxBonus.Text = "";
            }
            else
            {
                if (totalMinutes < 60)
                    textBoxDuration.Text = totalMinutes + "分钟";
                else if (totalMinutes % 60 == 0)
                    textBoxDuration.Text = (totalMinutes / 60) + "小时";
                else
                    textBoxDuration.Text = (totalMinutes / 60) + "小时" + (totalMinutes % 60) + "分钟";
                int halfHourCount = (totalMinutes + 15) / 30;
                if (comboBoxLevel.SelectedIndex == 0)
                {
                    textBoxBonus.Text = (halfHourCount * 4).ToString();
                }
                else if (comboBoxLevel.SelectedIndex == 1)
                {
                    textBoxBonus.Text = (halfHourCount * 2.5).ToString();
                }
                else
                {
                    textBoxBonus.Text = "";
                }
            }
        }

        private void buttonAddBonus_Click(object sender, EventArgs e)
        {
            if (currVip == null)
                return;
            if (comboBoxLevel.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择级别", "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (totalMinutes <= 0 || textBoxDuration.Text.Length == 0)
            {
                MessageBox.Show("上下桌时间不正确", "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (totalMinutes < 15)
            {
                MessageBox.Show("上下桌时长必须至少15分钟", "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (totalMinutes >= 60 * 24 * 3)
            {
                MessageBox.Show("上下桌时长不能大于3天", "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int halfHourCount = (totalMinutes + 15) / 30;
            float bonus = 0;
            if (comboBoxLevel.SelectedIndex == 0)
            {
                bonus = halfHourCount * 4;
            }
            else if (comboBoxLevel.SelectedIndex == 1)
            {
                bonus = halfHourCount * 2.5f;
            }
            else
            {
                MessageBox.Show("请先选择级别", "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string msg;
            bool result = DbHelper.Instance.AddBonus(currVip.vipId, comboBoxLevel.SelectedIndex
                , dateTimePickerStart.Value, dateTimePickerEnd.Value, totalMinutes, bonus, out msg);
            if (result)
            {
                MessageBox.Show(msg, "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Information);
                updateVipInfo();
            }
            else
            {
                MessageBox.Show(msg, "会员积分", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxBonusVipId_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBonusVipId.Text == "" || textBoxBonusVipId.Text.Length == 3)
            {
                int vipId = 0;
                int.TryParse(textBoxBonusVipId.Text, out vipId);
                dataGridViewBonus.DataSource = DbHelper.Instance.GetBonusChangeList(vipId, dateTimePickerBonusStart.Value);
            }
        }

        private void dateTimePickerBonusStart_ValueChanged(object sender, EventArgs e)
        {
            if (currPage != PageName.BonusChange)
                return;
            int vipId = 0;
            int.TryParse(textBoxBonusVipId.Text, out vipId);
            dataGridViewBonus.DataSource = DbHelper.Instance.GetBonusChangeList(vipId, dateTimePickerBonusStart.Value);
        }

        private void dataGridViewGoods_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn column = dataGridViewGoods.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    if (column.Name == "add")
                    {
                        float price = (float)(double)dataGridViewGoods.Rows[e.RowIndex].Cells["price"].Value;
                        object obj = dataGridViewGoods.Rows[e.RowIndex].Cells["count"].Value;
                        int count = (obj != null) ? (int)obj + 1 : 1;
                        dataGridViewGoods.Rows[e.RowIndex].Cells["count"].Value = count;
                        float needBonus = price * count;
                        dataGridViewGoods.Rows[e.RowIndex].Cells["needBonus"].Value = needBonus;
                        dataGridViewGoods.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PowderBlue;
                        updateConsumeGoods();
                    }
                    else if (column.Name == "remove")
                    {
                        object obj = dataGridViewGoods.Rows[e.RowIndex].Cells["count"].Value;
                        if (obj != null)
                        {
                            float price = (float)(double)dataGridViewGoods.Rows[e.RowIndex].Cells["price"].Value;
                            int count = (int)obj;
                            if (count > 1)
                            {
                                count--;
                                dataGridViewGoods.Rows[e.RowIndex].Cells["count"].Value = count;
                                float needBonus = price * count;
                                dataGridViewGoods.Rows[e.RowIndex].Cells["needBonus"].Value = needBonus;
                            }
                            else
                            {
                                dataGridViewGoods.Rows[e.RowIndex].Cells["count"].Value = null;
                                dataGridViewGoods.Rows[e.RowIndex].Cells["needBonus"].Value = null;
                                dataGridViewGoods.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Empty;
                            }
                            updateConsumeGoods();
                        }
                    }
                }
            }
        }

        private void updateConsumeGoods()
        {
            int totalCount = 0;
            float totalPrice = 0;
            foreach (DataGridViewRow row in dataGridViewGoods.Rows)
            {
                object obj = row.Cells["count"].Value;
                if (obj != null)
                {
                    int count = (int)obj;
                    totalCount += count;
                    float price = (float)(double)row.Cells["price"].Value;
                    totalPrice += price * count;
                }
            }
            textBoxGoodsCound.Text = totalCount > 0 ? totalCount.ToString() : "";
            textBoxGoodsPrice.Text = totalCount > 0 ? totalPrice.ToString() : "";
        }

        private void buttonConsume_Click(object sender, EventArgs e)
        {
            if (currVip == null)
                return;
            float needBonus = 0;
            List<GoodsInfo> listGoodsInfo = new List<GoodsInfo>();
            foreach (DataGridViewRow row in dataGridViewGoods.Rows)
            {
                if (row.Cells["count"].Value != null)
                {
                    GoodsInfo goodsInfo = new GoodsInfo((int)(long)row.Cells["goodsId"].Value, (string)row.Cells["name"].Value, (float)(double)row.Cells["price"].Value, (int)row.Cells["count"].Value);
                    if (goodsInfo.price < 0 || goodsInfo.count < 1)
                        continue;
                    needBonus += goodsInfo.price * goodsInfo.count;
                    listGoodsInfo.Add(goodsInfo);
                }
            }
            if (listGoodsInfo.Count == 0)
            {
                MessageBox.Show("请先增加需要兑换的商品", "积分兑换", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (needBonus > currVip.bonus)
            {
                MessageBox.Show("你选择的兑换商品所需积分超出会员当前所拥有的总积分", "积分兑换", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int duration = DbHelper.Instance.IsLastBonusChange(currVip.vipId, listGoodsInfo, needBonus);
            if (duration >= 0 && MessageBox.Show(duration == 0 ? "你刚刚兑换过此商品，确认再次兑换吗？" : "你" + duration + "分钟前兑换过此商品，确认再次兑换吗？"
                , "兑换积分", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            string msg;
            bool result = DbHelper.Instance.ConsumeBonus(currVip.vipId, listGoodsInfo, needBonus, out msg);
            if (result)
            {
                MessageBox.Show(msg, "兑换积分成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                updateVipInfo();
            }
            else
            {
                MessageBox.Show(msg, "兑换积分失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxVipId_TextChanged(object sender, EventArgs e)
        {
            updateVipInfo();
        }

        private void updateVipInfo()
        {
            if (textBoxVipId.Text.Length < 3)
            {
                clearVip(false);
                return;
            }
            else if (textBoxVipId.Text.Length == 3)
            {
                int vipId;
                if (int.TryParse(textBoxVipId.Text, out vipId) && vipId > 0)
                {
                    currVip = DbHelper.Instance.GetVipInfo(vipId);
                    if (currVip != null)
                    {
                        //textBoxVipId.Text = currVip.vipId.ToString();
                        textBoxVipName.Text = currVip.vipName;
                        textBoxTel.Text = currVip.tel;
                        textBoxCurrBonus.Text = currVip.bonus.ToString();
                        buttonAddBonus.Enabled = true;
                        buttonConsume.Enabled = true;
                        return;
                    }
                }
            }
            else if (textBoxVipId.Text.Length > 3)
            {
                //currVip = DbHelper.Instance.GetVipInfoByTel(textBoxVip.Text);
                //if (currVip != null)
                //{
                //    textBoxVipId.Text = currVip.vipId.ToString("");
                //    textBoxVipName.Text = currVip.vipName;
                //    textBoxTel.Text = currVip.tel;
                //    textBoxCurrBonus.Text = currVip.bonus.ToString();
                //    buttonAddBonus.Enabled = true;
                //    buttonConsume.Enabled = true;
                //    return;
                //}

                //var vips = DbHelper.Instance.FindVips(textBoxVip.Text);
                //if (vips.Count > 0)
                //{
                //    listBoxVip.Items.Clear();
                //    listBoxVip.Visible = true;
                //    listBoxVip.Size = new Size(listBoxVip.Size.Width, listBoxVip.ItemHeight * (vips.Count + 1));
                //    foreach (var vip in vips)
                //    {
                //        string text = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", vip.vipId, vip.vipName, vip.tel, vip.maxBonus, vip.bonus);
                //        listBoxVip.Items.Add(vip);
                //    }
                //}
                //else
                //{
                //    listBoxVip.Visible = false;
                //}
            }
            clearVip(false);

        }

        private void dataGridViewGoodsList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridViewGoodsList.Rows.Count)
                return;
            if (e.RowIndex == dataGridViewGoodsList.Rows.Count - 1)
            {
                EditGoodsForm addGoodsForm = new EditGoodsForm();
                if (addGoodsForm.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewGoodsList.DataSource = DbHelper.Instance.GetGoodsList();
                }
                return;
            }
            long id = (long)dataGridViewGoodsList.Rows[e.RowIndex].Cells["dataId"].Value;
            string name = (string)dataGridViewGoodsList.Rows[e.RowIndex].Cells["goodsName"].Value;
            float price = (float)(double)dataGridViewGoodsList.Rows[e.RowIndex].Cells["goodsPrice"].Value;
            long soldCount = (long)dataGridViewGoodsList.Rows[e.RowIndex].Cells["soldCount"].Value;
            EditGoodsForm editGoodsForm = new EditGoodsForm((int)id, name, price, (int)soldCount);
            if (editGoodsForm.ShowDialog() == DialogResult.OK)
            {
                dataGridViewGoodsList.DataSource = DbHelper.Instance.GetGoodsList();
            }
        }

        private void dataGridViewGoodsList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewGoodsList.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }
        }


        private void buttonBonusHistoryPage_Click(object sender, EventArgs e)
        {
            switchPage(PageName.BonusChange);
            int vipId = 0;
            int.TryParse(textBoxBonusVipId.Text, out vipId);
            dataGridViewBonus.DataSource = DbHelper.Instance.GetBonusChangeList(vipId, dateTimePickerBonusStart.Value);
        }

        private void dataGridViewBonus_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn column = dataGridViewBonus.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    if (column.Name == "revoke")
                    {
                        DataGridViewRow row = dataGridViewBonus.Rows[e.RowIndex];
                        int status = (int)(long)row.Cells["status"].Value;
                        if (status != (int)DbHelper.BonusChangeStatus.Normal)
                        {
                            if (status == (int)DbHelper.BonusChangeStatus.RevokeMark)
                                MessageBox.Show("此记录已被撤销过，不能重复撤销", "交易记录", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("此记录是撤销记录，不能再次撤销", "交易记录", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        DateTime createTime = (DateTime)row.Cells["bonusCreateTime"].Value;
                        if ((DateTime.Now - createTime).TotalDays > 3)
                        {
                            MessageBox.Show("此记录已超过3天，不支持撤销", "交易记录", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string typeText = row.Cells["typeText"].Value.ToString();
                        if (MessageBox.Show("你确认要撤销此" + typeText + "记录吗？", "交易记录", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                            return;
                        long id = (long)row.Cells["bonusId"].Value;
                        int type = (int)(long)row.Cells["type"].Value;
                        int vipId = (int)(long)row.Cells["bonusVipId"].Value;
                        int duration = row.Cells["duration"].Value == DBNull.Value ? 0 : (int)(long)row.Cells["duration"].Value;
                        float changeBonus = (float)(double)row.Cells["changeBonus"].Value;
                        string consume = row.Cells["consume"].Value is string ? (string)row.Cells["consume"].Value : "";
                        string msg;
                        if (DbHelper.Instance.RevokeBonus(id, type, typeText, status, vipId, changeBonus, duration, consume, out msg))
                        {
                            int.TryParse(textBoxBonusVipId.Text, out vipId);
                            dataGridViewBonus.DataSource = DbHelper.Instance.GetBonusChangeList(vipId, dateTimePickerBonusStart.Value);
                            //MessageBox.Show(msg, "交易记录", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("撤销失败，" + msg, "交易记录", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void linkLabelUser_Click(object sender, EventArgs e)
        {

        }

        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditUserForm editUserForm = new EditUserForm(LoginForm.UserType, LoginForm.Username);
            editUserForm.ShowDialog();
        }

    }
}
