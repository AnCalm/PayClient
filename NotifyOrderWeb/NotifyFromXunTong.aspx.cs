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
    public partial class NotifyFromXunTong : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "ef61aa43fbcd42ab9ccc69722eba5698";

            string username = Request["username"]; //	接入代理用户名
            string result = Request["result"]; //	充值结果：success 成功，fail 失败 ，其他结果为未知需要人工核查
            string orderid = Request["orderid"]; //	平台订单号
            string sporderid = Request["sporderid"]; //	SP订单号,商户平台的订单号
            string sign = Request["sign"]; //	MD5组合数字签名方式：MD5(username={}&orderid={}&sporderid={}&result={}&key=APIkey)加密串均为小写,MD5输出为32位小写

            WriteLog.Write("方法:XunTong回调，sporderid：" + sporderid + " orderid:" + orderid +
                "result:" + result, LogPathFile.Other.ToString());

            string md5str = "username=" + username + "&orderid=" + orderid + "&sporderid=" + sporderid + "&result=" + result + "&key=" + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (result.Equals("success"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);
                }
                else if (result.Equals("fail"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                }
                else 
                {//充值中

                }

               new SQLOrder().UpdateNotifyOrderBySql(sporderid, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:XunTong回调 本地系统订单号:" + sporderid +
              "签名验证错误，Xuntong数字签名" + sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("ok"); 
        }
    }
}