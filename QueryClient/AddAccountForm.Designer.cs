namespace QueryClient
{
    partial class AddAccountForm
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
            this.ChkNotLive = new System.Windows.Forms.CheckBox();
            this.chkLive = new System.Windows.Forms.CheckBox();
            this.TxtBalance = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.TxtAccountPwd = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.TxtAccountName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.TxtPayValue = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.TxtPayPwd = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.CmbAccountType = new System.Windows.Forms.ComboBox();
            this.btnSaveAccount = new System.Windows.Forms.Button();
            this.btnDeleteAccount = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ChkNotLive
            // 
            this.ChkNotLive.AutoSize = true;
            this.ChkNotLive.Location = new System.Drawing.Point(166, 197);
            this.ChkNotLive.Name = "ChkNotLive";
            this.ChkNotLive.Size = new System.Drawing.Size(48, 16);
            this.ChkNotLive.TabIndex = 36;
            this.ChkNotLive.Text = "停用";
            this.ChkNotLive.UseVisualStyleBackColor = true;
            // 
            // chkLive
            // 
            this.chkLive.AutoSize = true;
            this.chkLive.Checked = true;
            this.chkLive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLive.Location = new System.Drawing.Point(103, 198);
            this.chkLive.Name = "chkLive";
            this.chkLive.Size = new System.Drawing.Size(48, 16);
            this.chkLive.TabIndex = 35;
            this.chkLive.Text = "启用";
            this.chkLive.UseVisualStyleBackColor = true;
            // 
            // TxtBalance
            // 
            this.TxtBalance.Location = new System.Drawing.Point(103, 164);
            this.TxtBalance.Name = "TxtBalance";
            this.TxtBalance.Size = new System.Drawing.Size(190, 21);
            this.TxtBalance.TabIndex = 34;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(51, 168);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 12);
            this.label20.TabIndex = 33;
            this.label20.Text = "余额：";
            // 
            // TxtAccountPwd
            // 
            this.TxtAccountPwd.Location = new System.Drawing.Point(103, 82);
            this.TxtAccountPwd.Name = "TxtAccountPwd";
            this.TxtAccountPwd.Size = new System.Drawing.Size(190, 21);
            this.TxtAccountPwd.TabIndex = 32;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(51, 84);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(41, 12);
            this.label19.TabIndex = 31;
            this.label19.Text = "密码：";
            // 
            // TxtAccountName
            // 
            this.TxtAccountName.Location = new System.Drawing.Point(103, 51);
            this.TxtAccountName.Name = "TxtAccountName";
            this.TxtAccountName.Size = new System.Drawing.Size(190, 21);
            this.TxtAccountName.TabIndex = 30;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(51, 56);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(41, 12);
            this.label18.TabIndex = 29;
            this.label18.Text = "帐号：";
            // 
            // TxtPayValue
            // 
            this.TxtPayValue.Location = new System.Drawing.Point(103, 138);
            this.TxtPayValue.Name = "TxtPayValue";
            this.TxtPayValue.Size = new System.Drawing.Size(190, 21);
            this.TxtPayValue.TabIndex = 28;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(51, 140);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 27;
            this.label17.Text = "面值：";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(27, 196);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 26;
            this.label16.Text = "是否启用：";
            // 
            // TxtPayPwd
            // 
            this.TxtPayPwd.Location = new System.Drawing.Point(103, 111);
            this.TxtPayPwd.Name = "TxtPayPwd";
            this.TxtPayPwd.Size = new System.Drawing.Size(190, 21);
            this.TxtPayPwd.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(27, 112);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 24;
            this.label15.Text = "支付密码：";
            // 
            // CmbAccountType
            // 
            this.CmbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbAccountType.FormattingEnabled = true;
            this.CmbAccountType.Location = new System.Drawing.Point(103, 25);
            this.CmbAccountType.Name = "CmbAccountType";
            this.CmbAccountType.Size = new System.Drawing.Size(121, 20);
            this.CmbAccountType.TabIndex = 23;
            // 
            // btnSaveAccount
            // 
            this.btnSaveAccount.Location = new System.Drawing.Point(33, 249);
            this.btnSaveAccount.Name = "btnSaveAccount";
            this.btnSaveAccount.Size = new System.Drawing.Size(115, 41);
            this.btnSaveAccount.TabIndex = 20;
            this.btnSaveAccount.Text = "保存";
            this.btnSaveAccount.UseVisualStyleBackColor = true;
            this.btnSaveAccount.Click += new System.EventHandler(this.btnSaveAccount_Click);
            // 
            // btnDeleteAccount
            // 
            this.btnDeleteAccount.Location = new System.Drawing.Point(178, 249);
            this.btnDeleteAccount.Name = "btnDeleteAccount";
            this.btnDeleteAccount.Size = new System.Drawing.Size(115, 41);
            this.btnDeleteAccount.TabIndex = 22;
            this.btnDeleteAccount.Text = "删除";
            this.btnDeleteAccount.UseVisualStyleBackColor = true;
            this.btnDeleteAccount.Click += new System.EventHandler(this.btnDeleteAccount_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(27, 28);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 21;
            this.label14.Text = "帐号类型：";
            // 
            // AddAccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 331);
            this.Controls.Add(this.ChkNotLive);
            this.Controls.Add(this.chkLive);
            this.Controls.Add(this.TxtBalance);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.TxtAccountPwd);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.TxtAccountName);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.TxtPayValue);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.TxtPayPwd);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.CmbAccountType);
            this.Controls.Add(this.btnSaveAccount);
            this.Controls.Add(this.btnDeleteAccount);
            this.Controls.Add(this.label14);
            this.MaximizeBox = false;
            this.Name = "AddAccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新增帐号";
            this.Load += new System.EventHandler(this.AddAccountForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ChkNotLive;
        private System.Windows.Forms.CheckBox chkLive;
        private System.Windows.Forms.TextBox TxtBalance;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox TxtAccountPwd;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox TxtAccountName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TxtPayValue;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox TxtPayPwd;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox CmbAccountType;
        private System.Windows.Forms.Button btnSaveAccount;
        private System.Windows.Forms.Button btnDeleteAccount;
        private System.Windows.Forms.Label label14;
    }
}