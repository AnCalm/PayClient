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
    public partial class NotifyFromZhiXin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "duZBIBf3XTNXcBw9VY5K6zMVxTAKBjNn";

            string client_order_no = Request["client_order_no"]; //	商户订单号	是	32
            string up_order_no = Request["up_order_no"]; //	充值平台订单号	是	32
            string product_type = Request["product_type"]; //	产品类型	        是	10
            string phone_no = Request["phone_no"]; //	充值号码	是	20
            string deduction_amount = Request["deduction_amount"]; //	折扣金额	是	20
            string recharge_status = Request["recharge_status"]; //	2:充值成功 6:充值失败	是	10
            string elecardID = Request["elecardID"]; //	运营商充值凭证	否	128
            string desc = Request["desc"]; //	失败描述(该字段不参与签名)使用UTF-8 URLDECODE编码	否	256
            string channel_type = Request["channel_type"]; //	渠道编码(该字段不参与签名)参考附录8	否	10
            string sign = Request["sign"]; //	数据签名	是	32



            WriteLog.Write("方法:ZhiXin回调，client_order_no：" + client_order_no + " up_order_no:" + up_order_no +
                "recharge_status:" + recharge_status, LogPathFile.Other.ToString());

            string md5str = "client_order_no" + client_order_no + "deduction_amount" + deduction_amount + "elecardID" + elecardID + "phone_no" + phone_no + "product_type" + product_type + "recharge_status" + recharge_status + "up_order_no" + up_order_no + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (recharge_status.Equals("2"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);
                }
                else if (recharge_status.Equals("6"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                }
                else 
                {//充值中

                }

                new SQLOrder().UpdateNotifyOrderBySql(client_order_no, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:ZhiXin回调 本地系统订单号:" + client_order_no +
              "签名验证错误，ZhiXin数字签名" + sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("SUCCESS"); 
        }
    }
}