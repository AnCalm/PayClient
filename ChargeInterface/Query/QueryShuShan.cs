using ChargeInterface.AntoInterface;
using ChargeInterface.ShuShan;
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
    public class QueryShuShan : IQuery
    {
        public Order Query(Order order)
        {
            int num = 0;

            string MerchantID = PayAndQueryShuShan.merchantID;
            string key = PayAndQueryShuShan.key;
            string QueryUrl = PayAndQueryShuShan.queryUrl;

            while (num < 5)
            {
                CookieContainer coockie = new CookieContainer();

                string strmd5 = MerchantID + order.OrderInsideID + key;
                string sign = Md5Helper.MD5Encrypt(strmd5);
                StringBuilder str = new StringBuilder();
                str.AppendFormat("MerchantID={0}", MerchantID); //商家编号
                str.AppendFormat("&MerchantOrderID={0}", order.OrderInsideID); //商家订单编号
                str.AppendFormat("&Sign={0}", sign);  //数字签名


                WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " ShuShan 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

                string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

                WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " ShuShan 订单查询:" + result, LogPathFile.Recharge.ToString());

                string state = Regex.Match(result, @"<state>(.*?)</state>").Groups[1].Value;
                string ShuShanOrderId = Regex.Match(result, @"<order-id>(.*?)</order-id>").Groups[1].Value;
                string ShuShanStateInfo = Regex.Match(result, @"<state-info>(.*?)</state-info>").Groups[1].Value;


                if (state.Equals("101"))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                    order.RechargeMsg = ShuShanStateInfo + "-" + ShuShanOrderId;
                    break;
                }
                else if (state.Equals("201") || state.Equals("202") || state.Equals("203") || state.Equals("301") || state.Equals("302") || state.Equals("304") ||
                    state.Equals("305") || state.Equals("306") || state.Equals("307") || state.Equals("401") || state.Equals("405") || state.Equals("501"))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = ShuShanStateInfo + "-" + ShuShanOrderId;
                    break;

                }
                else if (state.Equals("601"))
                {
                    int status = getOrderStatus(ShuShanStateInfo, "601");
                    order.RechargeStatus = status;
                    order.RechargeMsg = ShuShanStateInfo + "-" + ShuShanOrderId;
                    break;
                }
                else if (state.Equals("102"))
                {//充值中

                }
                else
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                    order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious) + "-" + ShuShanOrderId;
                    break;
                }

                num++;

                System.Threading.Thread.Sleep(2 * 1000);
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
