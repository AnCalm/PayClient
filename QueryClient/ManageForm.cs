using Common;
using DBOperation.SQLHelper;
using EntityDB;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryClient
{
    public partial class ManageForm : Form
    {
        Sunisoft.IrisSkin.SkinEngine se = null;
        public ManageForm()
        {
            InitializeComponent();

            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;//所有窗体均应用此皮肤
            se.SkinFile = AppDomain.CurrentDomain.BaseDirectory + "\\skin\\MSN.ssk";
        }
        private void ManageForm_Load(object sender, EventArgs e)
        {
            BindingQueryAccountDataGridColumns();
            BindingQueryCardsDataGridColumns();
            BindingQueryOrderDataGridColumns();
            BindingCardStoreDataGridColumns();

            this.cmbOrderStatus.SelectedIndex = 0;
            dtOrderStartTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            dtOrderEndTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");
        }
        void LoadType(System.Windows.Forms.ComboBox cmb,bool isCard=false , bool isQuery = false)
        {
            List<ChargeAccountType> chargeAccountTypeSet = SQLChargeAccountType.GetChargeAccountType(p =>p.IsCard == isCard);
            if (isQuery)
            {
                ChargeAccountType type = new ChargeAccountType()
                {
                    Description = "全部",
                    ChargeAccountTypeID = 000000
                };
                chargeAccountTypeSet.Add(type);
            }
            cmb.DataSource = chargeAccountTypeSet;
            cmb.DisplayMember = "Description";
            cmb.ValueMember = "ChargeAccountTypeID";
        }
        public List<RechargeStatusSet> SetValues()
        {
            List<RechargeStatusSet> valuses = new List<RechargeStatusSet>();
            RechargeStatusSet untreatedStatus = new RechargeStatusSet
            {
                RechargeStatus = (int)OrderRechargeStatus.untreated,
                description = "未处理"
            };
            RechargeStatusSet processingStatus = new RechargeStatusSet
            {
                RechargeStatus = (int)OrderRechargeStatus.processing,
                description = "处理中"
            };
            RechargeStatusSet successfulStatus = new RechargeStatusSet
            {
                RechargeStatus = (int)OrderRechargeStatus.successful,
                description = "充值成功"
            };
            RechargeStatusSet failureStatus = new RechargeStatusSet
            {
                RechargeStatus = (int)OrderRechargeStatus.failure,
                description = "充值失败"
            };
            RechargeStatusSet suspiciousStatus = new RechargeStatusSet
            {
                RechargeStatus = (int)OrderRechargeStatus.suspicious,
                description = "充值存疑"
            };
            valuses.Add(untreatedStatus);
            valuses.Add(processingStatus);
            valuses.Add(successfulStatus);
            valuses.Add(failureStatus);
            valuses.Add(suspiciousStatus);
            return valuses;
        }

        #region QueryAccount
        #region QuertyAccount Event
        private void btnQueryAccount_Click(object sender, EventArgs e)
        {
            QueryChargeAccount();
        }
        private void btnAdded_Click(object sender, EventArgs e)
        {
            AddAccountForm form = new AddAccountForm();
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.Yes)
            {
                QueryChargeAccount();
            }
        }
        private void btnClearAccount_Click(object sender, EventArgs e)
        {
            cmbAccountType.SelectedIndex = cmbAccountType.Items.Count - 1;
            txtAccountName.Text = "";
        }
        private void dgvQueryAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isRefresh = false;
            if (dgvQueryAccount.Columns[e.ColumnIndex].Name == "Update")
            {
                if (UpdateAccount(dgvQueryAccount.Rows[e.RowIndex]))
                {
                    MessageBox.Show("保存成功");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("保存失败");
            }
            if (dgvQueryAccount.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (DeleteAccount(dgvQueryAccount.Rows[e.RowIndex]))
                {
                    MessageBox.Show("删除成功");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("删除失败");
            }

            if (isRefresh)
            {
                QueryChargeAccount();
            }
        }
        #endregion

        #region QueryAccount Data
        void QueryChargeAccount()
        {
            int typeId = (int)cmbAccountType.SelectedValue;
            string accountName = txtAccountName.Text.Trim();

            bool? IsAvailable = null;

            switch (cmbQueryAccountStatus.Text)
            {
                case "启用":
                    IsAvailable = true;
                    break;
                case "停用":
                    IsAvailable = false;
                    break;
                case "全部":
                default:
                    break;
            }

            List<OrderChargeAccount> accountSet = new List<OrderChargeAccount>();

            if (typeId == 000000) //查询所有类型
            {
                if (IsAvailable == null)
                    accountSet = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.ChargeAccount.Contains(accountName));
                else
                    accountSet = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.ChargeAccount.Contains(accountName) && p.IsAvailable == IsAvailable);
            }
            else
            {
                if (IsAvailable == null)
                    accountSet = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.ChargeAccount.Contains(accountName) && p.ChargeAccountTypeID == typeId);
                else
                    accountSet = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.ChargeAccount.Contains(accountName) && p.IsAvailable == IsAvailable && p.ChargeAccountTypeID == typeId);

            }

            BindingSource bs = new BindingSource();
            bs.DataSource = accountSet;
            dgvQueryAccount.DataSource = bs;
            bdnQueryAccount.BindingSource = bs;
        }
        bool UpdateAccount(DataGridViewRow row)
        {
            try
            {
                int OrderChargeAccountID = Convert.ToInt32(row.Cells["OrderChargeAccountID"].Value);

                OrderChargeAccount account = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.OrderChargeAccountID == OrderChargeAccountID).FirstOrDefault();

                account.ChargeAccountTypeID = Convert.ToInt32(row.Cells["ChargeAccountTypeID"].Value);
                account.ChargeAccount = Convert.ToString(row.Cells["ChargeAccount"].Value);
                account.ChargePassword = Convert.ToString(row.Cells["ChargePassword"].Value);
                account.PayPassword = Convert.ToString(row.Cells["PayPassword"].Value);
                account.Balance = Convert.ToDecimal(row.Cells["Balance"].Value);
                account.IsAvailable = Convert.ToBoolean(row.Cells["IsAvailable"].Value);

                return SQLOrderChargeAccount.UpdateOrderChargeAccount(account);
            }
            catch (Exception)
            {
            }
            return false;
        }
        bool DeleteAccount(DataGridViewRow row)
        {
            try
            {
                int OrderChargeAccountID = Convert.ToInt32(row.Cells["OrderChargeAccountID"].Value);

                OrderChargeAccount account = SQLOrderChargeAccount.GetOrderChargeAccount(p => p.OrderChargeAccountID == OrderChargeAccountID).FirstOrDefault();

                return SQLOrderChargeAccount.DeleteOrderChargeAccount(account);
            }
            catch (Exception)
            {
            }
            return false;
        }
        #endregion

        #region QueryAccount View
        void BindingQueryAccountDataGridColumns()
        {
            dgvQueryAccount.AllowUserToAddRows = false;
            dgvQueryAccount.AllowUserToDeleteRows = false;
            dgvQueryAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQueryAccount.AutoGenerateColumns = false;  //关闭自动产生列

            DataGridViewColumn cOrderChargeAccountID = new DataGridViewTextBoxColumn()
            {
                Name = "OrderChargeAccountID",
                DataPropertyName = "OrderChargeAccountID",
                HeaderText = "帐号编号",
                Width = 100,
                ReadOnly = true
            };

            DataGridViewComboBoxColumn cmbChargeAccountTypeID = new DataGridViewComboBoxColumn()
            {
                Name = "ChargeAccountTypeID",
                DataPropertyName = "ChargeAccountTypeID",
                HeaderText = "类型名称",
                Width = 100,
            };
            List<ChargeAccountType> chargeAccountTypeSet = SQLChargeAccountType.GetChargeAccountType(p => p.ChargeAccountTypeID != null);
            cmbChargeAccountTypeID.DataSource = chargeAccountTypeSet;
            cmbChargeAccountTypeID.DisplayMember = "Description";
            cmbChargeAccountTypeID.ValueMember = "ChargeAccountTypeID";


            DataGridViewColumn cChargeAccount = new DataGridViewTextBoxColumn()
            {
                Name = "ChargeAccount",
                DataPropertyName = "ChargeAccount",
                HeaderText = "帐号名字",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn cChargePassword = new DataGridViewTextBoxColumn()
            {
                Name = "ChargePassword",
                DataPropertyName = "ChargePassword",
                HeaderText = "登录密码",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn cPayPassword = new DataGridViewTextBoxColumn()
            {
                Name = "PayPassword",
                DataPropertyName = "PayPassword",
                HeaderText = "支付密码",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn cBalance = new DataGridViewTextBoxColumn()
            {
                Name = "Balance",
                DataPropertyName = "Balance",
                HeaderText = "帐号余额",
                Width = 100,
                Frozen = false

            };

            DataGridViewColumn cUseTimes = new DataGridViewTextBoxColumn()
            {
                Name = "UseTimes",
                DataPropertyName = "UseTimes",
                HeaderText = "使用次数",
                Width = 100,
                ReadOnly = true
            };

            DataGridViewCheckBoxColumn cIsAvailable = new DataGridViewCheckBoxColumn()
            {
                Name = "IsAvailable",
                DataPropertyName = "IsAvailable",
                HeaderText = "是否启用",
                Width = 100,
                Frozen = false
            };


            DataGridViewColumn cCreateTime = new DataGridViewTextBoxColumn()
            {
                Name = "CreateTime",
                DataPropertyName = "CreateTime",
                HeaderText = "导入时间",
                Width = 100,
                Frozen = false,
                ReadOnly = true
            }
            ;

            DataGridViewColumn cLastUseTime = new DataGridViewTextBoxColumn()
            {
                Name = "LastUseTime",
                DataPropertyName = "LastUseTime",
                HeaderText = "最后使用时间",
                Width = 100,
                Frozen = false,
                ReadOnly = true
            };

            DataGridViewButtonColumn bUpdate = new DataGridViewButtonColumn()
            {
                Name = "Update",
                HeaderText = "操作",
                Text = "保存",
                UseColumnTextForButtonValue = true
            };
            DataGridViewButtonColumn bDelete = new DataGridViewButtonColumn()
            {
                Name = "Delete",
                HeaderText = "操作",
                Text = "删除",
                UseColumnTextForButtonValue = true
            };


            dgvQueryAccount.Columns.Add(cOrderChargeAccountID);
            dgvQueryAccount.Columns.Add(cmbChargeAccountTypeID);
            dgvQueryAccount.Columns.Add(cChargeAccount);
            dgvQueryAccount.Columns.Add(cChargePassword);
            dgvQueryAccount.Columns.Add(cPayPassword);
            dgvQueryAccount.Columns.Add(cBalance);
            dgvQueryAccount.Columns.Add(cUseTimes);
            dgvQueryAccount.Columns.Add(cIsAvailable);
            dgvQueryAccount.Columns.Add(cCreateTime);
            dgvQueryAccount.Columns.Add(cLastUseTime);
            dgvQueryAccount.Columns.Add(bUpdate);
            dgvQueryAccount.Columns.Add(bDelete);

        }

        #endregion
        #endregion

        #region AddCards
        #region Add Cards Event
        private void btnSavaCards_Click(object sender, EventArgs e)
        {
            if (!CheckAddCards())
                return;

            if (InsertCards())
            {
                MessageBox.Show("保存成功");
            }
        }
        private void btnClearCards_Click(object sender, EventArgs e)
        {
            this.txtAddCards.Text = "";
            this.cmbAddCardsType.SelectedIndex = 0;
            this.txtCardValue.Text = "";
            this.txtFailCards.Text = "";
            this.chkYes.Checked = true;
        }
        #endregion

        #region Add Cards Data

        bool CheckAddCards()
        {
            if (string.IsNullOrEmpty(txtAddCards.Text))
            {
                MessageBox.Show("卡密为空!");
                return false;
            }
            else if (string.IsNullOrEmpty(txtCardValue.Text))
            {
                MessageBox.Show("卡面值为空!");
                return false;
            }
            else if (string.IsNullOrEmpty(cmbAddCardsType.Text))
            {
                MessageBox.Show("卡类型为空!");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool InsertCards()
        {
            try
            {
                string cards = this.txtAddCards.Text.Trim();
                string[] cardsArr = Regex.Split(cards, "\r\n", RegexOptions.IgnoreCase);

                foreach (string item in cardsArr)
                {
                    string[] cardsSet = null;
                    if (item.Contains("，") || item.Contains(","))
                        cardsSet = item.Split(new char[] { '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        cardsSet = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (cardsSet == null || cardsSet.Length < 2)
                    {
                        this.txtFailCards.Text += item + "\r\n";
                        continue;
                    }

                    Cards card = new Cards()
                    {
                        ChargeAccountTypeID = Convert.ToInt32(this.cmbAddCardsType.SelectedValue),
                        CardNumber = cardsSet[0],
                        CardPassWord = cardsSet[1],
                        Price = Convert.ToDecimal(this.txtCardValue.Text),
                        ReChargeStatus = (int)OrderRechargeStatus.untreated,
                        IsAvailable = chkYes.Checked,
                        CreatTime = DateTime.Now
                    };

                    if (!SQLCards.AddCards(card))
                        this.txtFailCards.Text += item + "\r\n";
                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #endregion
        #endregion

        #region QueryCards
        #region QueryCards Event
        private void btnClearQueryCards_Click(object sender, EventArgs e)
        {
            cmbQueryCardType.SelectedIndex = 0;
            cmbQueryCardsUseStatus.SelectedIndex = 0;
            txtQueryCardNo.Text = "";
            dtStartTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            dtEndTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");
        }

        private void chkUseTime_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseTime.Checked)
                chkAddTime.Checked = false;
            else
                chkAddTime.Checked = true;
        }

        private void chkAddTime_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAddTime.Checked)
                chkUseTime.Checked = false;
            else
                chkUseTime.Checked = true;
        }

        private void btnQueryCards_Click(object sender, EventArgs e)
        {
            QueryCards();
        }

        private void dgvQueryCards_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isRefresh = false;
            if (dgvQueryCards.Columns[e.ColumnIndex].Name == "UpdateCards")
            {
                if (UpdateCard(dgvQueryCards.Rows[e.RowIndex]))
                {
                    MessageBox.Show("保存成功");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("保存失败");
            }
            if (dgvQueryCards.Columns[e.ColumnIndex].Name == "DeleteCards")
            {
                if (DeleteCard(dgvQueryCards.Rows[e.RowIndex]))
                {
                    MessageBox.Show("删除成功");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("删除失败");
            }

            if (isRefresh)
            {
                QueryCards();
            }
        }
        #endregion

        #region QueryCards View
        void BindingQueryCardsDataGridColumns()
        {
            dgvQueryCards.AllowUserToAddRows = false;
            dgvQueryCards.AllowUserToDeleteRows = false;
            dgvQueryCards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQueryCards.AutoGenerateColumns = false;  //关闭自动产生列

            DataGridViewColumn cCardsID = new DataGridViewTextBoxColumn()
            {
                Name = "CardsID",
                DataPropertyName = "CardsID",
                HeaderText = "卡密编号",
                Width = 100,
                ReadOnly = true
            };

            DataGridViewComboBoxColumn cmbChargeAccountTypeID = new DataGridViewComboBoxColumn()
            {
                Name = "ChargeAccountTypeID",
                DataPropertyName = "ChargeAccountTypeID",
                HeaderText = "类型名称",
                Width = 100,
                ReadOnly = true
            };

            List<ChargeAccountType> chargeAccountTypeSet = SQLChargeAccountType.GetChargeAccountType(p => p.ChargeAccountTypeID != null);
            cmbChargeAccountTypeID.DataSource = chargeAccountTypeSet;
            cmbChargeAccountTypeID.DisplayMember = "Description";
            cmbChargeAccountTypeID.ValueMember = "ChargeAccountTypeID";


            DataGridViewColumn cCardNumber = new DataGridViewTextBoxColumn()
            {
                Name = "CardNumber",
                DataPropertyName = "CardNumber",
                HeaderText = "卡号",
                Width = 100
            };

            DataGridViewColumn cCardPassWord = new DataGridViewTextBoxColumn()
            {
                Name = "CardPassWord",
                DataPropertyName = "CardPassWord",
                HeaderText = "密码",
                Width = 100
            };


            DataGridViewColumn cPrice = new DataGridViewTextBoxColumn()
            {
                Name = "Price",
                DataPropertyName = "Price",
                HeaderText = "卡密面值",
                Width = 100

            };

            DataGridViewComboBoxColumn cmbRechargeStatus = new DataGridViewComboBoxColumn
            {
                Name = "RechargeStatus",
                DataPropertyName = "RechargeStatus",
                HeaderText = "充值状态"
            };
            List<RechargeStatusSet> ss = SetValues();
            cmbRechargeStatus.DataSource = ss;
            cmbRechargeStatus.DisplayMember = "description";
            cmbRechargeStatus.ValueMember = "RechargeStatus";



            DataGridViewColumn cReChargeMsg = new DataGridViewTextBoxColumn()
            {
                Name = "ReChargeMsg",
                DataPropertyName = "ReChargeMsg",
                HeaderText = "充值描述",
                Width = 100,
                ReadOnly = true
            };


            DataGridViewCheckBoxColumn cIsAvailable = new DataGridViewCheckBoxColumn()
            {
                Name = "IsAvailable",
                DataPropertyName = "IsAvailable",
                HeaderText = "是否可用",
                Width = 100
            };


            DataGridViewColumn cCreatTime = new DataGridViewTextBoxColumn()
            {
                Name = "CreatTime",
                DataPropertyName = "CreatTime",
                HeaderText = "导入时间",
                Width = 100,
                ReadOnly = true
            }
            ;

            DataGridViewColumn cUseTime = new DataGridViewTextBoxColumn()
            {
                Name = "UseTime",
                DataPropertyName = "UseTime",
                HeaderText = "使用时间",
                Width = 100,
                ReadOnly = true
            };

            DataGridViewButtonColumn bUpdateCards = new DataGridViewButtonColumn()
            {
                Name = "UpdateCards",
                HeaderText = "操作",
                Text = "保存",
                UseColumnTextForButtonValue = true
            };
            DataGridViewButtonColumn bDeleteCards = new DataGridViewButtonColumn()
            {
                Name = "DeleteCards",
                HeaderText = "操作",
                Text = "删除",
                UseColumnTextForButtonValue = true
            };

            dgvQueryCards.Columns.Add(cCardsID);
            dgvQueryCards.Columns.Add(cmbChargeAccountTypeID);
            dgvQueryCards.Columns.Add(cCardNumber);
            dgvQueryCards.Columns.Add(cCardPassWord);
            dgvQueryCards.Columns.Add(cPrice);
            dgvQueryCards.Columns.Add(cmbRechargeStatus);
            dgvQueryCards.Columns.Add(cReChargeMsg);
            dgvQueryCards.Columns.Add(cIsAvailable);
            dgvQueryCards.Columns.Add(cCreatTime);
            dgvQueryCards.Columns.Add(cUseTime);
            dgvQueryCards.Columns.Add(bUpdateCards);
            dgvQueryCards.Columns.Add(bDeleteCards);
        }

        #endregion

        #region QueryCards Data

        void QueryCards()
        {
            List<Cards> CardSet = new List<Cards>();
            try
            {
                DateTime startTime = dtStartTime.Value;
                DateTime endTime = dtEndTime.Value;
                string cardNo = txtQueryCardNo.Text;
                bool isUse = false;
                int type = -1;
                int status = -1;

                if (chkUseTime.Checked)
                    isUse = true;

                if ((int)cmbQueryCardType.SelectedValue != 000000)
                    type = (int)cmbQueryCardType.SelectedValue;

                switch (cmbQueryCardsUseStatus.Text)
                {
                    case "成功":
                        status = (int)OrderRechargeStatus.successful;
                        break;
                    case "失败":
                        status = (int)OrderRechargeStatus.failure;
                        break;
                    case "存疑":
                        status = (int)OrderRechargeStatus.suspicious;
                        break;
                    case "未使用":
                        status = (int)OrderRechargeStatus.untreated;
                        break;
                    case "处理中":
                        status = (int)OrderRechargeStatus.processing;
                        break;
                    case "全部":
                    default:
                        break;
                }

                CardSet = SQLCards.GetCards(cardNo, startTime, endTime, isUse, type, status);
            }
            catch (Exception)
            {
                throw;
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = CardSet;
            dgvQueryCards.DataSource = bs;
            bdnQueryCard.BindingSource = bs;
        }
        bool UpdateCard(DataGridViewRow row)
        {
            try
            {
                int cardsID = Convert.ToInt32(row.Cells["CardsID"].Value);

                Cards card = SQLCards.GetCards(p => p.CardsID == cardsID).FirstOrDefault();

                card.ChargeAccountTypeID = Convert.ToInt32(row.Cells["ChargeAccountTypeID"].Value);
                card.ReChargeStatus = Convert.ToInt32(row.Cells["ReChargeStatus"].Value);
                card.CardNumber = row.Cells["CardNumber"].Value.ToString();
                card.CardPassWord = row.Cells["CardPassWord"].Value.ToString();
                card.Price = Convert.ToInt32(row.Cells["Price"].Value);
                card.IsAvailable = Convert.ToBoolean(row.Cells["IsAvailable"].Value);

                return SQLCards.UpdateCards(card);
            }
            catch (Exception)
            {
            }
            return false;
        }
        bool DeleteCard(DataGridViewRow row)
        {
            try
            {
                int cardsID = Convert.ToInt32(row.Cells["CardsID"].Value);

                Cards card = SQLCards.GetCards(p => p.CardsID == cardsID).FirstOrDefault();

                return SQLCards.DeleteCards(card);
            }
            catch (Exception)
            {
            }
            return false;
        }


        #endregion
        #endregion

        #region QueryOrder
        #region QueryOrder Event
        private void btnQueryOrder_Click(object sender, EventArgs e)
        {
            QueryOrder();
        }

        private void btnOrderClear_Click(object sender, EventArgs e)
        {
            this.txtOrderInsideID.Text = "";
            this.txtOrderExternalID.Text = "";
            this.txtProductID.Text = "";
            this.txtProductName.Text = "";
            this.txtTargetAccount.Text = "";
            this.cmbOrderStatus.SelectedIndex = 0;
            dtOrderStartTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            dtOrderEndTime.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59");
        }
        private void dgvQueryOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isRefresh = false;
            if (dgvQueryOrder.Columns[e.ColumnIndex].Name == "UpdateOrder")
            {
                if (UpdateOrder(dgvQueryOrder.Rows[e.RowIndex]))
                {
                    if (NotifyOrder(dgvQueryOrder.Rows[e.RowIndex]))
                        MessageBox.Show("修改并通知成功");
                    else
                        MessageBox.Show("修改成功,通知失败，重新通知");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("修改失败");
            }
            if (dgvQueryOrder.Columns[e.ColumnIndex].Name == "NotifyOrder")
            {
                if (NotifyOrder(dgvQueryOrder.Rows[e.RowIndex]))
                {
                    MessageBox.Show("通知订单成功");
                    isRefresh = true;
                }
                else
                    MessageBox.Show("通知失败");
            }

            if (isRefresh)
            {
                QueryOrder();
            }
        }
        #endregion

        #region QueryOrder View
        void BindingQueryOrderDataGridColumns()
        {
            dgvQueryOrder.AllowUserToAddRows = false;
            dgvQueryOrder.AllowUserToDeleteRows = false;
            dgvQueryOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQueryOrder.AutoGenerateColumns = false;  //关闭自动产生列

            DataGridViewColumn cOrderID = new DataGridViewTextBoxColumn()
            {
                Name = "OrderID",
                DataPropertyName = "OrderID",
                HeaderText = "编号",
                ReadOnly = true
            };

            DataGridViewColumn cOrderInsideID = new DataGridViewTextBoxColumn()
            {
                Name = "OrderInsideID",
                DataPropertyName = "OrderInsideID",
                HeaderText = "系统订单号",
                ReadOnly = true
            };

            DataGridViewColumn cOrderExternalID = new DataGridViewTextBoxColumn()
            {
                Name = "OrderExternalID",
                DataPropertyName = "OrderExternalID",
                HeaderText = "商户订单号",
                ReadOnly = true
            };

            DataGridViewColumn cProductID = new DataGridViewTextBoxColumn()
            {
                Name = "ProductID",
                DataPropertyName = "ProductID",
                HeaderText = "商品编号",
                ReadOnly = true
            };


            DataGridViewColumn cProductName = new DataGridViewTextBoxColumn()
            {
                Name = "ProductName",
                DataPropertyName = "ProductName",
                HeaderText = "商品名称",
                ReadOnly = true
            };

            DataGridViewColumn cProductParValue = new DataGridViewTextBoxColumn()
            {
                Name = "ProductParValue",
                DataPropertyName = "ProductParValue",
                HeaderText = "商品面值",
                ReadOnly = true
            };

            DataGridViewColumn cTargetAccount = new DataGridViewTextBoxColumn()
            {
                Name = "TargetAccount",
                DataPropertyName = "TargetAccount",
                HeaderText = "充值帐号",
                ReadOnly = true
            };

            DataGridViewColumn cBuyAmount = new DataGridViewTextBoxColumn()
            {
                Name = "BuyAmount",
                DataPropertyName = "BuyAmount",
                HeaderText = "购买数量",
                ReadOnly = true

            };

            DataGridViewColumn cGameName = new DataGridViewTextBoxColumn()
            {
                Name = "GameName",
                DataPropertyName = "GameName",
                HeaderText = "游戏名称",
                ReadOnly = true
            };

            DataGridViewColumn cAreaName = new DataGridViewTextBoxColumn()
            {
                Name = "AreaName",
                DataPropertyName = "AreaName",
                HeaderText = "充值区域",
                ReadOnly = true
            };

            DataGridViewColumn cServerName = new DataGridViewTextBoxColumn()
            {
                Name = "ServerName",
                DataPropertyName = "ServerName",
                HeaderText = "充值服务器",
                ReadOnly = true
            }
            ;

            DataGridViewColumn cStartDatetime = new DataGridViewTextBoxColumn()
            {
                Name = "StartDatetime",
                DataPropertyName = "StartDatetime",
                HeaderText = "开始时间",
                ReadOnly = true
            };


            DataGridViewComboBoxColumn cmbRechargeStatus = new DataGridViewComboBoxColumn
            {
                Name = "RechargeStatus",
                DataPropertyName = "RechargeStatus",
                HeaderText = "充值状态"
            };
            List<RechargeStatusSet> ss = SetValues();
            cmbRechargeStatus.DataSource = ss;
            cmbRechargeStatus.DisplayMember = "description";
            cmbRechargeStatus.ValueMember = "RechargeStatus";


            DataGridViewColumn cSuccessfulAmount = new DataGridViewTextBoxColumn()
            {
                Name = "SuccessfulAmount",
                DataPropertyName = "SuccessfulAmount",
                HeaderText = "成功数量",
                ReadOnly = true
            };

            DataGridViewColumn cRechargeMsg = new DataGridViewTextBoxColumn()
            {
                Name = "RechargeMsg",
                DataPropertyName = "RechargeMsg",
                HeaderText = "充值描述",
                ReadOnly = true
            };


            DataGridViewColumn cChargeAccountInfo = new DataGridViewTextBoxColumn()
            {
                Name = "ChargeAccountInfo",
                DataPropertyName = "ChargeAccountInfo",
                HeaderText = "代充信息",
                ReadOnly = true
            };
            DataGridViewColumn cEndDatetime = new DataGridViewTextBoxColumn()
            {
                Name = "EndDatetime",
                DataPropertyName = "EndDatetime",
                HeaderText = "完成时间",
                ReadOnly = true
            };

            DataGridViewButtonColumn bUpdateOrder = new DataGridViewButtonColumn()
            {
                Name = "UpdateOrder",
                HeaderText = "操作",
                Text = "保存",
                UseColumnTextForButtonValue = true
            };
            DataGridViewButtonColumn bNotifyOrder = new DataGridViewButtonColumn()
            {
                Name = "NotifyOrder",
                HeaderText = "操作",
                Text = "通知",
                UseColumnTextForButtonValue = true
            };


            dgvQueryOrder.Columns.Add(cOrderID);
            dgvQueryOrder.Columns.Add(cOrderInsideID);
            dgvQueryOrder.Columns.Add(cOrderExternalID);
            dgvQueryOrder.Columns.Add(cProductID);
            dgvQueryOrder.Columns.Add(cProductName);
            dgvQueryOrder.Columns.Add(cProductParValue);
            dgvQueryOrder.Columns.Add(cTargetAccount);
            dgvQueryOrder.Columns.Add(cBuyAmount);
            dgvQueryOrder.Columns.Add(cmbRechargeStatus);
            dgvQueryOrder.Columns.Add(cSuccessfulAmount);
            dgvQueryOrder.Columns.Add(cRechargeMsg);
            dgvQueryOrder.Columns.Add(cChargeAccountInfo);
            dgvQueryOrder.Columns.Add(cGameName);
            dgvQueryOrder.Columns.Add(cAreaName);
            dgvQueryOrder.Columns.Add(cServerName);
            dgvQueryOrder.Columns.Add(cStartDatetime);
            dgvQueryOrder.Columns.Add(cEndDatetime);
            dgvQueryOrder.Columns.Add(bUpdateOrder);
            dgvQueryOrder.Columns.Add(bNotifyOrder);


        }
        #endregion

        #region QueryOrder Data

        void QueryOrder()
        {
            List<Order> orderSet = new List<Order>();
            try
            {
                string orderInsideID = this.txtOrderInsideID.Text.Trim();
                string orderExternalID = this.txtOrderExternalID.Text.Trim();
                string productID = this.txtProductID.Text.Trim();
                string productName = this.txtProductName.Text.Trim();
                string targetAccount = this.txtTargetAccount.Text.Trim();
                string reChargeAccount = this.txtReChargeAccount.Text.Trim();
                DateTime startTime = this.dtOrderStartTime.Value;
                DateTime endTime = this.dtOrderEndTime.Value;
                int status = -1;
                switch (cmbOrderStatus.Text)
                {

                    case "成功":
                        status = (int)OrderRechargeStatus.successful;
                        break;
                    case "失败":
                        status = (int)OrderRechargeStatus.failure;
                        break;
                    case "存疑":
                        status = (int)OrderRechargeStatus.suspicious;
                        break;
                    case "未处理":
                        status = (int)OrderRechargeStatus.untreated;
                        break;
                    case "处理中":
                        status = (int)OrderRechargeStatus.processing;
                        break;
                    case "全部":
                    default:
                        break;
                }

                orderSet = SQLOrder.GetOrder(orderInsideID, orderExternalID, productID, productName, targetAccount,reChargeAccount, startTime, endTime, status)
                    .OrderByDescending(p => p.StartDatetime).ToList();

            }
            catch (Exception)
            {

                throw;
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = orderSet;
            dgvQueryOrder.DataSource = bs;
            bdnQueryOrder.BindingSource = bs;
        }
        bool UpdateOrder(DataGridViewRow row)
        {
            try
            {
                int orderID = Convert.ToInt32(row.Cells["OrderID"].Value);

                Order order = new SQLOrder().GetOrder(p => p.OrderID == orderID).FirstOrDefault();

                order.RechargeStatus = Convert.ToInt32(row.Cells["RechargeStatus"].Value);

                return new SQLOrder().UpdateOrder(order);
            }
            catch (Exception)
            {
            }
            return false;
        }
        bool NotifyOrder(DataGridViewRow row)
        {
            try
            {
                int orderID = Convert.ToInt32(row.Cells["OrderID"].Value);

                Order order = new SQLOrder().GetOrder(p => p.OrderID == orderID).FirstOrDefault();

                ClientConfig clientConfigSets = SQLClientConfig.GetClientConfig(p => p.ClientConfigID != null).FirstOrDefault();

                return new ChargeInterface.SW.ManageSW().notigyOrderToSW(order);
            }
            catch (Exception)
            {
            }
            return false;
        }

        #endregion

       
        #endregion

        #region CardStore
        #region Event
        private void btnQueryStore_Click(object sender, EventArgs e)
        {
            QueryCardsForStore();
        }
        private void btnClearStore_Click(object sender, EventArgs e)
        {
            
        }
        private void chkUsetimeForStore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsetimeForStore.Checked)
                chkAddtimeForStore.Checked = false;
            else
                chkAddtimeForStore.Checked = true;
        }
        private void chkAddtimeForStore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAddtimeForStore.Checked)
                chkUsetimeForStore.Checked = false;
            else
                chkUsetimeForStore.Checked = true;
        }
        #endregion

        #region View
        void BindingCardStoreDataGridColumns()
        {

            dgvCardStore.AllowUserToAddRows = false;
            dgvCardStore.AllowUserToDeleteRows = false;
            dgvCardStore.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvCardStore.AutoGenerateColumns = false;  //关闭自动产生列


            DataGridViewColumn cID = new DataGridViewTextBoxColumn()
            {
                Name = "CardTypeID",
                DataPropertyName = "CardTypeID",
                HeaderText = "卡密编号",
                ReadOnly = true
            };

            DataGridViewColumn cCardTypeDescription = new DataGridViewTextBoxColumn()
            {
                Name = "CardTypeDescription",
                DataPropertyName = "CardTypeDescription",
                HeaderText = "卡密名称",
                ReadOnly = true
            };

            DataGridViewColumn cCardValue = new DataGridViewTextBoxColumn()
            {
                Name = "CardValue",
                DataPropertyName = "CardValue",
                HeaderText = "卡密面值",
                ReadOnly = true
            };

            DataGridViewColumn cTotalCount = new DataGridViewTextBoxColumn()
            {
                Name = "TotalCount",
                DataPropertyName = "TotalCount",
                HeaderText = "总数量",
                ReadOnly = true
            };


            DataGridViewColumn cUntreatedCount = new DataGridViewTextBoxColumn()
            {
                Name = "UntreatedCount",
                DataPropertyName = "UntreatedCount",
                HeaderText = "未使用数量",
                ReadOnly = true
            };

            DataGridViewColumn cProcessingCount = new DataGridViewTextBoxColumn()
            {
                Name = "ProcessingCount",
                DataPropertyName = "ProcessingCount",
                HeaderText = "处理中数量",
                ReadOnly = true
            };

            DataGridViewColumn cSuccessfulCount = new DataGridViewTextBoxColumn()
            {
                Name = "SuccessfulCount",
                DataPropertyName = "SuccessfulCount",
                HeaderText = "成功数量",
                ReadOnly = true
            };

            DataGridViewColumn cFailureCount = new DataGridViewTextBoxColumn()
            {
                Name = "FailureCount",
                DataPropertyName = "FailureCount",
                HeaderText = "失败数量",
                ReadOnly = true

            };

            DataGridViewColumn cSuspiciousCount = new DataGridViewTextBoxColumn()
            {
                Name = "SuspiciousCount",
                DataPropertyName = "SuspiciousCount",
                HeaderText = "存疑数量",
                ReadOnly = true
            };

            //DataGridViewButtonColumn bUpdateOrder = new DataGridViewButtonColumn()
            //{
            //    Name = "UpdateOrder",
            //    HeaderText = "操作",
            //    Text = "保存",
            //    UseColumnTextForButtonValue = true
            //};
            //DataGridViewButtonColumn bNotifyOrder = new DataGridViewButtonColumn()
            //{
            //    Name = "NotifyOrder",
            //    HeaderText = "操作",
            //    Text = "通知",
            //    UseColumnTextForButtonValue = true
            //};

            dgvCardStore.Columns.Add(cID);
            dgvCardStore.Columns.Add(cCardTypeDescription);
            dgvCardStore.Columns.Add(cCardValue);
            dgvCardStore.Columns.Add(cTotalCount);
            dgvCardStore.Columns.Add(cUntreatedCount);
            dgvCardStore.Columns.Add(cProcessingCount);
            dgvCardStore.Columns.Add(cSuccessfulCount);
            dgvCardStore.Columns.Add(cFailureCount);
            dgvCardStore.Columns.Add(cSuspiciousCount);


        }
        #endregion

        #region Data

        void QueryCardsForStore()
        {
            List<Cards> CardSet = new List<Cards>();
            try
            {
                DateTime startTime = dtStartTimeForStore.Value;
                DateTime endTime = dtEndTimeForStore.Value;
                bool isUse = false;
                int type = -1;

                if (chkUsetimeForStore.Checked)
                    isUse = true;

                if ((int)cmbCardTypeForStore.SelectedValue != 000000)
                    type = (int)cmbCardTypeForStore.SelectedValue;


                CardSet = SQLCards.GetCards(startTime, endTime, isUse, type);

                SetCardStoreDataSouce(CardSet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        void SetCardStoreDataSouce(List<Cards>cardSet)
        {
            List<CardStore> cardStoreSet = new List<CardStore>();
            try
            {
                List<IGrouping<int, Cards>> cardType = cardSet.GroupBy(p => p.ChargeAccountTypeID).ToList();
                foreach (IGrouping<int, Cards> group in cardType)
                {
                    List<Cards> cardValue = group.ToList();
                    var result = cardValue.GroupBy(a => a.Price)
                              .Select(g => (new
                              {
                                  value = g.Key,
                                  count = g.Count(),
                                  ageUntreated = g.Count(item => item.ReChargeStatus == 0),
                                  ageProcessing = g.Count(item => item.ReChargeStatus == 1),
                                  ageSuccessful = g.Count(item => item.ReChargeStatus == 2),
                                  ageFailure = g.Count(item => item.ReChargeStatus == 3),
                                  ageSuspicious = g.Count(item => item.ReChargeStatus == 4)
                              }));

                    foreach (var item in result)
                    {
                        CardStore cardStore = new CardStore();
                        cardStore.CardTypeID = cardValue[0].ChargeAccountTypeID;
                        cardStore.CardTypeDescription = SQLChargeAccountType.GetChargeAccountType(p => p.ChargeAccountTypeID == cardValue[0].ChargeAccountTypeID).FirstOrDefault().Description;
                        cardStore.CardValue =Convert.ToInt16(item.value);
                        cardStore.TotalCount = item.count;
                        cardStore.UntreatedCount = Convert.ToInt16(item.ageUntreated);
                        cardStore.ProcessingCount = Convert.ToInt16(item.ageProcessing);
                        cardStore.SuccessfulCount = Convert.ToInt16(item.ageSuccessful);
                        cardStore.FailureCount = Convert.ToInt16(item.ageFailure);
                        cardStore.SuspiciousCount = Convert.ToInt16(item.ageSuspicious);
                        cardStoreSet.Add(cardStore);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = cardStoreSet;
            dgvCardStore.DataSource = bs;
            bdnCardStore.BindingSource = bs;
        }

        #endregion

        #endregion

        #region QueryZYCardBalance

        private void btnClearZY_Click(object sender, EventArgs e)
        {
            TxtUsed.Text = "";
            TXTQueryZY.Text = "";
            TXTNoUserd.Text = "";
        }

        private void btnQueryZYCard_Click(object sender, EventArgs e)
        {
            btnClearZY.Enabled = false;
            btnQueryZYCard.Enabled = false;
        }
        


        #endregion
    }
}
