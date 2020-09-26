using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ChargeInterface.Xinqidian
{
    public class PayAndQueryXinqidian
    {
        public const string submitUrl = "http://open.greatnesss.com/api/Order/CreateNetGameOrder ";
        public const string QueryUrl = "http://open.greatnesss.com/api/Order/QueryChargeResult";
        public const string key = "f8f573fd1308fe8080c2201480580c0b";
        public const string merchantID = "20057";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Xinqidian 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Xinqidian 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {

            string UserId = merchantID;	//用户编号	必选	String	是	
            string UserOrderId = order.OrderInsideID;	//用户订单号	必选	String	是	
            string ChargeAccount = order.TargetAccount;	//充值账号	必选	String	是	
            string BuyNum = order.BuyAmount.ToString();	//购买数量	必选	Int	是	
            string GoodsId = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);	//商品Id	必选	Int	是	
            string GameArea = System.Web.HttpUtility.UrlEncode(order.AreaName, Encoding.UTF8);	//游戏区	选填	String	否	游戏区服信息-游戏大区
            string GameServer = System.Web.HttpUtility.UrlEncode(order.ServerName, Encoding.UTF8);	//游戏服	选填	String	否	游戏区服信息-游戏服务器名称
            string PassCheck = "";	//游戏通行证	选填	String	否	针对某些有些需要传入通行证账号信息
            string Sign = "";	//签名字符串	必选	String	否	加密后的字符串



            string md5str = "buynum" + BuyNum + "chargeaccount" + ChargeAccount + "goodsid" + GoodsId
                +"userid" + UserId + "userorderid" + UserOrderId + "key" + key;

            WriteLog.Write("方法:setPostDate，订单号：" + order.OrderInsideID + " Xinqidian 机密参数:" + md5str.ToString(), LogPathFile.Recharge.ToString());

            Sign = Md5Helper.GetMD5String_Default(md5str);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("UserId={0}", UserId);
            str.AppendFormat("&UserOrderId={0}", UserOrderId);
            str.AppendFormat("&ChargeAccount={0}", ChargeAccount);
            str.AppendFormat("&BuyNum={0}", BuyNum);
            str.AppendFormat("&GoodsId={0}", GoodsId);
            str.AppendFormat("&GameArea={0}", GameArea);
            str.AppendFormat("&GameServer={0}", GameServer);
            str.AppendFormat("&PassCheck={0}", PassCheck);
            str.AppendFormat("&Sign={0}", Sign);

            return str.ToString();
        }

       public  string getGameValue(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "SWProduct.xml");    //加载Xml文件 
            XmlElement root = doc.DocumentElement;   //获取根节点 
            XmlNodeList GameNodes = root.GetElementsByTagName("Game"); //获取GameList子节点集合 
            foreach (XmlNode node in GameNodes)
            {
                //string Code = ((XmlElement)node).GetAttribute("Code");   //获取Code属性值 
                foreach (var item in node.ChildNodes)
                {
                    string GameName = ((XmlElement)node).GetElementsByTagName("GameName")[0].InnerText;
                    string GameValue = ((XmlElement)node).GetElementsByTagName("GameValue")[0].InnerText;

                    if (GameName.Equals(gameName))
                        return GameValue;
                }
            }

            return null;
        }

       public bool isChinese(string str)
       {
           for (int i = 0; i < str.Length; i++)
           {
               if ((int)str[i] > 127)
               {
                   return true;
               }
           }
           return false;
       }
    }
}
