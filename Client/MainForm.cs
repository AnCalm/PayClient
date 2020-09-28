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
        //从数据库获取订单线程
        Thread getOrderFormDBThread;
        //从第三方获取订单线程
        Thread getOrderFromThirdThread;

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

            //开启从DB数据库获取订单线程
            if (getOrderFormDBThread != null)
            {
                while (getOrderFormDBThread.IsAlive)
                    Thread.Sleep(3000);
                getOrderFormDBThread.Abort();
            }
            getOrderFormDBThread = new Thread(new ThreadStart(GetOrderFormDB));
            getOrderFormDBThread.SetApartmentState(System.Threading.ApartmentState.STA);
            getOrderFormDBThread.Start();

            //开启从第三方获取订单线程
            if (getOrderFromThirdThread != null)
            {
                while (getOrderFromThirdThread.IsAlive)
                    Thread.Sleep(3000);
                getOrderFromThirdThread.Abort();
            }
            getOrderFromThirdThread = new Thread(new ThreadStart(GetSWOrders));
            getOrderFromThirdThread.SetApartmentState(System.Threading.ApartmentState.STA);
            getOrderFromThirdThread.Start();
        }

        #region EVENT
        private void MainForm_Load(object sender, EventArgs e)
        {
            //string dd = Common.VbiHelper.VbiChargeHelper.GetPwdMethod(new string[] { "1232" });

            //WrapperHelp.SetSoftInfo();
            //WrapperHelp.Login_UU();

            this.btnCancel.Enabled = false;

            BindingDataGridColumns();

            QueryBindingDataGridColumns();

            loadSW();

            loadProductView();
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnOK.Enabled = false;
                this.btnCancel.Enabled = true;
                this.btnSaveSW.Enabled = false;
                setSWEnabled(false);
                SetProductCheckBoxEnabled(false);
                SaveProduct();
                threadStatus = true;
                //ApplicationStart();
            }
            catch (Exception)
            {
                threadStatus = false;
                this.btnOK.Enabled = true;
                this.btnCancel.Enabled = false;
                this.btnSaveSW.Enabled = true;
                setSWEnabled(true);
                SetProductCheckBoxEnabled(true);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = false;
            this.btnSaveSW.Enabled = true;
            setSWEnabled(true);
            SetProductCheckBoxEnabled(true);
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

        private void btnSaveSW_Click(object sender, EventArgs e)
        {
            saveSW();
        }
        #endregion

        #region ChargeThread
        void GetOrderFormDB()
        {
            while (threadStatus)
            {
                try
                {
                    List<Order> reChargeOrderSet = new List<Order>();
                    reChargeOrderSet = SQLOrder.GetBySql();

                    if (reChargeOrderSet != null)
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

            List<Product> productLst = SQLProduct.GetProducts(p => p.MerchantCode == order.MerchantCode);
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
                WriteLog.Write("ReflectChargeClasss1: 订单号：" + order.OrderInsideID + ",异常信息：" + chargeClass + "," + ex.Message, LogPathFile.Exception.ToString());

                if (order.RechargeStatus == (int)OrderRechargeStatus.processing)
                    order.RechargeStatus = (int)OrderRechargeStatus.untreated;
                return order;
            }
        }

        //从数网获取充值订单
        void GetSWOrders()
        {
            while (threadStatus)
            {
                try
                {
                    int time = 0;

                    List<Order> orderSet = new ManageSW().getOrderFromSW(ref time);

                    if (orderSet == null)
                    {
                        Thread.Sleep(time * 1000);
                        continue;
                    }

                    foreach (Order order in orderSet)
                    {
                        if (new ManageSW().notigyOrderToSW(order, true))
                        {
                            bool result = SQLOrder.AddOrder(order);
                            int Recount = 0;
                            while (!result)
                            {
                                if (Recount > 10)
                                {
                                    break;
                                }
                                result = SQLOrder.AddOrder(order);
                                Recount++;
                                Thread.Sleep(1 * 1000);
                            }
                        }
                    }
                    Thread.Sleep(time * 1000);
                }
                catch (Exception ex)
                {
                    WriteLog.Write("方法:getSWOrders异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception.ToString());
                }
            }
        }
        #endregion

        #region QueryThread

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

                        QueryAssignmentDatagirdView(queryOrder);

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
                            queryOrder = QueryOrder(order);
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

                    QueryAssignmentDatagirdView(queryOrder);
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

        Order QueryOrder(object obj)
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

            foreach (Control c in this.ChargeClassPanel.Controls)
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

            foreach (Control c in this.ChargeClassPanel.Controls)
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

        #endregion

        #region View

        delegate void DelAssignmentDatagirdView(Order order);
        void AssignmentDatagirdView(Order order)
        {
            try
            {
                this.BeginInvoke(new System.Threading.ThreadStart(delegate ()
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


        #region View

        delegate void QueryDelAssignmentDatagirdView(Order order);
        void QueryAssignmentDatagirdView(Order order)
        {
            try
            {
                this.BeginInvoke(new System.Threading.ThreadStart(delegate ()
                {
                    bool isNew = true;
                    foreach (DataGridViewRow item in dgOrderQuery.Rows)
                    {
                        if (dgOrderQuery.Rows[item.Index].Cells["OrderInsideID"].Value.Equals(order.OrderInsideID))
                        {
                            isNew = false;
                            dgOrderQuery.Rows[item.Index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
                            dgOrderQuery.Rows[item.Index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
                            dgOrderQuery.Rows[item.Index].Cells["RechargeMsg"].Value = order.RechargeMsg;
                            dgOrderQuery.Rows[item.Index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
                            dgOrderQuery.Rows[item.Index].Cells["EndDatetime"].Value = order.EndDatetime;
                            dgOrderQuery.Rows[item.Index].Cells["IsNotify"].Value = order.IsNotify == false ? "未通知" : "通知成功";
                            QuerySetGirdRowColor(item);
                            break;
                        }
                    }

                    if (isNew)
                    {
                        QueryAddDataGridRows(order);
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
            //    this.Invoke(new QueryDelAssignmentDatagirdView(AssignmentDatagirdView), new object[] { order });
            //}
            //else
            //{
            //    try
            //    {
            //        bool isNew = true;
            //        foreach (DataGridViewRow item in dgOrderQuery.Rows)
            //        {
            //            if (dgOrderQuery.Rows[item.Index].Cells["OrderInsideID"].Value.Equals(order.OrderInsideID))
            //            {
            //                isNew = false;
            //                dgOrderQuery.Rows[item.Index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
            //                dgOrderQuery.Rows[item.Index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
            //                dgOrderQuery.Rows[item.Index].Cells["RechargeMsg"].Value = order.RechargeMsg;
            //                dgOrderQuery.Rows[item.Index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
            //                dgOrderQuery.Rows[item.Index].Cells["EndDatetime"].Value = order.EndDatetime;

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
        void QueryAddDataGridRows(Order order)
        {
            int index = this.dgOrderQuery.Rows.Add();

            dgOrderQuery.Rows[index].Cells["OrderInsideID"].Value = order.OrderInsideID;
            dgOrderQuery.Rows[index].Cells["OrderExternalID"].Value = order.OrderExternalID;
            dgOrderQuery.Rows[index].Cells["ProductName"].Value = order.ProductName;
            dgOrderQuery.Rows[index].Cells["ProductParValue"].Value = order.ProductParValue;
            dgOrderQuery.Rows[index].Cells["TargetAccount"].Value = order.TargetAccount;
            dgOrderQuery.Rows[index].Cells["BuyAmount"].Value = order.BuyAmount;
            dgOrderQuery.Rows[index].Cells["GameName"].Value = order.GameName;
            dgOrderQuery.Rows[index].Cells["AreaName"].Value = order.AreaName;
            dgOrderQuery.Rows[index].Cells["ServerName"].Value = order.ServerName;
            dgOrderQuery.Rows[index].Cells["StartDatetime"].Value = order.StartDatetime;
            dgOrderQuery.Rows[index].Cells["RechargeStatus"].Value = EnumService.GetDescription((int)order.RechargeStatus);
            dgOrderQuery.Rows[index].Cells["IsNotify"].Value = order.IsNotify == false ? "未通知" : "通知成功";
            dgOrderQuery.Rows[index].Cells["SuccessfulAmount"].Value = order.SuccessfulAmount;
            dgOrderQuery.Rows[index].Cells["RechargeMsg"].Value = order.RechargeMsg;
            dgOrderQuery.Rows[index].Cells["ChargeAccountInfo"].Value = order.ChargeAccountInfo;
            dgOrderQuery.Rows[index].Cells["EndDatetime"].Value = order.EndDatetime;

            while (dgOrderQuery.Rows.Count > 20)
            {
                dgOrderQuery.Rows.RemoveAt(0);
            }
        }
        void QueryBindingDataGridColumns()
        {
            dgOrderQuery.AllowUserToAddRows = false;
            dgOrderQuery.AllowUserToDeleteRows = false;

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



            dgOrderQuery.Columns.Add(c0);
            dgOrderQuery.Columns.Add(c1);
            dgOrderQuery.Columns.Add(c2);
            dgOrderQuery.Columns.Add(c22);
            dgOrderQuery.Columns.Add(c3);
            dgOrderQuery.Columns.Add(c4);
            dgOrderQuery.Columns.Add(c9);
            dgOrderQuery.Columns.Add(c99);
            dgOrderQuery.Columns.Add(c11);
            dgOrderQuery.Columns.Add(c111);
            dgOrderQuery.Columns.Add(c12);
            dgOrderQuery.Columns.Add(c5);
            dgOrderQuery.Columns.Add(c6);
            dgOrderQuery.Columns.Add(c7);
            dgOrderQuery.Columns.Add(c8);
            dgOrderQuery.Columns.Add(c13);
        }
        void QuerySetGirdRowColor(DataGridViewRow row)
        {
            switch (dgOrderQuery.Rows[row.Index].Cells["RechargeStatus"].Value.ToString())
            {
                case "处理中":
                    dgOrderQuery.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Blue;
                    break;
                case "充值成功":
                    dgOrderQuery.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Green;
                    break;
                case "充值失败":
                    dgOrderQuery.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Red;
                    break;
                case "充值存疑":
                    dgOrderQuery.Rows[row.Index].Cells["RechargeStatus"].Style.ForeColor = System.Drawing.Color.Violet;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 数网参数加载
        /// </summary>
        void loadSW()
        {
            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SW).FirstOrDefault();

            if (clientConfig != null)
            {
                this.txtSWUserCode.Text = clientConfig.MerchantID;
                this.txtSWKey.Text = clientConfig.MerchantKey;
                this.txtSWGetOrderUrl.Text = clientConfig.GetOrdersURL;
                this.txtSWNotifyUrl.Text = clientConfig.NotifyOrderURL;
                this.txtSWGetOrderCount.Text = clientConfig.GetOrderCount.ToString();
                this.txtSWGetOrderTime.Text = clientConfig.GetOrderTime.ToString();
                this.txtSWDescription.Text = clientConfig.Description;
            }
        }

        /// <summary>
        /// 保存数网参数
        /// </summary>
        void saveSW()
        {
            bool isNew = false;
            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SW).FirstOrDefault();

            if (clientConfig == null)
            {
                isNew = true;
                clientConfig = new ClientConfig();
                clientConfig.CreateTime = DateTime.Now;
            }

            clientConfig.MerchantID = this.txtSWUserCode.Text;
            clientConfig.MerchantKey = this.txtSWKey.Text;
            clientConfig.GetOrdersURL = this.txtSWGetOrderUrl.Text;
            clientConfig.NotifyOrderURL = this.txtSWNotifyUrl.Text;
            clientConfig.GetOrderCount = string.IsNullOrEmpty(this.txtSWGetOrderCount.Text) ? 0 : Convert.ToInt32(this.txtSWGetOrderCount.Text);
            clientConfig.GetOrderTime = string.IsNullOrEmpty(this.txtSWGetOrderTime.Text) ? 0 : Convert.ToInt32(this.txtSWGetOrderTime.Text);
            clientConfig.Description = this.txtSWDescription.Text;
            clientConfig.UpdateTime = DateTime.Now;

            if (clientConfig.CreateTime == null)
                clientConfig.CreateTime = DateTime.Now;

            clientConfig.MerchantCode = MerchantCodeType.SW;
            if (isNew)
            {
                if (SQLClientConfig.AddClientConfig(clientConfig))
                    MessageBox.Show("保存成功");
            }
            else
            {
                if (SQLClientConfig.UpdateClientConfig(clientConfig))
                    MessageBox.Show("修改成功");
            }
        }

        /// <summary>
        /// 设置数网参数控件
        /// </summary>
        /// <param name="isEnabled"></param>
        void setSWEnabled(bool isEnabled)
        {
            this.txtSWUserCode.Enabled = isEnabled;
            this.txtSWKey.Enabled = isEnabled;
            this.txtSWGetOrderUrl.Enabled = isEnabled;
            this.txtSWNotifyUrl.Enabled = isEnabled;
            this.txtSWGetOrderCount.Enabled = isEnabled;
            this.txtSWGetOrderTime.Enabled = isEnabled;
            this.txtSWDescription.Enabled = isEnabled;

            this.btnSaveSW.Enabled = isEnabled;
        }

        /// <summary>
        /// 保存产品查询设置
        /// </summary>
        void SaveProduct()
        {
            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.IsEnable == true);
            List<ChargeClass> updateList = new List<ChargeClass>();
            foreach (Control c in this.ChargeClassPanel.Controls)
            {
               var charge= chargeClassLst.Where(n => n.ChargeClassID == int.Parse((c as CheckBox).Name) && n.IsUsed != (c as CheckBox).Checked).FirstOrDefault();

                if (charge!=null )
                {
                    charge.IsUsed = (c as CheckBox).Checked;
                    updateList.Add(charge);
                }
            }
            if (updateList.Any())
            {
                SQLChargeClass.UpdateChargeClasssList(updateList);
            }
        }


        /// <summary>
        /// 产品查询设置
        /// </summary>
        void loadProductView()
        {
            List<ChargeClass> chargeClassLst = SQLChargeClass.GetChargeClasss(p => p.IsEnable ==true);

            int i = 1; int J = 1;
            foreach (ChargeClass item in chargeClassLst)
            {
                CheckBox chbox = new CheckBox();
                int x = 30 * i + ((i - 1) * 100);
                int y = 30 * J;
                chbox.Location = new System.Drawing.Point(x, y);
                chbox.Text = item.Descrtion;
                chbox.Size = new System.Drawing.Size(100, 16);
                chbox.Checked = item.IsUsed ?? false;
                chbox.Name = item.ChargeClassID.ToString();
                this.ChargeClassPanel.Controls.Add(chbox);

                i++;
                if (i == 10)
                {
                    J++;
                    i = 1;
                }
            }
        }

        /// <summary>
        /// 设置控件是否启用
        /// </summary>
        /// <param name="IsEnabled"></param>
        void SetProductCheckBoxEnabled(bool IsEnabled)
        {
            foreach (Control c in this.ChargeClassPanel.Controls)
            {
                if (c is CheckBox)
                {
                    (c as CheckBox).Enabled = IsEnabled;
                }
            }
        }

       
    }
}
