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
            this.tabContainer.SuspendLayout();
            this.OrderCharge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spOrderCharge)).BeginInit();
            this.spOrderCharge.Panel1.SuspendLayout();
            this.spOrderCharge.Panel2.SuspendLayout();
            this.spOrderCharge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderCharge)).BeginInit();
            this.SuspendLayout();
            // 
            // tabContainer
            // 
            this.tabContainer.Controls.Add(this.OrderCharge);
            this.tabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContainer.Location = new System.Drawing.Point(0, 0);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(1043, 466);
            this.tabContainer.TabIndex = 0;
            // 
            // OrderCharge
            // 
            this.OrderCharge.Controls.Add(this.spOrderCharge);
            this.OrderCharge.Location = new System.Drawing.Point(4, 22);
            this.OrderCharge.Name = "OrderCharge";
            this.OrderCharge.Padding = new System.Windows.Forms.Padding(3);
            this.OrderCharge.Size = new System.Drawing.Size(1035, 440);
            this.OrderCharge.TabIndex = 0;
            this.OrderCharge.Text = "订单充值";
            this.OrderCharge.UseVisualStyleBackColor = true;
            // 
            // spOrderCharge
            // 
            this.spOrderCharge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spOrderCharge.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spOrderCharge.Location = new System.Drawing.Point(3, 3);
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
            this.spOrderCharge.Size = new System.Drawing.Size(1029, 434);
            this.spOrderCharge.SplitterDistance = 101;
            this.spOrderCharge.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(715, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(715, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(715, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(596, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "存疑的订单数量：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(585, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "未处理的订单数量：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(585, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "充值中的订单数量：";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(149, 25);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 45);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "停止";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(31, 25);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 45);
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
            this.dgOrderCharge.Name = "dgOrderCharge";
            this.dgOrderCharge.RowTemplate.Height = 23;
            this.dgOrderCharge.Size = new System.Drawing.Size(1029, 329);
            this.dgOrderCharge.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 466);
            this.Controls.Add(this.tabContainer);
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
    }
}

