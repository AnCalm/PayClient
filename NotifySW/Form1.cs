using Common;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBOperation.SQLHelper;
using ChargeInterface.SW;
using ChargeInterface.SUP;
using System.Reflection;

namespace NotifyToMerchant
{
    public partial class FrmNotify : Form
    {
        Thread threadNotify;

        bool threadStatus = false;

        public FrmNotify()
        {
            InitializeComponent();
        }

        private void FrmNotify_Load(object sender, EventArgs e)
        {
            threadStatus = false;
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = false;

            BindingDataGridColumns();

            loadProductView();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                threadStatus = true;
                this.btnOK.Enabled = false;
                this.btnCancel.Enabled = true;

                SetProductCheckBoxEnabled(false);

                if (threadNotify != null)
                {
                    while (threadNotify.IsAlive)
                        Thread.Sleep(3000);
                    threadNotify.Abort();
                }
                threadNotify = new Thread(new ThreadStart(Notify));
                threadNotify.SetApartmentState(System.Threading.ApartmentState.STA);
                threadNotify.Start();
            }
            catch (Exception)
            {
                threadStatus = false;
                this.btnOK.Enabled = true;
                this.btnCancel.Enabled = false;
                SetProductCheckBoxEnabled(true);

                throw;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            threadStatus = false;
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = false;

            SetProductCheckBoxEnabled(true);
        }

        private void FrmNotify_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void FrmNotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadStatus)
            {
                MessageBox.Show("请先停止挂机再关闭程序！");
                e.Cancel = true;
                return;
            }
        }
        void Notify()
        {
            List<int> chargeClassIDList = new List<int>();
            string sort = "asc";

            while (threadStatus)
            {
                try
                {
                    if (chargeClassIDList == null || chargeClassIDList.Count <= 0)
                        chargeClassIDList = GetNotifyChargeClass();

                    if (chargeClassIDList == null)
                        continue;

                    List<Order> NotifyOrderSet = new SQLOrder().GetNotNotifyOrderBySql(sort, chargeClassIDList);

                    if (NotifyOrderSet == null)
                    {
                        Thread.Sleep(1 * 5000);
                        continue;
                    }

                    //待返回的订单
                    List<Order> nOrderSet = NotifyOrderSet.Where(n => n.RechargeStatus == (int)OrderRechargeStatus.successful
                        || n.RechargeStatus == (int)OrderRechargeStatus.failure
                        || n.RechargeStatus == (int)OrderRechargeStatus.suspicious).ToList();

                    //待查询的订单
                    List<Order> qOrderSet = NotifyOrderSet.Where(n => n.RechargeStatus == (int)OrderRechargeStatus.Submit).ToList();

                    foreach (object order in qOrderSet)
                    {
                        Order queryOrder = (Order)order;

                        AssignmentDatagirdView(queryOrder);

                        TimeSpan timeSpan = DateTime.Now - queryOrder.StartDatetime.Value;

                        if (timeSpan.TotalSeconds < 30)
                        {
                            continue;
                        }

                        if (timeSpan.TotalMinutes >= 15)
                        {
                            queryOrder.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                            queryOrder.RechargeMsg = "订单超时，挂机存疑";
                            Common.LogHelper.WriteLog.Write("方法:Notify，订单号：" + queryOrder.OrderInsideID + " 订单状态查询超时，设为可疑:" + EnumService.GetDescription((int)queryOrder.RechargeStatus), LogPathFile.Other.ToString());
                        }

                        if (queryOrder.RechargeStatus == (int)OrderRechargeStatus.Submit)
                        {
                            queryOrder = payOrder(order);
                        }

                        if (queryOrder.RechargeStatus == (int)OrderRechargeStatus.successful
                            || queryOrder.RechargeStatus == (int)OrderRechargeStatus.failure
                            || queryOrder.RechargeStatus == (int)OrderRechargeStatus.suspicious)
                        {
                            if (nOrderSet == null)
                                nOrderSet = new List<Order>();

                            nOrderSet.Add(queryOrder);
                        }
                    }


                    if (nOrderSet != null && nOrderSet.Count > 0)
                    {
                        NotifyOrders(nOrderSet);
                        Thread.Sleep(1 * 500);
                    }
                    else
                        Thread.Sleep(1 * 1000);

                    sort = sort == "asc" ? "desc" : "asc";
                }
                catch (Exception ex)
                {
                    Common.LogHelper.WriteLog.Write("方法:Notify，异常：" + ex.Message + " ," + ex.Source, LogPathFile.Exception.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// 订单返回通知
        /// </summary>
        /// <param name="NotifyOrderSet"></param>
        void NotifyOrders(List<Order> NotifyOrderSet)
        {
            try
            {
                List<Order> orders = new List<Order>();

                foreach (object order in NotifyOrderSet)
                {
                    Order queryOrder = (Order)order;

                    bool isNotifyState = false;

                    if (queryOrder.MerchantCode == MerchantCodeType.SW)
                        isNotifyState = new ManageSW().notigyOrderToSW(queryOrder);
                    else if (queryOrder.MerchantCode == MerchantCodeType.SUP)
                        isNotifyState = new GetAndNotifySUPOrders().notigyOrderToSUP(queryOrder);

                    if (isNotifyState)
                    {
                        queryOrder.IsNotify = true;
                        queryOrder.EndDatetime = DateTime.Now;

                        orders.Add(queryOrder);
                    }

                    AssignmentDatagirdView(queryOrder);
                }

                //更新本地数据库
                if (orders != null && orders.Count > 0)
                {
                    //foreach (var item in orders)
                    //{
                    //    new SQLOrder().UpdateOrder(item);
                    //}
                    new SQLOrder().MultiUpdateData(orders);
                }
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog.Write("方法:NotifyOrders，异常：" + ex.Message + " ," + ex.Source, LogPathFile.Exception.ToString());
            }
        }

        Order payOrder(object obj)
        {
            Order order = (Order)obj;
            try
            {
                List<Product> productLst = SQLProduct.GetProducts(p => p.MerchantCode == order.MerchantCode);
                if (productLst == null || productLst.Count == 0)
                {
                    Common.LogHelper.WriteLog.Write("payOrder: productLst is null, 订单号：" + order.OrderInsideID, LogPathFile.Recharge.ToString());
                    return order;
                }

                List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.ChargeClassID > 0);
                if (chargeClassLst == null || chargeClassLst.Count == 0)
                {
                    Common.LogHelper.WriteLog.Write("payOrder: chargeClassLst is null, 订单号：" + order.OrderInsideID, LogPathFile.Recharge.ToString());
                    return order;
                }

                foreach (Product item in productLst)
                {
                    if (order.ProductID.Equals(item.ProductCode) && item.IsActive)
                    {
                        var result = chargeClassLst.SingleOrDefault(p => p.ChargeClassID == item.ChargeClassID);

                        order = (Order)ReflectQueryClasss(order, result.QueryClassName);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog.Write("方法:Query异常：" + ex.Message, LogPathFile.Other.ToString());
            }

            return order;
        }

        object ReflectQueryClasss(Order order, string queryClass)
        {
            string reflectClass = "ChargeInterface.Query." + queryClass;

            try
            {
                //加载程序集(dll文件地址)，使用Assembly类   
                Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "ChargeInterface.dll");

                //获取类型，参数（名称空间+类）   
                Type type = assembly.GetType(reflectClass);

                //创建该对象的实例，object类型，参数（名称空间+类）   
                object instance = assembly.CreateInstance(reflectClass);

                //设置方法中的参数值；如有多个参数可以追加多个   
                Object[] params_obj = new Object[1];
                params_obj[0] = order;


                //执行方法   
                MethodInfo method = type.GetMethod("Query");
                return method.Invoke(instance, params_obj);
            }
            catch (Exception ex)
            {
                return order;
            }

        }

        List<string> GetNotifyProduct()
        {
            List<Product> productLst = SQLProduct.GetProducts(p => p.ProductID > 0);

            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.ChargeClassID > 0);

            List<string> productIDList = new List<string>();

            foreach (Control c in this.pannelCheckbox.Controls)
            {
                if (c is CheckBox && (c as CheckBox).Checked)
                {
                    string checkTxt = (c as CheckBox).Text;

                    foreach (ChargeClass chargeClass in chargeClassLst)
                    {
                        if (checkTxt.Equals(chargeClass.Descrtion))
                        {
                            foreach (Product product in productLst)
                            {
                                if (product.ChargeClassID.Equals(chargeClass.ChargeClassID))
                                {
                                    productIDList.Add(product.ProductCode);
                                }
                            }
                        }

                    }
                }
            }

            return productIDList;
        }

        List<int> GetNotifyChargeClass()
        {
            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.ChargeClassID > 0);

            List<int> chargeClassIDList = new List<int>();

            foreach (Control c in this.pannelCheckbox.Controls)
            {
                if (c is CheckBox && (c as CheckBox).Checked)
                {
                    string checkTxt = (c as CheckBox).Text;

                    foreach (ChargeClass chargeClass in chargeClassLst)
                    {
                        if (checkTxt.Equals(chargeClass.Descrtion))
                        {
                            chargeClassIDList.Add(chargeClass.ChargeClassID);
                        }

                    }
                }
            }

            return chargeClassIDList;
        }

        void loadProductView()
        {
            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.ChargeClassID > 0);

            int i = 1; int J = 1;
            foreach (ChargeClass item in chargeClassLst)
            {
                CheckBox chbox = new CheckBox();
                int x = 30 * i + ((i - 1) * 100);
                int y = 30 * J;
                chbox.Location = new System.Drawing.Point(x, y);
                chbox.Text = item.Descrtion;
                chbox.Size = new System.Drawing.Size(100, 16);

                this.pannelCheckbox.Controls.Add(chbox);

                i++;
                if (i == 9)
                {
                    J++;
                    i = 1;
                }
            }




        }

        void SetProductCheckBoxEnabled(bool IsEnabled)
        {
            foreach (Control c in this.pannelCheckbox.Controls)
            {
                if (c is CheckBox)
                {
                    (c as CheckBox).Enabled = IsEnabled;
                }
            }
        }

        #region View

        delegate void DelAssignmentDatagirdView(Order order);
        void AssignmentDatagirdView(Order order)
        {
            try
            {
                this.BeginInvoke(new System.Threading.ThreadStart(delegate()
                {
                    bool isNew = true;
                    foreach (DataGridViewRow item in dgOrderCharge.Rows)
                    {
                        if (dgOrderCharge.Rows[item.Index].Cells["OrderInsideID"].Value.Equals(order.OrderInsideID))
                        {
                            isNew = false;
                            dgOrderCharge.Rows[item.Index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
                            dgOrderCharge.Rows[item.Index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
                            dgOrderCharge.Rows[item.Index].Cells["RechargeMsg"].Value = order.RechargeMsg;
                            dgOrderCharge.Rows[item.Index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
                            dgOrderCharge.Rows[item.Index].Cells["EndDatetime"].Value = order.EndDatetime;
                            dgOrderCharge.Rows[item.Index].Cells["IsNotify"].Value = order.IsNotify == false ? "未通知" : "通知成功";
                            SetGirdRowColor(item);
                            break;
                        }
                    }

                    if (isNew)
                    {
                        AddDataGridRows(order);
                    }
                    Application.DoEvents();
                }));
            }
            catch (Exception)
            {

                throw;
            }
            //if (this.InvokeRequired)
            //{
            //    this.Invoke(new DelAssignmentDatagirdView(AssignmentDatagirdView), new object[] { order });
            //}
            //else
            //{
            //    try
            //    {
            //        bool isNew = true;
            //        foreach (DataGridViewRow item in dgOrderCharge.Rows)
            //        {
            //            if (dgOrderCharge.Rows[item.Index].Cells["OrderInsideID"].Value.Equals(order.OrderInsideID))
            //            {
            //                isNew = false;
            //                dgOrderCharge.Rows[item.Index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
            //                dgOrderCharge.Rows[item.Index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
            //                dgOrderCharge.Rows[item.Index].Cells["RechargeMsg"].Value = order.RechargeMsg;
            //                dgOrderCharge.Rows[item.Index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
            //                dgOrderCharge.Rows[item.Index].Cells["EndDatetime"].Value = order.EndDatetime;

            //                SetGirdRowColor(item);
            //                break;
            //            }
            //        }

            //        if (isNew)
            //        {
            //            AddDataGridRows(order);
            //        }

            //        Application.DoEvents();
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
        }
        void AddDataGridRows(Order order)
        {
            int index = this.dgOrderCharge.Rows.Add();

            dgOrderCharge.Rows[index].Cells["OrderInsideID"].Value = order.OrderInsideID;
            dgOrderCharge.Rows[index].Cells["OrderExternalID"].Value = order.OrderExternalID;
            dgOrderCharge.Rows[index].Cells["ProductName"].Value = order.ProductName;
            dgOrderCharge.Rows[index].Cells["ProductParValue"].Value = order.ProductParValue;
            dgOrderCharge.Rows[index].Cells["TargetAccount"].Value = order.TargetAccount;
            dgOrderCharge.Rows[index].Cells["BuyAmount"].Value = order.BuyAmount;
            dgOrderCharge.Rows[index].Cells["GameName"].Value = order.GameName;
            dgOrderCharge.Rows[index].Cells["AreaName"].Value = order.AreaName;
            dgOrderCharge.Rows[index].Cells["ServerName"].Value = order.ServerName;
            dgOrderCharge.Rows[index].Cells["StartDatetime"].Value = order.StartDatetime;
            dgOrderCharge.Rows[index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
            dgOrderCharge.Rows[index].Cells["IsNotify"].Value = order.IsNotify == false ? "未通知" : "通知成功";
            dgOrderCharge.Rows[index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
            dgOrderCharge.Rows[index].Cells["RechargeMsg"].Value = order.RechargeMsg;
            dgOrderCharge.Rows[index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
            dgOrderCharge.Rows[index].Cells["EndDatetime"].Value = order.EndDatetime;

            while (dgOrderCharge.Rows.Count > 20)
            {
                dgOrderCharge.Rows.RemoveAt(0);
            }
        }
        void BindingDataGridColumns()
        {
            dgOrderCharge.AllowUserToAddRows = false;
            dgOrderCharge.AllowUserToDeleteRows = false;

            DataGridViewColumn c0 = new DataGridViewTextBoxColumn()
            {
                Name = "OrderInsideID",
                DataPropertyName = "OrderInsideID",
                HeaderText = "系统订单号",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c1 = new DataGridViewTextBoxColumn()
            {
                Name = "OrderExternalID",
                DataPropertyName = "OrderExternalID",
                HeaderText = "商户订单号",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c2 = new DataGridViewTextBoxColumn()
            {
                Name = "ProductName",
                DataPropertyName = "ProductName",
                HeaderText = "商品名称",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c22 = new DataGridViewTextBoxColumn()
            {
                Name = "ProductParValue",
                DataPropertyName = "ProductParValue",
                HeaderText = "商品面值",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c3 = new DataGridViewTextBoxColumn()
            {
                Name = "TargetAccount",
                DataPropertyName = "TargetAccount",
                HeaderText = "充值帐号",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c4 = new DataGridViewTextBoxColumn()
            {
                Name = "BuyAmount",
                DataPropertyName = "BuyAmount",
                HeaderText = "购买数量",
                Width = 100,
                Frozen = false

            };

            DataGridViewColumn c5 = new DataGridViewTextBoxColumn()
            {
                Name = "GameName",
                DataPropertyName = "GameName",
                HeaderText = "游戏名称",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c6 = new DataGridViewTextBoxColumn()
            {
                Name = "AreaName",
                DataPropertyName = "AreaName",
                HeaderText = "充值区域",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c7 = new DataGridViewTextBoxColumn()
            {
                Name = "ServerName",
                DataPropertyName = "ServerName",
                HeaderText = "充值服务器",
                Width = 100,
                Frozen = false
            }
            ;

            DataGridViewColumn c8 = new DataGridViewTextBoxColumn()
            {
                Name = "StartDatetime",
                DataPropertyName = "StartDatetime",
                HeaderText = "开始时间",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c9 = new DataGridViewTextBoxColumn()
            {
                Name = "RechargeStatus",
                DataPropertyName = "RechargeStatus",
                HeaderText = "充值状态",
                Width = 100,
                Frozen = false
            }
            ;
            DataGridViewColumn c99 = new DataGridViewTextBoxColumn()
            {
                Name = "IsNotify",
                DataPropertyName = "IsNotify",
                HeaderText = "数网通知状态",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c11 = new DataGridViewTextBoxColumn()
            {
                Name = "SuccessfulAmount",
                DataPropertyName = "SuccessfulAmount",
                HeaderText = "成功数量",
                Width = 100,
                Frozen = false
            };

            DataGridViewColumn c111 = new DataGridViewTextBoxColumn()
            {
                Name = "RechargeMsg",
                DataPropertyName = "RechargeMsg",
                HeaderText = "充值描述",
                Width = 100,
                Frozen = false
            };


            DataGridViewColumn c12 = new DataGridViewTextBoxColumn()
            {
                Name = "ChargeAccountInfo",
                DataPropertyName = "ChargeAccountInfo",
                HeaderText = "代充信息",
                Width = 100,
                Frozen = false
            };
            DataGridViewColumn c13 = new DataGridViewTextBoxColumn()
            {
                Name = "EndDatetime",
                DataPropertyName = "EndDatetime",
                HeaderText = "完成时间",
                Width = 100,
                Frozen = false
            };



            dgOrderCharge.Columns.Add(c0);
            dgOrderCharge.Columns.Add(c1);
            dgOrderCharge.Columns.Add(c2);
            dgOrderCharge.Columns.Add(c22);
            dgOrderCharge.Columns.Add(c3);
            dgOrderCharge.Columns.Add(c4);
            dgOrderCharge.Columns.Add(c9);
            dgOrderCharge.Columns.Add(c99);
            dgOrderCharge.Columns.Add(c11);
            dgOrderCharge.Columns.Add(c111);
            dgOrderCharge.Columns.Add(c12);
            dgOrderCharge.Columns.Add(c5);
            dgOrderCharge.Columns.Add(c6);
            dgOrderCharge.Columns.Add(c7);
            dgOrderCharge.Columns.Add(c8);
            dgOrderCharge.Columns.Add(c13);
        }
        void SetGirdRowColor(DataGridViewRow row)
        {
            switch (dgOrderCharge.Rows[row.Index].Cells["RechargeStatus"].Value.ToString())
            {
                case "处理中":
                    dgOrderCharge.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Blue;
                    break;
                case "充值成功":
                    dgOrderCharge.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Green;
                    break;
                case "充值失败":
                    dgOrderCharge.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Red;
                    break;
                case "充值存疑":
                    dgOrderCharge.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Violet;
                    break;
            }
        }
        #endregion



    }
}
