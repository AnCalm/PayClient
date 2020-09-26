using DBOperation.SQLHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EntityDB;
using Common;
using Common.LogHelper;
using System.Text;

namespace NotifyOrderWeb
{
    public partial class NotifyFormMBJ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "n6ln6ikodhiuavy";

            string OrderID = Request["OrderID"]; //服务端订单编号
            string MerchantOrderID = Request["MerchantOrderID"];   //商家订单编号
            string State = Request["State"];   //订单状态
            string StateInfo = Request["StateInfo"]; //充值描述
            string Sign = Request["Sign"];  //签名

            WriteLog.Write("方法:MBJ回调，OrderID：" + OrderID + " MerchantOrderID:" + MerchantOrderID +
                "State:" + State + " StateInfo:" + StateInfo, LogPathFile.Other.ToString());

            string md5str = OrderID + MerchantOrderID + State + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (Sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (State.Equals("101"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                else if (State.Equals("201") || State.Equals("202") || State.Equals("203") || State.Equals("301") || State.Equals("302") || State.Equals("304") ||
                    State.Equals("305") || State.Equals("306") || State.Equals("307") || State.Equals("401") || State.Equals("405") || State.Equals("501"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                }
                else if (State.Equals("601"))
                {
                    int status = new ChargeInterface.Query.QueryMBJ().getOrderStatus(StateInfo, "601");
                    RechargeStatus = status;
                }
                else if (State.Equals("102"))
                {//充值中

                }
                else
                {
                    RechargeStatus = (int)OrderRechargeStatus.suspicious;
                }


                RechargeMsg = StateInfo + "-" + OrderID;
                new SQLOrder().UpdateNotifyOrderBySql(MerchantOrderID, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:MBJ回调，MBJ订单号：" + OrderID + " 本地系统订单号:" + MerchantOrderID +
              "签名验证错误，Kamen数字签名" + Sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("OK");
        }
    }
}