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
using Common;
using System.Threading;
using Common.LogHelper;
using Common.UUWise;
using System.IO;
using System.Runtime.InteropServices;
using DBOperation.SQLHelper;
using ChargeInterface.SW;
using System.Reflection;
using ChargeInterface.SUP;

namespace Client
{
    public partial class MainForm : Form
    {
        Thread getOrderFormDBThread;
        Thread GCThread;

        bool threadStatus = false;

        Sunisoft.IrisSkin.SkinEngine se = null;
        public MainForm()
        {
            InitializeComponent();
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;//所有窗体均应用此皮肤
            se.SkinFile = AppDomain.CurrentDomain.BaseDirectory + "\\skin\\RealOne.ssk";
        }
        void ApplicationStart()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 10;

            if (getOrderFormDBThread != null)
            {
                while (getOrderFormDBThread.IsAlive)
                    Thread.Sleep(3000);
                getOrderFormDBThread.Abort();
            }
            getOrderFormDBThread = new Thread(new ThreadStart(GetOrderFormDB));
            getOrderFormDBThread.SetApartmentState(System.Threading.ApartmentState.STA);
            getOrderFormDBThread.Start();
        }

        #region EVENT
        private void MainForm_Load(object sender, EventArgs e)
        {
            //string dd = Common.VbiHelper.VbiChargeHelper.GetPwdMethod(new string[] { "1232" });

            //WrapperHelp.SetSoftInfo();
            //WrapperHelp.Login_UU();

            this.btnCancel.Enabled = false;

            BindingDataGridColumns();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.btnOK.Enabled = false;
            this.btnCancel.Enabled = true;

            threadStatus = true;
            ApplicationStart();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = false;

            threadStatus = false;
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.Exit();
            System.Environment.Exit(0);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadStatus)
            {
                MessageBox.Show("请先停止充值再关闭程序！");
                e.Cancel = true;
                return;
            }
        }
        #endregion

        #region Thread
        void GetOrderFormDB()
        {
            while (threadStatus)
            {
                try
                {
                    List<Order> reChargeOrderSet = new List<Order>();
                    reChargeOrderSet = SQLOrder.GetBySql();

                    if (reChargeOrderSet!=null )
                    foreach (Order order in reChargeOrderSet)
                    {
                        order.RechargeStatus = (int)OrderRechargeStatus.processing;
                        if (SQLOrder.UpdateBySql(order.OrderID, order.RechargeStatus))
                        {
                            Thread rechargeThread = new Thread(new ParameterizedThreadStart(Recharge));
                            rechargeThread.Start(order);
                            Thread.Sleep(500);
                        }
                    }
                   
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    WriteLog.Write("方法:GetOrderFormDB异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception.ToString());
                }
            }
        }

        void Recharge(object obj)
        {
            try
            {
                AssignmentDatagirdView((Order)obj);

                Order order = payOrder(obj);
                order.EndDatetime = DateTime.Now;

                string logMsg = string.Format("订单号:{0},代充商品：{1},代充帐号：{2},订单充值状态：{3},订单充值描述：{4}"
                    , order.OrderInsideID, order.ProductName, order.TargetAccount, EnumService.GetDescription((int)order.RechargeStatus), order.RechargeMsg);
                WriteLog.Write(logMsg, LogPathFile.Recharge.ToString());

                AssignmentDatagirdView(order);

                if (order.MerchantCode == MerchantCodeType.SW)
                {
                   new ManageSW().notigyOrderToSW(order);

                   order.IsNotify = true;
                }

                new SQLOrder().UpdateOrder(order);

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:Recharge异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Recharge.ToString());
            }
        }

        Order payOrder(object obj)
        {
            Order order = (Order)obj;

            List<Product> productLst = SQLProduct.GetProducts(p => p.MerchantCode==order.MerchantCode);
            if (productLst == null || productLst.Count == 0)
            {
                WriteLog.Write("payOrder: productLst is null, 订单号：" + order.OrderInsideID, LogPathFile.Recharge.ToString());
                return order;
            }

            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.ChargeClassID > 0);
            if (chargeClassLst == null || chargeClassLst.Count == 0)
            {
                WriteLog.Write("payOrder: chargeClassLst is null, 订单号：" + order.OrderInsideID, LogPathFile.Recharge.ToString());
                return order;

            }

            foreach (Product item in productLst)
            {
                if (order.ProductID.Equals(item.ProductCode) && item.IsActive)
                {
                    var result = chargeClassLst.SingleOrDefault(p => p.ChargeClassID == item.ChargeClassID);

                    order = (Order)ReflectChargeClasss(order, result.ChargeClassName);

                    break;
                }
            }

            return order;
        }

        object ReflectChargeClasss(Order order, string chargeClass)
        {
            string reflectClass = "ChargeInterface.Charge." + chargeClass;

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
                MethodInfo method = type.GetMethod("Charge");
                return method.Invoke(instance, params_obj);
            }
            catch (Exception ex)
            {
                WriteLog.Write("ReflectChargeClasss1: 订单号：" + order.OrderInsideID + ",异常信息：" + chargeClass+"," + ex.Message, LogPathFile.Exception.ToString());

                if (order.RechargeStatus == (int)OrderRechargeStatus.processing)
                    order.RechargeStatus = (int)OrderRechargeStatus.untreated;
                return order;
            }
        }
        
        #endregion

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
