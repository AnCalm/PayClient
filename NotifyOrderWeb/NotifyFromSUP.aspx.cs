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

namespace NotifyOrderWeb
{
    public partial class NotifyFromSUP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "75e2ae34888644289751ee1082538d2b";

            string SupOrderID = Request["SupOrderID"]; //系统内部订单号
            string OrderID = Request["OrderID"];   //下游自定义订单号
            string Status = Request["Status"];   //充值状态
            string DetailMsg = Request["DetailMsg"]; //供货商返回充值详情
            string Sign = Request["Sign"];  //数字签名 Md5(SupOrderID+OrderID+ Status+ key)
            string StatusNo = Request["StatusNo"];  //充值状态的编号 数字,跟上面的Status中文一一对应0:充值成功 1:充值失败 2:处理中 3:可疑订单
            string Sign2 = Request["Sign2"];  //另外一组数字签名  Md5(SupOrderID+OrderID+ StatusNo + key)
            string Cards = Request["Cards"];   //卡密类订单的卡密信息 类似这样的多组数据可以自行解析,[{'No':'2055029031001601','PassWord':'2010079641000701','ExpireTime':'2018-12-22   00:00:00'}]

            WriteLog.Write("方法:SUP回调，SUP订单号：" + SupOrderID + " 本地系统订单号:" + OrderID +
                "返回充值状态:" + Status + "-" + StatusNo + " 返回充值详情:" + DetailMsg, LogPathFile.Other.ToString());

            string md5str = SupOrderID + OrderID + StatusNo + key;
            string checkSign = Md5Helper.GetMD5String_utf8(md5str);

            if (Sign2 == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = DetailMsg + SupOrderID;

                switch (StatusNo)
                {
                    case "0":
                      RechargeStatus= (int)OrderRechargeStatus.successful;
                        break;
                    case "1":
                        RechargeStatus= (int)OrderRechargeStatus.failure;
                        break;
                    case "2":
                        RechargeStatus= (int)OrderRechargeStatus.processing;
                        break;
                    case "3":
                    default:
                        RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                }


                new SQLOrder().UpdateNotifyOrderBySql(OrderID, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:SUP回调，SUP订单号：" + SupOrderID + " 本地系统订单号:" + OrderID +
              "签名验证错误，SUP数字签名" + Sign2 + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("ok"); 
        }
    }
}