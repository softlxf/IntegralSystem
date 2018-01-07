namespace IntegralSystem
{
    partial class EditVipForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxVipId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxVipName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTel = new System.Windows.Forms.TextBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonReg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "会员号";
            // 
            // textBoxVipId
            // 
            this.textBoxVipId.Location = new System.Drawing.Point(77, 21);
            this.textBoxVipId.MaxLength = 3;
            this.textBoxVipId.Name = "textBoxVipId";
            this.textBoxVipId.ReadOnly = true;
            this.textBoxVipId.Size = new System.Drawing.Size(160, 21);
            this.textBoxVipId.TabIndex = 1;
            this.textBoxVipId.TabStop = false;
            this.textBoxVipId.Text = " ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "会员名称";
            // 
            // textBoxVipName
            // 
            this.textBoxVipName.Location = new System.Drawing.Point(77, 54);
            this.textBoxVipName.MaxLength = 20;
            this.textBoxVipName.Name = "textBoxVipName";
            this.textBoxVipName.Size = new System.Drawing.Size(160, 21);
            this.textBoxVipName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "电话号码";
            // 
            // textBoxTel
            // 
            this.textBoxTel.Location = new System.Drawing.Point(77, 87);
            this.textBoxTel.MaxLength = 15;
            this.textBoxTel.Name = "textBoxTel";
            this.textBoxTel.Size = new System.Drawing.Size(160, 21);
            this.textBoxTel.TabIndex = 3;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(173, 124);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(89, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "删除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Visible = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(89, 124);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(89, 23);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonReg
            // 
            this.buttonReg.Location = new System.Drawing.Point(89, 124);
            this.buttonReg.Name = "buttonReg";
            this.buttonReg.Size = new System.Drawing.Size(89, 23);
            this.buttonReg.TabIndex = 6;
            this.buttonReg.Text = "注册";
            this.buttonReg.UseVisualStyleBackColor = true;
            this.buttonReg.Visible = false;
            this.buttonReg.Click += new System.EventHandler(this.buttonReg_Click);
            // 
            // EditVipForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 171);
            this.Controls.Add(this.buttonReg);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.textBoxTel);
            this.Controls.Add(this.textBoxVipName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxVipId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EditVipForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "会员更改";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxVipId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxVipName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTel;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonReg;
    }
}