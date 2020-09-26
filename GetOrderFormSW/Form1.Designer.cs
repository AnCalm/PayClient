namespace GetOrder
{
    partial class FrmGetOrders
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGetOrders = new System.Windows.Forms.TabPage();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.btnStopSW = new System.Windows.Forms.Button();
            this.btnStopSUP = new System.Windows.Forms.Button();
            this.btnStartAll = new System.Windows.Forms.Button();
            this.btnStartSW = new System.Windows.Forms.Button();
            this.btnStartSUP = new System.Windows.Forms.Button();
            this.tabSW = new System.Windows.Forms.TabPage();
            this.txtSWDescription = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSWGetOrderTime = new System.Windows.Forms.TextBox();
            this.txtSWGetOrderCount = new System.Windows.Forms.TextBox();
            this.txtSWNotifyUrl = new System.Windows.Forms.TextBox();
            this.txtSWGetOrderUrl = new System.Windows.Forms.TextBox();
            this.txtSWKey = new System.Windows.Forms.TextBox();
            this.txtSWUserCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveSW = new System.Windows.Forms.Button();
            this.tabSUP = new System.Windows.Forms.TabPage();
            this.txtSUPDescription = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSUPGetOrderTime = new System.Windows.Forms.TextBox();
            this.txtSUPGetOrderCount = new System.Windows.Forms.TextBox();
            this.txtSUPNotifyUrl = new System.Windows.Forms.TextBox();
            this.txtSUPGetOrderUrl = new System.Windows.Forms.TextBox();
            this.txtSUPKey = new System.Windows.Forms.TextBox();
            this.txtSUPUserCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSaveSUP = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabGetOrders.SuspendLayout();
            this.tabSW.SuspendLayout();
            this.tabSUP.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGetOrders);
            this.tabControl1.Controls.Add(this.tabSW);
            this.tabControl1.Controls.Add(this.tabSUP);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(469, 344);
            this.tabControl1.TabIndex = 0;
            // 
            // tabGetOrders
            // 
            this.tabGetOrders.Controls.Add(this.btnStopAll);
            this.tabGetOrders.Controls.Add(this.btnStopSW);
            this.tabGetOrders.Controls.Add(this.btnStopSUP);
            this.tabGetOrders.Controls.Add(this.btnStartAll);
            this.tabGetOrders.Controls.Add(this.btnStartSW);
            this.tabGetOrders.Controls.Add(this.btnStartSUP);
            this.tabGetOrders.Location = new System.Drawing.Point(4, 22);
            this.tabGetOrders.Name = "tabGetOrders";
            this.tabGetOrders.Padding = new System.Windows.Forms.Padding(3);
            this.tabGetOrders.Size = new System.Drawing.Size(461, 318);
            this.tabGetOrders.TabIndex = 0;
            this.tabGetOrders.Text = "获取订单";
            this.tabGetOrders.UseVisualStyleBackColor = true;
            // 
            // btnStopAll
            // 
            this.btnStopAll.Location = new System.Drawing.Point(237, 193);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(100, 43);
            this.btnStopAll.TabIndex = 5;
            this.btnStopAll.Text = "全部停止";
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
            // 
            // btnStopSW
            // 
            this.btnStopSW.Location = new System.Drawing.Point(237, 123);
            this.btnStopSW.Name = "btnStopSW";
            this.btnStopSW.Size = new System.Drawing.Size(100, 43);
            this.btnStopSW.TabIndex = 4;
            this.btnStopSW.Text = "停止取数网订单";
            this.btnStopSW.UseVisualStyleBackColor = true;
            this.btnStopSW.Click += new System.EventHandler(this.btnStopSW_Click);
            // 
            // btnStopSUP
            // 
            this.btnStopSUP.Location = new System.Drawing.Point(237, 55);
            this.btnStopSUP.Name = "btnStopSUP";
            this.btnStopSUP.Size = new System.Drawing.Size(100, 43);
            this.btnStopSUP.TabIndex = 3;
            this.btnStopSUP.Text = "停止取易约订单";
            this.btnStopSUP.UseVisualStyleBackColor = true;
            this.btnStopSUP.Click += new System.EventHandler(this.btnStopSUP_Click);
            // 
            // btnStartAll
            // 
            this.btnStartAll.Location = new System.Drawing.Point(96, 193);
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(100, 43);
            this.btnStartAll.TabIndex = 2;
            this.btnStartAll.Text = "全部开始";
            this.btnStartAll.UseVisualStyleBackColor = true;
            this.btnStartAll.Click += new System.EventHandler(this.btnStartAll_Click);
            // 
            // btnStartSW
            // 
            this.btnStartSW.Location = new System.Drawing.Point(96, 123);
            this.btnStartSW.Name = "btnStartSW";
            this.btnStartSW.Size = new System.Drawing.Size(100, 43);
            this.btnStartSW.TabIndex = 1;
            this.btnStartSW.Text = "获取数网订单";
            this.btnStartSW.UseVisualStyleBackColor = true;
            this.btnStartSW.Click += new System.EventHandler(this.btnStartSW_Click);
            // 
            // btnStartSUP
            // 
            this.btnStartSUP.Location = new System.Drawing.Point(96, 55);
            this.btnStartSUP.Name = "btnStartSUP";
            this.btnStartSUP.Size = new System.Drawing.Size(100, 43);
            this.btnStartSUP.TabIndex = 0;
            this.btnStartSUP.Text = "获取易约订单";
            this.btnStartSUP.UseVisualStyleBackColor = true;
            this.btnStartSUP.Click += new System.EventHandler(this.btnStartSUP_Click);
            // 
            // tabSW
            // 
            this.tabSW.Controls.Add(this.txtSWDescription);
            this.tabSW.Controls.Add(this.label13);
            this.tabSW.Controls.Add(this.txtSWGetOrderTime);
            this.tabSW.Controls.Add(this.txtSWGetOrderCount);
            this.tabSW.Controls.Add(this.txtSWNotifyUrl);
            this.tabSW.Controls.Add(this.txtSWGetOrderUrl);
            this.tabSW.Controls.Add(this.txtSWKey);
            this.tabSW.Controls.Add(this.txtSWUserCode);
            this.tabSW.Controls.Add(this.label6);
            this.tabSW.Controls.Add(this.label5);
            this.tabSW.Controls.Add(this.label4);
            this.tabSW.Controls.Add(this.label3);
            this.tabSW.Controls.Add(this.label2);
            this.tabSW.Controls.Add(this.label1);
            this.tabSW.Controls.Add(this.btnSaveSW);
            this.tabSW.Location = new System.Drawing.Point(4, 22);
            this.tabSW.Name = "tabSW";
            this.tabSW.Padding = new System.Windows.Forms.Padding(3);
            this.tabSW.Size = new System.Drawing.Size(461, 318);
            this.tabSW.TabIndex = 1;
            this.tabSW.Text = "数网配置";
            this.tabSW.UseVisualStyleBackColor = true;
            // 
            // txtSWDescription
            // 
            this.txtSWDescription.Location = new System.Drawing.Point(176, 204);
            this.txtSWDescription.Name = "txtSWDescription";
            this.txtSWDescription.Size = new System.Drawing.Size(245, 21);
            this.txtSWDescription.TabIndex = 15;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(38, 207);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "数网描述信息：";
            // 
            // txtSWGetOrderTime
            // 
            this.txtSWGetOrderTime.Location = new System.Drawing.Point(176, 177);
            this.txtSWGetOrderTime.Name = "txtSWGetOrderTime";
            this.txtSWGetOrderTime.Size = new System.Drawing.Size(245, 21);
            this.txtSWGetOrderTime.TabIndex = 13;
            // 
            // txtSWGetOrderCount
            // 
            this.txtSWGetOrderCount.Location = new System.Drawing.Point(176, 150);
            this.txtSWGetOrderCount.Name = "txtSWGetOrderCount";
            this.txtSWGetOrderCount.Size = new System.Drawing.Size(245, 21);
            this.txtSWGetOrderCount.TabIndex = 12;
            // 
            // txtSWNotifyUrl
            // 
            this.txtSWNotifyUrl.Location = new System.Drawing.Point(176, 123);
            this.txtSWNotifyUrl.Name = "txtSWNotifyUrl";
            this.txtSWNotifyUrl.Size = new System.Drawing.Size(245, 21);
            this.txtSWNotifyUrl.TabIndex = 11;
            // 
            // txtSWGetOrderUrl
            // 
            this.txtSWGetOrderUrl.Location = new System.Drawing.Point(176, 93);
            this.txtSWGetOrderUrl.Name = "txtSWGetOrderUrl";
            this.txtSWGetOrderUrl.Size = new System.Drawing.Size(245, 21);
            this.txtSWGetOrderUrl.TabIndex = 10;
            // 
            // txtSWKey
            // 
            this.txtSWKey.Location = new System.Drawing.Point(176, 66);
            this.txtSWKey.Name = "txtSWKey";
            this.txtSWKey.PasswordChar = '*';
            this.txtSWKey.Size = new System.Drawing.Size(245, 21);
            this.txtSWKey.TabIndex = 9;
            // 
            // txtSWUserCode
            // 
            this.txtSWUserCode.Location = new System.Drawing.Point(176, 39);
            this.txtSWUserCode.Name = "txtSWUserCode";
            this.txtSWUserCode.Size = new System.Drawing.Size(245, 21);
            this.txtSWUserCode.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "数网取单频率（秒）：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "数网每次取单数量：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "数网密钥：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "数网商户编号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "数网通知地址：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "数网获取订单地址：";
            // 
            // btnSaveSW
            // 
            this.btnSaveSW.Location = new System.Drawing.Point(176, 264);
            this.btnSaveSW.Name = "btnSaveSW";
            this.btnSaveSW.Size = new System.Drawing.Size(111, 44);
            this.btnSaveSW.TabIndex = 0;
            this.btnSaveSW.Text = "保存";
            this.btnSaveSW.UseVisualStyleBackColor = true;
            this.btnSaveSW.Click += new System.EventHandler(this.btnSaveSW_Click);
            // 
            // tabSUP
            // 
            this.tabSUP.Controls.Add(this.txtSUPDescription);
            this.tabSUP.Controls.Add(this.label14);
            this.tabSUP.Controls.Add(this.txtSUPGetOrderTime);
            this.tabSUP.Controls.Add(this.txtSUPGetOrderCount);
            this.tabSUP.Controls.Add(this.txtSUPNotifyUrl);
            this.tabSUP.Controls.Add(this.txtSUPGetOrderUrl);
            this.tabSUP.Controls.Add(this.txtSUPKey);
            this.tabSUP.Controls.Add(this.txtSUPUserCode);
            this.tabSUP.Controls.Add(this.label7);
            this.tabSUP.Controls.Add(this.label8);
            this.tabSUP.Controls.Add(this.label9);
            this.tabSUP.Controls.Add(this.label10);
            this.tabSUP.Controls.Add(this.label11);
            this.tabSUP.Controls.Add(this.label12);
            this.tabSUP.Controls.Add(this.btnSaveSUP);
            this.tabSUP.Location = new System.Drawing.Point(4, 22);
            this.tabSUP.Name = "tabSUP";
            this.tabSUP.Size = new System.Drawing.Size(461, 318);
            this.tabSUP.TabIndex = 2;
            this.tabSUP.Text = "易约配置";
            this.tabSUP.UseVisualStyleBackColor = true;
            // 
            // txtSUPDescription
            // 
            this.txtSUPDescription.Location = new System.Drawing.Point(177, 213);
            this.txtSUPDescription.Name = "txtSUPDescription";
            this.txtSUPDescription.Size = new System.Drawing.Size(245, 21);
            this.txtSUPDescription.TabIndex = 28;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(39, 218);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 12);
            this.label14.TabIndex = 27;
            this.label14.Text = "易约描述信息：";
            // 
            // txtSUPGetOrderTime
            // 
            this.txtSUPGetOrderTime.Location = new System.Drawing.Point(177, 186);
            this.txtSUPGetOrderTime.Name = "txtSUPGetOrderTime";
            this.txtSUPGetOrderTime.Size = new System.Drawing.Size(245, 21);
            this.txtSUPGetOrderTime.TabIndex = 26;
            // 
            // txtSUPGetOrderCount
            // 
            this.txtSUPGetOrderCount.Location = new System.Drawing.Point(177, 159);
            this.txtSUPGetOrderCount.Name = "txtSUPGetOrderCount";
            this.txtSUPGetOrderCount.Size = new System.Drawing.Size(245, 21);
            this.txtSUPGetOrderCount.TabIndex = 25;
            // 
            // txtSUPNotifyUrl
            // 
            this.txtSUPNotifyUrl.Location = new System.Drawing.Point(177, 132);
            this.txtSUPNotifyUrl.Name = "txtSUPNotifyUrl";
            this.txtSUPNotifyUrl.Size = new System.Drawing.Size(245, 21);
            this.txtSUPNotifyUrl.TabIndex = 24;
            // 
            // txtSUPGetOrderUrl
            // 
            this.txtSUPGetOrderUrl.Location = new System.Drawing.Point(177, 102);
            this.txtSUPGetOrderUrl.Name = "txtSUPGetOrderUrl";
            this.txtSUPGetOrderUrl.Size = new System.Drawing.Size(245, 21);
            this.txtSUPGetOrderUrl.TabIndex = 23;
            // 
            // txtSUPKey
            // 
            this.txtSUPKey.Location = new System.Drawing.Point(177, 75);
            this.txtSUPKey.Name = "txtSUPKey";
            this.txtSUPKey.PasswordChar = '*';
            this.txtSUPKey.Size = new System.Drawing.Size(245, 21);
            this.txtSUPKey.TabIndex = 22;
            // 
            // txtSUPUserCode
            // 
            this.txtSUPUserCode.Location = new System.Drawing.Point(177, 43);
            this.txtSUPUserCode.Name = "txtSUPUserCode";
            this.txtSUPUserCode.Size = new System.Drawing.Size(245, 21);
            this.txtSUPUserCode.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(125, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "易约取单频率（秒）：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 162);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "易约每次取单数量：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "易约密钥：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(39, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "易约商户编号：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 135);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 16;
            this.label11.Text = "易约通知地址：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(39, 105);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(113, 12);
            this.label12.TabIndex = 15;
            this.label12.Text = "易约获取订单地址：";
            // 
            // btnSaveSUP
            // 
            this.btnSaveSUP.Location = new System.Drawing.Point(167, 255);
            this.btnSaveSUP.Name = "btnSaveSUP";
            this.btnSaveSUP.Size = new System.Drawing.Size(111, 44);
            this.btnSaveSUP.TabIndex = 14;
            this.btnSaveSUP.Text = "保存";
            this.btnSaveSUP.UseVisualStyleBackColor = true;
            this.btnSaveSUP.Click += new System.EventHandler(this.btnSaveSUP_Click);
            // 
            // FrmGetOrders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 344);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmGetOrders";
            this.Text = "取单挂机";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmGetOrders_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmGetOrders_FormClosed);
            this.Load += new System.EventHandler(this.FrmGetOrders_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabGetOrders.ResumeLayout(false);
            this.tabSW.ResumeLayout(false);
            this.tabSW.PerformLayout();
            this.tabSUP.ResumeLayout(false);
            this.tabSUP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGetOrders;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.Button btnStopSW;
        private System.Windows.Forms.Button btnStopSUP;
        private System.Windows.Forms.Button btnStartAll;
        private System.Windows.Forms.Button btnStartSW;
        private System.Windows.Forms.Button btnStartSUP;
        private System.Windows.Forms.TabPage tabSW;
        private System.Windows.Forms.TabPage tabSUP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveSW;
        private System.Windows.Forms.TextBox txtSWGetOrderTime;
        private System.Windows.Forms.TextBox txtSWGetOrderCount;
        private System.Windows.Forms.TextBox txtSWNotifyUrl;
        private System.Windows.Forms.TextBox txtSWGetOrderUrl;
        private System.Windows.Forms.TextBox txtSWKey;
        private System.Windows.Forms.TextBox txtSWUserCode;
        private System.Windows.Forms.TextBox txtSUPGetOrderTime;
        private System.Windows.Forms.TextBox txtSUPNotifyUrl;
        private System.Windows.Forms.TextBox txtSUPGetOrderUrl;
        private System.Windows.Forms.TextBox txtSUPKey;
        private System.Windows.Forms.TextBox txtSUPUserCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnSaveSUP;
        private System.Windows.Forms.TextBox txtSUPGetOrderCount;
        private System.Windows.Forms.TextBox txtSWDescription;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtSUPDescription;
        private System.Windows.Forms.Label label14;

    }
}

