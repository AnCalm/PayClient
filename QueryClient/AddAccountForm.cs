using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryClient
{
    public partial class AddAccountForm : Form
    {
        public AddAccountForm()
        {
            InitializeComponent();
        }

        private void AddAccountForm_Load(object sender, EventArgs e)
        {
            LoadChargeAccountTypeToText();
        }

        void LoadChargeAccountTypeToText()
        {
            List<ChargeAccountType> chargeAccountTypeSet = SQLChargeAccountType.GetChargeAccountType(p => p.ChargeAccountTypeID != null);
            this.CmbAccountType.DataSource = chargeAccountTypeSet;
            this.CmbAccountType.DisplayMember = "Description";
            this.CmbAccountType.ValueMember = "ChargeAccountTypeID";
        }
        bool CheckOrderChargeAccountText()
        {
            if (string.IsNullOrEmpty(this.CmbAccountType.Text))
            {
                this.CmbAccountType.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.TxtAccountName.Text))
            {
                this.TxtAccountName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.TxtAccountPwd.Text))
            {
                this.TxtAccountPwd.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.TxtPayPwd.Text))
            {
                this.TxtPayPwd.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.TxtPayValue.Text))
            {
                this.TxtPayValue.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(this.TxtBalance.Text))
            {
                this.TxtBalance.Focus();
                return false;
            }
            return true;
        }
        OrderChargeAccount AssignmentOrderChargeAccountFormText()
        {
            if (!CheckOrderChargeAccountText())
            {
                MessageBox.Show("值不能为空!");
                return null;
            }
            OrderChargeAccount orderChargeAccount = new OrderChargeAccount();

            orderChargeAccount.ChargeAccountTypeID = (int)CmbAccountType.SelectedValue;
            orderChargeAccount.ChargeAccount = TxtAccountName.Text;
            orderChargeAccount.ChargePassword = TxtAccountPwd.Text;
            orderChargeAccount.PayPassword = TxtPayPwd.Text;
            orderChargeAccount.ParValue = Convert.ToInt32(TxtPayValue.Text);
            orderChargeAccount.Balance = Convert.ToDecimal(TxtBalance.Text);
            orderChargeAccount.IsAvailable = chkLive.Checked;
            orderChargeAccount.IsUsing = false;
            orderChargeAccount.CreateTime = DateTime.Now;
            orderChargeAccount.LastUseTime = orderChargeAccount.CreateTime;

            return orderChargeAccount;
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnSaveAccount_Click(object sender, EventArgs e)
        {
            try
            {
                OrderChargeAccount orderChargeAccount = AssignmentOrderChargeAccountFormText();
                if (orderChargeAccount != null)
                {
                    if (SQLOrderChargeAccount.AddOrderChargeAccount(orderChargeAccount))
                    {
                        MessageBox.Show("添加成功");
                        this.DialogResult = DialogResult.Yes;
                    }
                    else
                    {
                        MessageBox.Show("添加失败");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("添加失败");
            }
        }
    }
}
