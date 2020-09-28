namespace Client
{
    partial class MainForm
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
            this.tabContainer = new System.Windows.Forms.TabControl();
            this.OrderCharge = new System.Windows.Forms.TabPage();
            this.spOrderCharge = new System.Windows.Forms.SplitContainer();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dgOrderCharge = new System.Windows.Forms.DataGridView();
            this.OrderQuery = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgOrderQuery = new System.Windows.Forms.DataGridView();
            this.ChargeClassPanel = new System.Windows.Forms.Panel();
            this.Setting = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSaveSW = new System.Windows.Forms.Button();
            this.txtSWDescription = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSWGetOrderTime = new System.Windows.Forms.TextBox();
            this.txtSWGetOrderCount = new System.Windows.Forms.TextBox();
            this.txtSWNotifyUrl = new System.Windows.Forms.TextBox();
            this.txtSWGetOrderUrl = new System.Windows.Forms.TextBox();
            this.txtSWKey = new System.Windows.Forms.TextBox();
            this.txtSWUserCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tabContainer.SuspendLayout();
            this.OrderCharge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spOrderCharge)).BeginInit();
            this.spOrderCharge.Panel1.SuspendLayout();
            this.spOrderCharge.Panel2.SuspendLayout();
            this.spOrderCharge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderCharge)).BeginInit();
            this.OrderQuery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderQuery)).BeginInit();
            this.Setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabContainer
            // 
            this.tabContainer.Controls.Add(this.OrderCharge);
            this.tabContainer.Controls.Add(this.OrderQuery);
            this.tabContainer.Controls.Add(this.Setting);
            this.tabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContainer.Location = new System.Drawing.Point(0, 0);
            this.tabContainer.Margin = new System.Windows.Forms.Padding(4);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(1541, 753);
            this.tabContainer.TabIndex = 0;
            // 
            // OrderCharge
            // 
            this.OrderCharge.Controls.Add(this.spOrderCharge);
            this.OrderCharge.Location = new System.Drawing.Point(4, 25);
            this.OrderCharge.Margin = new System.Windows.Forms.Padding(4);
            this.OrderCharge.Name = "OrderCharge";
            this.OrderCharge.Padding = new System.Windows.Forms.Padding(4);
            this.OrderCharge.Size = new System.Drawing.Size(1533, 724);
            this.OrderCharge.TabIndex = 0;
            this.OrderCharge.Text = "订单充值";
            this.OrderCharge.UseVisualStyleBackColor = true;
            // 
            // spOrderCharge
            // 
            this.spOrderCharge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spOrderCharge.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spOrderCharge.Location = new System.Drawing.Point(4, 4);
            this.spOrderCharge.Margin = new System.Windows.Forms.Padding(4);
            this.spOrderCharge.Name = "spOrderCharge";
            this.spOrderCharge.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spOrderCharge.Panel1
            // 
            this.spOrderCharge.Panel1.Controls.Add(this.label6);
            this.spOrderCharge.Panel1.Controls.Add(this.label5);
            this.spOrderCharge.Panel1.Controls.Add(this.label4);
            this.spOrderCharge.Panel1.Controls.Add(this.label3);
            this.spOrderCharge.Panel1.Controls.Add(this.label2);
            this.spOrderCharge.Panel1.Controls.Add(this.label1);
            this.spOrderCharge.Panel1.Controls.Add(this.btnCancel);
            this.spOrderCharge.Panel1.Controls.Add(this.btnOK);
            // 
            // spOrderCharge.Panel2
            // 
            this.spOrderCharge.Panel2.Controls.Add(this.dgOrderCharge);
            this.spOrderCharge.Size = new System.Drawing.Size(1525, 716);
            this.spOrderCharge.SplitterDistance = 101;
            this.spOrderCharge.SplitterWidth = 5;
            this.spOrderCharge.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(953, 81);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(953, 51);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(953, 24);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(795, 81);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "存疑的订单数量：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(780, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "未处理的订单数量：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(780, 51);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "充值中的订单数量：";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(199, 31);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(133, 56);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "停止";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(41, 31);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(133, 56);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "开始";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dgOrderCharge
            // 
            this.dgOrderCharge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOrderCharge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgOrderCharge.Location = new System.Drawing.Point(0, 0);
            this.dgOrderCharge.Margin = new System.Windows.Forms.Padding(4);
            this.dgOrderCharge.Name = "dgOrderCharge";
            this.dgOrderCharge.RowHeadersWidth = 51;
            this.dgOrderCharge.RowTemplate.Height = 23;
            this.dgOrderCharge.Size = new System.Drawing.Size(1525, 610);
            this.dgOrderCharge.TabIndex = 0;
            // 
            // OrderQuery
            // 
            this.OrderQuery.Controls.Add(this.splitContainer2);
            this.OrderQuery.Location = new System.Drawing.Point(4, 25);
            this.OrderQuery.Name = "OrderQuery";
            this.OrderQuery.Size = new System.Drawing.Size(1533, 724);
            this.OrderQuery.TabIndex = 1;
            this.OrderQuery.Text = "订单查询";
            this.OrderQuery.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgOrderQuery);
            this.splitContainer2.Size = new System.Drawing.Size(1533, 724);
            this.splitContainer2.SplitterDistance = 155;
            this.splitContainer2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ChargeClassPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1529, 151);
            this.panel1.TabIndex = 0;
            // 
            // dgOrderQuery
            // 
            this.dgOrderQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOrderQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgOrderQuery.Location = new System.Drawing.Point(0, 0);
            this.dgOrderQuery.Name = "dgOrderQuery";
            this.dgOrderQuery.RowHeadersWidth = 51;
            this.dgOrderQuery.Size = new System.Drawing.Size(1529, 561);
            this.dgOrderQuery.TabIndex = 0;
            // 
            // ChargeClassPanel
            // 
            this.ChargeClassPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChargeClassPanel.Location = new System.Drawing.Point(0, 0);
            this.ChargeClassPanel.Name = "ChargeClassPanel";
            this.ChargeClassPanel.Size = new System.Drawing.Size(1529, 151);
            this.ChargeClassPanel.TabIndex = 0;
            // 
            // Setting
            // 
            this.Setting.Controls.Add(this.splitContainer1);
            this.Setting.Location = new System.Drawing.Point(4, 25);
            this.Setting.Name = "Setting";
            this.Setting.Size = new System.Drawing.Size(1533, 724);
            this.Setting.TabIndex = 2;
            this.Setting.Text = "设置";
            this.Setting.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSaveSW);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWDescription);
            this.splitContainer1.Panel1.Controls.Add(this.label13);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWGetOrderTime);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWGetOrderCount);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWNotifyUrl);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWGetOrderUrl);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWKey);
            this.splitContainer1.Panel1.Controls.Add(this.txtSWUserCode);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Size = new System.Drawing.Size(1533, 724);
            this.splitContainer1.SplitterDistance = 177;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnSaveSW
            // 
            this.btnSaveSW.Location = new System.Drawing.Point(1267, 41);
            this.btnSaveSW.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveSW.Name = "btnSaveSW";
            this.btnSaveSW.Size = new System.Drawing.Size(148, 55);
            this.btnSaveSW.TabIndex = 30;
            this.btnSaveSW.Text = "保存";
            this.btnSaveSW.UseVisualStyleBackColor = true;
            this.btnSaveSW.Click += new System.EventHandler(this.btnSaveSW_Click);
            // 
            // txtSWDescription
            // 
            this.txtSWDescription.Location = new System.Drawing.Point(815, 91);
            this.txtSWDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWDescription.Name = "txtSWDescription";
            this.txtSWDescription.Size = new System.Drawing.Size(325, 25);
            this.txtSWDescription.TabIndex = 29;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(631, 95);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(112, 15);
            this.label13.TabIndex = 28;
            this.label13.Text = "数网描述信息：";
            // 
            // txtSWGetOrderTime
            // 
            this.txtSWGetOrderTime.Location = new System.Drawing.Point(815, 57);
            this.txtSWGetOrderTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWGetOrderTime.Name = "txtSWGetOrderTime";
            this.txtSWGetOrderTime.Size = new System.Drawing.Size(325, 25);
            this.txtSWGetOrderTime.TabIndex = 27;
            // 
            // txtSWGetOrderCount
            // 
            this.txtSWGetOrderCount.Location = new System.Drawing.Point(815, 24);
            this.txtSWGetOrderCount.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWGetOrderCount.Name = "txtSWGetOrderCount";
            this.txtSWGetOrderCount.Size = new System.Drawing.Size(325, 25);
            this.txtSWGetOrderCount.TabIndex = 26;
            // 
            // txtSWNotifyUrl
            // 
            this.txtSWNotifyUrl.Location = new System.Drawing.Point(202, 134);
            this.txtSWNotifyUrl.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWNotifyUrl.Name = "txtSWNotifyUrl";
            this.txtSWNotifyUrl.Size = new System.Drawing.Size(325, 25);
            this.txtSWNotifyUrl.TabIndex = 25;
            // 
            // txtSWGetOrderUrl
            // 
            this.txtSWGetOrderUrl.Location = new System.Drawing.Point(202, 96);
            this.txtSWGetOrderUrl.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWGetOrderUrl.Name = "txtSWGetOrderUrl";
            this.txtSWGetOrderUrl.Size = new System.Drawing.Size(325, 25);
            this.txtSWGetOrderUrl.TabIndex = 24;
            // 
            // txtSWKey
            // 
            this.txtSWKey.Location = new System.Drawing.Point(202, 57);
            this.txtSWKey.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWKey.Name = "txtSWKey";
            this.txtSWKey.PasswordChar = '*';
            this.txtSWKey.Size = new System.Drawing.Size(325, 25);
            this.txtSWKey.TabIndex = 23;
            // 
            // txtSWUserCode
            // 
            this.txtSWUserCode.Location = new System.Drawing.Point(202, 24);
            this.txtSWUserCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtSWUserCode.Name = "txtSWUserCode";
            this.txtSWUserCode.Size = new System.Drawing.Size(325, 25);
            this.txtSWUserCode.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(631, 61);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(157, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "数网取单频率（秒）：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(631, 27);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "数网每次取单数量：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 61);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "数网密钥：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 27);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 15);
            this.label10.TabIndex = 18;
            this.label10.Text = "数网商户编号：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 138);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 15);
            this.label11.TabIndex = 17;
            this.label11.Text = "数网通知地址：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 100);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(142, 15);
            this.label12.TabIndex = 16;
            this.label12.Text = "数网获取订单地址：";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1541, 753);
            this.Controls.Add(this.tabContainer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "充值挂机 2.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabContainer.ResumeLayout(false);
            this.OrderCharge.ResumeLayout(false);
            this.spOrderCharge.Panel1.ResumeLayout(false);
            this.spOrderCharge.Panel1.PerformLayout();
            this.spOrderCharge.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spOrderCharge)).EndInit();
            this.spOrderCharge.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderCharge)).EndInit();
            this.OrderQuery.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderQuery)).EndInit();
            this.Setting.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabContainer;
        private System.Windows.Forms.TabPage OrderCharge;
        private System.Windows.Forms.SplitContainer spOrderCharge;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridView dgOrderCharge;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage OrderQuery;
        private System.Windows.Forms.TabPage Setting;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtSWDescription;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtSWGetOrderTime;
        private System.Windows.Forms.TextBox txtSWGetOrderCount;
        private System.Windows.Forms.TextBox txtSWNotifyUrl;
        private System.Windows.Forms.TextBox txtSWGetOrderUrl;
        private System.Windows.Forms.TextBox txtSWKey;
        private System.Windows.Forms.TextBox txtSWUserCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dgOrderQuery;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveSW;
        private System.Windows.Forms.Panel ChargeClassPanel;
    }
}

