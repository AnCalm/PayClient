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

namespace ChargeInterface.XunTong
{
    public class PayAndQueryXunTong
    {
        public const string submitUrl = "http://121.42.166.214:8099/zongapi.aspx ";
        public const string QueryUrl = "http://121.42.166.214:8099/select.aspx ";
        public const string key = "ef61aa43fbcd42ab9ccc69722eba5698";
        public const string merchantID = "15071226434";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " XunTong 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " XunTong 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {
            string username = merchantID;	//接入代理用户名
            string type = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);	//	具体请参表4 《type匹配表》
            string buynum = order.BuyAmount.ToString();	//	购买数量 
            string sporderid = order.OrderInsideID;	//	SP订单号,商户平台的订单号，最长32位（yyyyMMddHHmmss+8）
            string gameuser = "";	//	游戏内分账号（如：WOW1）（如无传空，但是必须传）
            string buyhaoma = order.TargetAccount;	//	要进行充值的号码或通行证号码
            string clientip = order.CustomerIp;	//	玩家所在IP，必须真实的区域IP地址
            string returl = "http://116.62.44.48/NotifyFromXunTong.aspx";	//	回调地址,订单充值成功后返回的URL地址
            string sign = "";	//	MD5组合数字签名方式：MD5(username={}&type={}&sporderid={}&buynum={}&buyhaoma={}&key=APIkey) 加密MD5输出为32位小写 “{}”为占位符

            string md5str = "username=" + username + "&type=" + type + "&sporderid=" + sporderid + "&buynum=" + buynum + "&buyhaoma=" + buyhaoma + "&key=" + key;
            sign = Md5Helper.MD5Encrypt(md5str);

            if (isChinese(buyhaoma))
                buyhaoma = System.Web.HttpUtility.UrlEncode(buyhaoma, Encoding.UTF8);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("username={0}", username);
            str.AppendFormat("&type={0}", type);
            str.AppendFormat("&buynum={0}", buynum);
            str.AppendFormat("&sporderid={0}", sporderid);
            str.AppendFormat("&gameuser={0}", gameuser);
            str.AppendFormat("&buyhaoma={0}", buyhaoma);
            str.AppendFormat("&clientip={0}", clientip);
            str.AppendFormat("&returl={0}", System.Web.HttpUtility.UrlEncode(returl, Encoding.UTF8));
            str.AppendFormat("&sign={0}", sign);

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
