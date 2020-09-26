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
    public partial class NotifyFromSW : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "y7y8hihweqcb408";

            string OrderID = Request["OrderID"]; //服务端订单编号
            string MerchantOrderID = Request["MerchantOrderID"];   //商家订单编号
            string State = Request["State"];   //订单状态
            string StateInfo = Request["StateInfo"]; //充值描述
            string Sign = Request["Sign"];  //签名

            WriteLog.Write("方法:SW回调，OrderID：" + OrderID + " MerchantOrderID:" + MerchantOrderID +
                "State:" + State + " StateInfo:" + StateInfo, LogPathFile.Other.ToString());

            string md5str = OrderID + MerchantOrderID + State + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (Sign== checkSign)
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
                    int status = new ChargeInterface.Query.QuerySW().getOrderStatus(StateInfo, "601");
                    RechargeStatus = status;

                    //if (StateInfo.Contains("帐号错误") || StateInfo.Contains("充值失败：玩家账号不存在") || StateInfo.Contains("区域库存不足")
                    //    || StateInfo.Contains("批价失败，你的帐号无法存入Q币!失败原因:单日累计存款金额超过系统限额") || StateInfo.Contains("交易失败")
                    //    || StateInfo.Contains("您输入的游戏账号无法充值")|| StateInfo.Contains("充值已达每日上限")|| StateInfo.Contains("所选金额会超出每日限额"))
                    //{
                    //    RechargeStatus = (int)OrderRechargeStatus.failure;
                    //}
                    //else
                    //{
                    //    RechargeStatus = (int)OrderRechargeStatus.suspicious;
                    //}
                    

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
                WriteLog.Write("方法:SW回调，SW订单号：" + OrderID + " 本地系统订单号:" + MerchantOrderID +
              "签名验证错误，Kamen数字签名" + Sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("OK"); 
        }
    }
}