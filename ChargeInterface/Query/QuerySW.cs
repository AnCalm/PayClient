using ChargeInterface.AntoInterface;
using ChargeInterface.SW;
using Common;
using Common.LogHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;


namespace ChargeInterface.Query
{
    public class QuerySW : IQuery
    {
        public Order Query(Order order)
        {
            string MerchantID = PayAndQuerySW.merchantID;
            string key = PayAndQuerySW.key;
            string QueryUrl = PayAndQuerySW.QueryUrl;

            CookieContainer coockie = new CookieContainer();

            string strmd5 = MerchantID + order.OrderInsideID + key;
            string sign = Md5Helper.MD5Encrypt(strmd5);
            StringBuilder str = new StringBuilder();
            str.AppendFormat("MerchantID={0}", MerchantID); //商家编号
            str.AppendFormat("&MerchantOrderID={0}", order.OrderInsideID); //商家订单编号
            str.AppendFormat("&Sign={0}", sign);  //数字签名


            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " SW 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " SW 订单查询:" + result, LogPathFile.Recharge.ToString());

            string state = Regex.Match(result, @"<state>(.*?)</state>").Groups[1].Value;
            string swOrderId = Regex.Match(result, @"<order-id>(.*?)</order-id>").Groups[1].Value;
            string swStateInfo = Regex.Match(result, @"<state-info>(.*?)</state-info>").Groups[1].Value;


            if (state.Equals("101"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
                order.RechargeMsg = swStateInfo + "-" + swOrderId;
            }
            else if (state.Equals("201") || state.Equals("202") || state.Equals("203") || state.Equals("301") || state.Equals("302") || state.Equals("304") ||
                state.Equals("305") || state.Equals("306") || state.Equals("307") || state.Equals("401") || state.Equals("405") || state.Equals("501"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = swStateInfo + "-" + swOrderId;

            }
            else if (state.Equals("601"))
            {
                int status = getOrderStatus(swStateInfo, "601");
                order.RechargeStatus = status;
                order.RechargeMsg = swStateInfo;

                //if (swStateInfo.Contains("帐号错误") || swStateInfo.Contains("充值失败：玩家账号不存在") || swStateInfo.Contains("充值失败：玩家账号不存在")
                //    || swStateInfo.Contains("批价失败，你的帐号无法存入Q币!失败原因:单日累计存款金额超过系统限额") || swStateInfo.Contains("交易失败")
                //    || swStateInfo.Contains("您输入的游戏账号无法充值")|| swStateInfo.Contains("充值已达每日上限")|| swStateInfo.Contains("所选金额会超出每日限额"))
                //{
                //    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                //    order.RechargeMsg = swStateInfo;
                //}
                //else
                //{
                //    order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                //    order.RechargeMsg = swStateInfo;
                //}

            }
            else if (state.Equals("102"))
            {//充值中

            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious) + "-" + swOrderId;
            }

            return order;
        }

        public int getOrderStatus(string msg, string msgCode)
        {
            if (string.IsNullOrEmpty(msg))
                return 4;  //可疑

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "SWOrderStatus.xml");    //加载Xml文件 
            XmlElement root = doc.DocumentElement;   //获取根节点 
            XmlNodeList StatusNodes = root.GetElementsByTagName("Status"); //获取Status子节点集合 
            foreach (XmlNode node in StatusNodes)
            {
                string id = ((XmlElement)node).GetAttribute("id");   //获取id属性值 
                if (id.Equals(msgCode))
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        string code = ((XmlElement)childNode).InnerText;

                        int status = Convert.ToInt16(((XmlElement)childNode).GetAttribute("value"));

                        if (msg.Contains(code))
                            return status;
                    }
                }
            }

            return 4;
        }
    }
}
