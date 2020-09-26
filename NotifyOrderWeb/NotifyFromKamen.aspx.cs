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
    public partial class NotifyFromKamen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "918BB8A545017EB8B5E26D9F62AD33E1";

            string ChargeTime = Request["ChargeTime"]; //充值时间
            string CustomerOrderNo = Request["CustomerOrderNo"];   //客户外部系统订单号
            string OrderNo = Request["OrderNo"];   //卡门网系统订单号
            string ReMark = Request["ReMark"]; //充值描述
            string Status = Request["Status"];  //充值状态（True/False）
            string Sign = Request["Sign"];  //签名

            WriteLog.Write("方法:Kamen回调，ChargeTime：" + ChargeTime + " CustomerOrderNo:" + CustomerOrderNo +
                "OrderNo:" + OrderNo + " ReMark:" + ReMark + " Status:" + Status, LogPathFile.Other.ToString());


            string sData = "chargetime=" + ChargeTime.Replace("/", "-") + "&customerorderno=" + CustomerOrderNo + "&orderno=" + OrderNo + "&remark="
                + ReMark + "&status=" + Status;
            string checkSign = Md5Helper.EncryptMd5_Kamen1(sData+key);

            if (Sign== checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = ReMark + OrderNo;

                if (Status == "True")
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                else
                    RechargeStatus = (int)OrderRechargeStatus.failure;

                new SQLOrder().UpdateNotifyOrderBySql(CustomerOrderNo, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:Kamen回调，Kamen订单号：" + OrderNo + " 本地系统订单号:" + CustomerOrderNo +
              "签名验证错误，Kamen数字签名" + Sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("True"); 
        }
    }
}