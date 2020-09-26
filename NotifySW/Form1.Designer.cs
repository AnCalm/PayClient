namespace NotifyToMerchant
{
    partial class FrmNotify
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
            this.tabNotify = new System.Windows.Forms.TabControl();
            this.tabOrder = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.dgOrderCharge = new System.Windows.Forms.DataGridView();
            this.tabProduct = new System.Windows.Forms.TabPage();
            this.pannelCheckbox = new System.Windows.Forms.Panel();
            this.tabNotify.SuspendLayout();
            this.tabOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderCharge)).BeginInit();
            this.tabProduct.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabNotify
            // 
            this.tabNotify.Controls.Add(this.tabOrder);
            this.tabNotify.Controls.Add(this.tabProduct);
            this.tabNotify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabNotify.Location = new System.Drawing.Point(0, 0);
            this.tabNotify.Name = "tabNotify";
            this.tabNotify.SelectedIndex = 0;
            this.tabNotify.Size = new System.Drawing.Size(1098, 471);
            this.tabNotify.TabIndex = 0;
            // 
            // tabOrder
            // 
            this.tabOrder.Controls.Add(this.splitContainer1);
            this.tabOrder.Location = new System.Drawing.Point(4, 22);
            this.tabOrder.Name = "tabOrder";
            this.tabOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabOrder.Size = new System.Drawing.Size(1090, 445);
            this.tabOrder.TabIndex = 0;
            this.tabOrder.Text = "订单查询";
            this.tabOrder.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel1.Controls.Add(this.btnOK);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgOrderCharge);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 439);
            this.splitContainer1.SplitterDistance = 96;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(140, 27);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 45);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "停止";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(22, 27);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 45);
            this.btnOK.TabIndex = 2;
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
            this.dgOrderCharge.Size = new System.Drawing.Size(1084, 339);
            this.dgOrderCharge.TabIndex = 0;
            // 
            // tabProduct
            // 
            this.tabProduct.Controls.Add(this.pannelCheckbox);
            this.tabProduct.Location = new System.Drawing.Point(4, 22);
            this.tabProduct.Name = "tabProduct";
            this.tabProduct.Padding = new System.Windows.Forms.Padding(3);
            this.tabProduct.Size = new System.Drawing.Size(923, 443);
            this.tabProduct.TabIndex = 1;
            this.tabProduct.Text = "订单查询商品配置";
            this.tabProduct.UseVisualStyleBackColor = true;
            // 
            // pannelCheckbox
            // 
            this.pannelCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pannelCheckbox.Location = new System.Drawing.Point(3, 3);
            this.pannelCheckbox.Name = "pannelCheckbox";
            this.pannelCheckbox.Size = new System.Drawing.Size(917, 437);
            this.pannelCheckbox.TabIndex = 0;
            // 
            // FrmNotify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 471);
            this.Controls.Add(this.tabNotify);
            this.Name = "FrmNotify";
            this.Text = "订单状态返回挂机";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNotify_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmNotify_FormClosed);
            this.Load += new System.EventHandler(this.FrmNotify_Load);
            this.tabNotify.ResumeLayout(false);
            this.tabOrder.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOrderCharge)).EndInit();
            this.tabProduct.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabNotify;
        private System.Windows.Forms.TabPage tabOrder;
        private System.Windows.Forms.TabPage tabProduct;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgOrderCharge;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pannelCheckbox;
    }
}

