using ChargeInterface.SUP;
using ChargeInterface.SW;
using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
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

namespace GetOrder
{
    public partial class FrmGetOrders : Form
    {
        bool isGetSWOrdersThreadStatus = false;
        bool isGetSUPOrdersThreadStatus = false;

        Thread GetSWOrdersThread;
        Thread GetSUPOrdersThread;

        public FrmGetOrders()
        {
            InitializeComponent();
        }

        #region Events

        private void FrmGetOrders_Load(object sender, EventArgs e)
        {
            this.btnStopSW.Enabled = false;
            this.btnStopSUP.Enabled = false;
            this.btnStopAll.Enabled = false;

            loadSW();
            loadSUP();
        }

        private void btnStartSUP_Click(object sender, EventArgs e)
        {
            if (GetSUPOrdersThread != null && GetSUPOrdersThread.IsAlive)
            {
                MessageBox.Show("操作过于频繁，请稍等");
            }
            else
            {
                this.btnStartSUP.Enabled = false;
                this.btnStopSUP.Enabled = true;
                setSUPEnabled(false);
                setAllbtn();

                isGetSUPOrdersThreadStatus = true;
                startGetSUPOrdersThread();
            }
        }

        private void btnStopSUP_Click(object sender, EventArgs e)
        {
            this.btnStartSUP.Enabled = true;
            this.btnStopSUP.Enabled = false;
            setSUPEnabled(true);
            setAllbtn();

            isGetSUPOrdersThreadStatus = false;
        }

        private void btnStartSW_Click(object sender, EventArgs e)
        {
            if (GetSWOrdersThread != null && GetSWOrdersThread.IsAlive)
            {
                MessageBox.Show("操作过于频繁，请稍等");
            }
            else
            {
                this.btnStartSW.Enabled = false;
                this.btnStopSW.Enabled = true;
                setSWEnabled(false);
                setAllbtn();


                isGetSWOrdersThreadStatus = true;

                startSWGetordersThread();
            }
        }

        private void btnStopSW_Click(object sender, EventArgs e)
        {
            this.btnStartSW.Enabled = true;
            this.btnStopSW.Enabled = false;
            setSWEnabled(true);
            setAllbtn();

            isGetSWOrdersThreadStatus = false;
        }

        private void btnStartAll_Click(object sender, EventArgs e)
        {
            if ((GetSWOrdersThread != null && GetSWOrdersThread.IsAlive)
                || (GetSUPOrdersThread != null && GetSUPOrdersThread.IsAlive))
            {
                MessageBox.Show("操作过于频繁，请稍等");
            }
            else
            {
                this.btnStartSUP.Enabled = false;
                this.btnStopSUP.Enabled = true;
                setSUPEnabled(false);

                this.btnStartSW.Enabled = false;
                this.btnStopSW.Enabled = true;
                setSWEnabled(false);

                this.btnStartAll.Enabled = false;
                this.btnStopAll.Enabled = true;

                if (isGetSWOrdersThreadStatus == false)
                {
                    isGetSWOrdersThreadStatus = true;
                    startSWGetordersThread();
                }

                if (isGetSUPOrdersThreadStatus == false)
                {
                    isGetSUPOrdersThreadStatus = true;
                    startGetSUPOrdersThread();
                }
            }
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            this.btnStartSUP.Enabled = true;
            this.btnStopSUP.Enabled = false;
            setSUPEnabled(true);

            this.btnStartSW.Enabled = true;
            this.btnStopSW.Enabled = false;
            setSWEnabled(true);

            this.btnStartAll.Enabled = true;
            this.btnStopAll.Enabled = false;

            isGetSWOrdersThreadStatus = false;
            isGetSUPOrdersThreadStatus = false;
        }

        private void btnSaveSW_Click(object sender, EventArgs e)
        {
            saveSW();
        }

        private void btnSaveSUP_Click(object sender, EventArgs e)
        {
            saveSUP();
        }

        private void FrmGetOrders_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void FrmGetOrders_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isGetSUPOrdersThreadStatus || isGetSWOrdersThreadStatus)
            {
                MessageBox.Show("请先停止充值再关闭程序！");
                e.Cancel = true;
                return;
            }
        }

        #endregion

        void startSWGetordersThread()
        {
            if (GetSWOrdersThread != null)
            {
                while (GetSWOrdersThread.IsAlive)
                    Thread.Sleep(3000);
                GetSWOrdersThread.Abort();
            }
            GetSWOrdersThread = new Thread(new ThreadStart(getSWOrders));
            GetSWOrdersThread.SetApartmentState(System.Threading.ApartmentState.STA);
            GetSWOrdersThread.Start();
        }

        void startGetSUPOrdersThread()
        {
            if (GetSUPOrdersThread != null)
            {
                while (GetSUPOrdersThread.IsAlive)
                    Thread.Sleep(3000);
                GetSUPOrdersThread.Abort();
            }
            GetSUPOrdersThread = new Thread(new ThreadStart(GetSUPOrders));
            GetSUPOrdersThread.SetApartmentState(System.Threading.ApartmentState.STA);
            GetSUPOrdersThread.Start();
        }

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

        void setSUPEnabled(bool isEnabled)
        {
            this.txtSUPUserCode.Enabled = isEnabled;
            this.txtSUPKey.Enabled = isEnabled;
            this.txtSUPGetOrderUrl.Enabled = isEnabled;
            this.txtSUPNotifyUrl.Enabled = isEnabled;
            this.txtSUPGetOrderCount.Enabled = isEnabled;
            this.txtSUPGetOrderTime.Enabled = isEnabled;
            this.txtSUPDescription.Enabled = isEnabled;

            this.btnSaveSUP.Enabled = isEnabled;
        }

        void setAllbtn()
        {
            if (!btnStartSUP.Enabled && !btnStartSW.Enabled)
                this.btnStartAll.Enabled = false;
            else
                this.btnStartAll.Enabled = true;

            if (!btnStopSUP.Enabled && !btnStopSW.Enabled)
                this.btnStopAll.Enabled = false;
            else
                this.btnStopAll.Enabled = true;
        }
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
        void loadSUP()
        {
            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SUP).FirstOrDefault();

            if (clientConfig != null)
            {
                this.txtSUPUserCode.Text = clientConfig.MerchantID;
                this.txtSUPKey.Text = clientConfig.MerchantKey;
                this.txtSUPGetOrderUrl.Text = clientConfig.GetOrdersURL;
                this.txtSUPNotifyUrl.Text = clientConfig.NotifyOrderURL;
                this.txtSUPGetOrderCount.Text = clientConfig.GetOrderCount.ToString();
                this.txtSUPGetOrderTime.Text = clientConfig.GetOrderTime.ToString();
                this.txtSUPDescription.Text = clientConfig.Description;
            }
        }
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
        void saveSUP()
        {
            bool isNew = false;

            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SUP).FirstOrDefault();

            if (clientConfig == null)
            {
                isNew = true;
                clientConfig = new ClientConfig();
                clientConfig.CreateTime = DateTime.Now;

            }

            clientConfig.MerchantID = this.txtSUPUserCode.Text;
            clientConfig.MerchantKey = this.txtSUPKey.Text;
            clientConfig.GetOrdersURL = this.txtSUPGetOrderUrl.Text;
            clientConfig.NotifyOrderURL = this.txtSUPNotifyUrl.Text;
            clientConfig.GetOrderCount = string.IsNullOrEmpty(this.txtSUPGetOrderCount.Text) ? 0 : Convert.ToInt32(this.txtSUPGetOrderCount.Text);
            clientConfig.GetOrderTime = string.IsNullOrEmpty(this.txtSUPGetOrderTime.Text) ? 0 : Convert.ToInt32(this.txtSUPGetOrderTime.Text);
            clientConfig.Description = this.txtSUPDescription.Text;
            clientConfig.UpdateTime = DateTime.Now;

            if (clientConfig.CreateTime == null)
                clientConfig.CreateTime = DateTime.Now;

            clientConfig.MerchantCode = MerchantCodeType.SUP;

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

        #region Thread

        void getSWOrders()
        {
            while (isGetSWOrdersThreadStatus)
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

        void GetSUPOrders()
        {
            while (isGetSUPOrdersThreadStatus)
            {
                try
                {
                    int time = 0;

                    WriteLog.Write("方法:GetSUPOrders 开始取单：" + DateTime.Now.ToString(), LogPathFile.Other);

                    List<Order> orderSet = new GetAndNotifySUPOrders().getOrderFromSUP(ref time);

                    WriteLog.Write("方法:GetSUPOrders 取到订单：" + DateTime.Now.ToString(), LogPathFile.Other);

                    if (orderSet != null && orderSet.Count > 0)
                    {
                        bool result = SQLOrder.AddOrder(orderSet);
                        int Recount = 0;
                        while (!result)
                        {
                            if (Recount > 10)
                            {
                                break;
                            }
                            result = SQLOrder.AddOrder(orderSet);
                            Recount++;
                            Thread.Sleep(1 * 1000);
                        }

                        WriteLog.Write("方法:GetSUPOrders 订单保存成功：" + DateTime.Now.ToString(), LogPathFile.Other);

                        //foreach (Order order in orderSet)
                        //{
                        //    bool result = SQLOrder.AddOrder(order);
                        //    int Recount = 0;
                        //    while (!result)
                        //    {
                        //        if (Recount > 10)
                        //        {
                        //            break;
                        //        }
                        //        result = SQLOrder.AddOrder(order);
                        //        Recount++;
                        //        Thread.Sleep(1 * 1000);
                        //    }
                        //}
                    }
                    Thread.Sleep(time * 1000);
                }
                catch (Exception ex)
                {
                    WriteLog.Write("方法:GetSUPOrders异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception.ToString());
                }
            }
        }

        #endregion
    }
}
