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
using Newtonsoft.Json;

namespace NotifyOrderWeb
{
    public partial class NotifyFromFulu2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "e017b189ab704546b8cad3ba916592f4";
           
            string order_id = Request["order_id"]; //	String	是	19072426795315681646	福禄开放平台订单号
            string charge_finish_time = Request["charge_finish_time"]; //	String	是	2019-07-24 16:46:14	交易完成时间，格式为：yyyy-MM-dd HH:mm:ss
            string customer_order_no = Request["customer_order_no"]; //	String	是	535126603	合作商家订单号
            string order_status = Request["order_status"]; //	String	是	success、failed	订单状态
            string recharge_description = Request["recharge_description"]; //	String	是	充值成功	充值描述
            string product_id = Request["product_id"]; //	String	是	10000497	商品Id
            string price = Request["price"]; //	String	是	1.0000	交易单价
            string buy_num = Request["buy_num"]; //	String	是	2	购买数量
            string operator_serial_number = Request["operator_serial_number"]; //	String	是		运营商流水号
            string sign = Request["sign"]; //String	是	b716ea73535df393d8ce8efae7518b70	签名


            WriteLog.Write("方法:Fulu回调，order_id：" + order_id + " charge_finish_time:" + charge_finish_time +
                "customer_order_no:" + customer_order_no + " order_status:" + order_status + " recharge_description:" + recharge_description
                + " product_id:" + product_id + " price:" + price + " buy_num:" + buy_num + " operator_serial_number:" + operator_serial_number, LogPathFile.Other.ToString());


            #region  MD5加密
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("order_id", order_id);
            dictionary.Add("charge_finish_time", charge_finish_time);
            dictionary.Add("customer_order_no", customer_order_no);
            dictionary.Add("order_status", order_status);
            dictionary.Add("recharge_description", recharge_description);
            dictionary.Add("product_id", product_id);
            dictionary.Add("price", price);
            dictionary.Add("buy_num", buy_num);
            dictionary.Add("operator_serial_number", operator_serial_number);

            string jsonData = JavaScriptConvert.SerializeObject(dictionary);

            var chars = jsonData.ToCharArray();
            Array.Sort(chars);

            string data = new string(chars) + key;

            string checkSign = Md5Helper.Md5_Fulu(data).ToLower();
            #endregion


            if (sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.Submit;
                string RechargeMsg = recharge_description + order_id;

                if (order_status == "success")
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                else if (order_status == "failed")
                    RechargeStatus = (int)OrderRechargeStatus.failure;
         
                if (RechargeStatus !=(int)OrderRechargeStatus.Submit)
                {
                    new SQLOrder().UpdateNotifyOrderBySql(customer_order_no, RechargeStatus, RechargeMsg);
                }
            }
            else
            {
                WriteLog.Write("方法:Fulu回调，Fulu订单号：" + order_id + " 本地系统订单号:" + customer_order_no +
              "签名验证错误，Fulu数字签名" + sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }

            Response.Write("success");
        }
    }
}