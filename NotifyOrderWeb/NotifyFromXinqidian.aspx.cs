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
    public partial class NotifyFromXinqidian : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "f8f573fd1308fe8080c2201480580c0b";

            string UserId = Request["UserId"]; //	用户编号
            string UserOrderId = Request["UserOrderId"]; //	用户订单号
            string OrderId = Request["OrderId"]; //	平台订单号
            string TimeStamp = Request["TimeStamp"]; //	时间戳
            string Sign = Request["Sign"]; //	签名字符串
            string Status = Request["Status"];  //订单状态  1：成功  2：失败



            WriteLog.Write("方法:Xinqidian回调，UserOrderId：" + UserOrderId + " OrderId:" + OrderId +
                "result:" + Status, LogPathFile.Other.ToString());

            string md5str = "orderid" + OrderId + "status" + Status + "timestamp" + TimeStamp + "userid" + UserId + "userorderid" + UserOrderId + "key" + key;

            string checkSign = Md5Helper.GetMD5String_Default(md5str);

            if (Sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (Status.Equals("1"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful) + OrderId;

                    new SQLOrder().UpdateNotifyOrderBySql(UserOrderId, RechargeStatus, RechargeMsg);
                }
                else if (Status.Equals("2"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure) + OrderId;
                    new SQLOrder().UpdateNotifyOrderBySql(UserOrderId, RechargeStatus, RechargeMsg);
                }
                else 
                {//充值中

                }
            }
            else
            {
                WriteLog.Write("方法:Xinqidian回调 本地系统订单号:" + UserOrderId +
              "签名验证错误，Xinqidian数字签名" + Sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("ok"); 
        }
    }
}