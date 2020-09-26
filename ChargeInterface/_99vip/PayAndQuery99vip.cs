using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using Common;
using Common.LogHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DBOperation.SQLHelper;
using System.Xml;

namespace ChargeInterface._99vip
{
    public class PayAndQuery99vip
    {
        public const string submitUrl = "http://a.99vip.cn/Api/Pay";
        public const string queryUrl = "http://a.99vip.cn/Api/QueryOrder";
        public const string key = "1p98fswjw031rih";
        public const string merchantID = "10004";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order, "http://116.62.44.48/NotifyForm99vip.aspx", AreaValueType._99vip);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " 99vip 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " 99vip 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        public string setPostDate(Order order, string responseUrl, string iD)
        {
            string MerchantID = merchantID;	//商家编号	Y
            string MerchantOrderID = order.OrderInsideID;	//商家订单编号（不允许重复）	Y
            string ProductID = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);	//商品编号	Y
            string BuyAmount = order.BuyAmount.ToString();	//充值数量	Y
            string TargetAccount = order.TargetAccount;	//充值账户	N
            string TargetAccountType = System.Web.HttpUtility.UrlEncode(order.TargetAccountType, Encoding.UTF8);	//充值账户类型	N
            string TargetAccountTypeName = System.Web.HttpUtility.UrlEncode(order.TargetAccountTypeName, Encoding.UTF8);	//充值账户类型名称	N
            string RechargeMode = System.Web.HttpUtility.UrlEncode(order.RechargeMode, Encoding.UTF8);	//充值方式	N
            string RechargeModeName = System.Web.HttpUtility.UrlEncode(order.RechargeModeName, Encoding.UTF8);	//充值方式名称	N
            string Game = System.Web.HttpUtility.UrlEncode(order.Game, Encoding.UTF8);	//充值游戏	N
            string GameName = System.Web.HttpUtility.UrlEncode(order.GameName, Encoding.UTF8);	//充值游戏名称	N
            string Area = getAreaValue(order.AreaName, ProductID, iD);//游戏区域	N
            string AreaName = System.Web.HttpUtility.UrlEncode(order.AreaName, Encoding.UTF8);		//游戏区域名称	N
            string Server = System.Web.HttpUtility.UrlEncode(order.Server, Encoding.UTF8);	//游戏服务器	N
            string ServerName = System.Web.HttpUtility.UrlEncode(order.ServerName, Encoding.UTF8);	//游戏服务器名称	N
            string CustomerIP = order.CustomerIp;	//客户IP/客户区域，用来确定消费区域，只需传一个即可。如果都传以IP为准，如果都不传则默认区域为全国。	N
            string CustomerRegion = System.Web.HttpUtility.UrlEncode(order.CustomerRegion, Encoding.UTF8);
            string ResponseUrl = System.Web.HttpUtility.UrlEncode(responseUrl, Encoding.UTF8);	//接收异步通知订单状态的Url	N
            string Sign = string.Empty;//数字签名	Y

            string TargetAccount_Sign = order.TargetAccount;
            if (new ChargeInterface.SW.PayAndQuerySW().isChinese(TargetAccount))
                TargetAccount = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8);


            Game = new ChargeInterface.SW.PayAndQuerySW().getGameValue(order.GameName);//充值游戏

            if (order.GameName.Contains("魔域") || order.GameName.Contains("魔域掉钱版") || order.GameName.Contains("魔域口袋版")
               || order.GameName.Contains("机战") || order.GameName.Contains("征服"))
            {
               new ChargeInterface.SW.PayAndQuerySW().getmoyuAreaAndServer(order.GameName, order.AreaName, order.ServerName, ref Area, ref Server);

                switch (order.RechargeModeName)
                {
                    case "魔石卡":
                        RechargeMode = "1021101902000000001";
                        break;
                    case "神石卡":
                        RechargeMode = "1021372029000000001";
                        break;
                    case "太阳石卡":
                        RechargeMode = "1021111902000000001";
                        break;
                    default:
                        RechargeMode = "";
                        break;

                }

            }
            else
            {
                Area = new ChargeInterface.MBJ.PayAndQueryMBJ().getAreaValue(order.AreaName, ProductID, AreaValueType.ShuWang);
                Server = System.Web.HttpUtility.UrlEncode(order.Server, Encoding.UTF8);
            }


            string MD5Str = MerchantID + MerchantOrderID + ProductID + BuyAmount + TargetAccount_Sign + key;

            Sign = Md5Helper.MD5Encrypt(MD5Str);

            StringBuilder str = new StringBuilder();

            str.AppendFormat("MerchantID={0}", MerchantID);
            str.AppendFormat("&MerchantOrderID={0}", MerchantOrderID);
            str.AppendFormat("&ProductID={0}", ProductID);
            str.AppendFormat("&BuyAmount={0}", BuyAmount);
            str.AppendFormat("&TargetAccount={0}", TargetAccount);
            str.AppendFormat("&TargetAccountType={0}", TargetAccountType);
            str.AppendFormat("&TargetAccountTypeName={0}", TargetAccountTypeName);
            str.AppendFormat("&RechargeMode={0}", RechargeMode);
            str.AppendFormat("&RechargeModeName={0}", RechargeModeName);
            str.AppendFormat("&Game={0}", Game);
            str.AppendFormat("&GameName={0}", GameName);
            str.AppendFormat("&Area={0}", Area);
            str.AppendFormat("&AreaName={0}", AreaName);
            str.AppendFormat("&Server={0}", Server);
            str.AppendFormat("&ServerName={0}", ServerName);
            str.AppendFormat("&CustomerIP={0}", CustomerIP);
            str.AppendFormat("&CustomerRegion={0}", CustomerRegion);
            str.AppendFormat("&ResponseUrl={0}", ResponseUrl);
            str.AppendFormat("&Sign={0}", Sign);  // 签名
            return str.ToString();
        }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="gameName"></param>
      /// <param name="productid"></param>
        /// <param name="ID">默认值为 14 为 梦本捷</param>
      /// <returns></returns>
        public string getAreaValue(string areaName, string productid, string ID)
        {
            if (string.IsNullOrEmpty(areaName))
                return null;

            if (string.IsNullOrEmpty(productid))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "GameInfo.xml");    //加载Xml文件 
            XmlElement root = doc.DocumentElement;   //获取根节点 

            foreach (XmlNode node in doc.SelectNodes("/GameInfo/Merchant")) //获取所有 Merchant节点
            {
                string mid = ((XmlElement)node).GetAttribute("ID");   //获取 Merchant 节点的 ID 属性值 
                if (mid.Equals(ID))
                {
                    foreach (XmlNode item in node.ChildNodes) //获取 Product 节点
                    {
                        string pid = ((XmlElement)item).GetAttribute("ID");   //获取 Product 节点的 ID 属性值 

                        if (pid.Equals(productid))
                        {
                            foreach (XmlNode childitem in item.ChildNodes) //获取 Area 节点
                            {
                                string area = ((XmlElement)childitem).InnerText;
                                string areaValue = ((XmlElement)childitem).GetAttribute("value");   //获取 Area 节点的 ID 属性值 
                                if (area.Equals(areaName))
                                {
                                    return areaValue;
                                }
                            }
                        }
                    }
                }
            }


            return null;
        }
    }
}
