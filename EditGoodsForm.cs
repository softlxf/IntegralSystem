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
    public partial class EditGoodsForm : Form
    {
        int goodsId = 0;
        int soldCount = 0;
        string name = "";
        float price = 0;

        public EditGoodsForm()
        {
            InitializeComponent();
            
            buttonAdd.Visible = true;
            AcceptButton = buttonAdd;
            buttonSave.Visible = false;
            buttonDelete.Visible = false;
            Text = "添加商品";
        }

        public EditGoodsForm(int id, string name, float price, int soldCount)
        {
            InitializeComponent();

            this.goodsId = id;
            this.name = name;
            this.price = price;
            this.soldCount = soldCount;
            textBoxName.Text = name;
            textBoxPrice.Text = price.ToString();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Trim() == "")
            {
                MessageBox.Show("商品名称不能为空", "添加商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            float price = 0;
            if (!float.TryParse(textBoxPrice.Text, out price) || price < 0.001)
            {
                MessageBox.Show("商品价格输入不正确", "添加商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!DbHelper.Instance.AddGoods(textBoxName.Text.Trim(), price))
            {
                MessageBox.Show("商品添加失败，可能已存在相同名称商品", "添加商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.GoodsNew, string.Format("新建商品信息：{0},{1},{2}", -1, textBoxName.Text.Trim(), price));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text.Trim() == "")
            {
                MessageBox.Show("商品名称不能为空", "修改商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            float price = 0;
            if (!float.TryParse(textBoxPrice.Text, out price) || price < 0.001)
            {
                MessageBox.Show("商品价格输入不正确", "修改商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.price.ToString() == textBoxPrice.Text && this.name == textBoxName.Text)
            {
                Close();
                return;
            }
            if (!DbHelper.Instance.UpdateGoods(goodsId, textBoxName.Text.Trim(), price))
            {
                MessageBox.Show("商品修改失败，可能已存在相同名称商品", "修改商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.GoodsUpdate, string.Format("更改商品信息：{0},{1},{2}", goodsId, textBoxName.Text.Trim(), price));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string msg = soldCount > 0 ? string.Format("商品【{0}】已兑换过{1}次，你确定要删除吗？", textBoxName.Text, soldCount) : string.Format("你确定要删除商品【{0}】吗？", textBoxName.Text);
            if (MessageBox.Show(msg, "删除商品", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            if (!DbHelper.Instance.DeleteGoods(goodsId))
            {
                MessageBox.Show("删除商品【" + textBoxName.Text + "】失败", "删除商品", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DbHelper.Instance.InsertLog(DbHelper.LogType.GoodsUpdate, string.Format("删除商品：{0},{1}", goodsId, textBoxName.Text.Trim()));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
