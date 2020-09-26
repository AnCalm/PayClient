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

namespace ChargeInterface.RuiLian
{
    public class PayAndQueryRuiLian
    {
        public const string submitUrl = "http://124.232.165.196:18082/Interface/SenderOrderEx";
        public const string QueryUrl = "http://124.232.165.196:18082/Interface/QueryOrderEx";
        public const string key = "8af8ef3102bd5361ce69a7c12989a4dd";
        public const string merchantID = "100183";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " RuiLian 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " RuiLian 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {

            string oid = order.OrderInsideID;//	商家订单ID	必选	String	是	商家自己生成的平台ID，注意参数为小写
            string cid = merchantID; //	商家ID	必选	String	是	商家注册的时候产生的一个商家ID
            string pid = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);//	商品ID	必选	String	是	商品ID请在对接的时候向对接技术索要
            string pn = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8);//	充值号码	必选	String	是	您所需要充值的帐号信息
            string nb = order.BuyAmount.ToString();//	商品数量	必选	String	是	您所需要充值帐号的充值数量个数
            string fm =( order.ProductParValue * order.BuyAmount).ToString();//	充值金额	必选	String	是	充值金额=商品面值*商品数量
            string ru = System.Web.HttpUtility.UrlEncode("http://116.62.44.48/NotifyFromRuiLian.aspx", Encoding.UTF8);//	通知地址	必选	String	是	帐号充值结果所反馈到的地址
            string at = System.Web.HttpUtility.UrlEncode(order.TargetAccountType, Encoding.UTF8); //	帐号类型	可选	String	否	辅助参数，与指定的商品有关
            string ct = System.Web.HttpUtility.UrlEncode(order.RechargeModeName, Encoding.UTF8);//	计费类型	可选	String	否	辅助参数，与指定的商品有关
            string fr = "";//	充值区域	可选	String	否	辅助参数，与指定的商品有(格式:区域编号|区域名称)
            string fs = "";//	充值服务器	可选	String	否	辅助参数，与指定的商品有(格式:服务器编号|服务器名称)
            string rin = "";//	商家区域	可选	String	否	辅助参数，商家所在区域
            string pip = order.CustomerIp;//	充值帐号IP	可选	String	否	辅助参数，充值帐号IP地址
            string info1 = "";//	商家自定义	可选	String	否	可选参数，商家自定义参数，原样返回
            string info2 = "";//	商家自定义	可选	String	否	可选参数，商家自定义参数，原样返回
            string sign = "";//	MD5签名	必选	String	否	原串拼接规则:oid-cid-pid-pn-nb-fm-ru-key 例:101447975-test-10097-123456-1-1-backcallurl-test

            string md5str = oid + "-" + cid + "-" + pid + "-" + pn + "-" + nb + "-" + fm + "-" + ru + "-" + key;
            sign = Md5Helper.MD5Encrypt(md5str);

            if (!string.IsNullOrEmpty(order.AreaName))
            {
                //string Area = new ChargeInterface.MBJ.PayAndQueryMBJ().getAreaValue(order.AreaName, pid, AreaValueType.ShuWang);//游戏区域
                fr = System.Web.HttpUtility.UrlEncode(order.Area + "|" + order.AreaName, Encoding.UTF8);
            }

            if (!string.IsNullOrEmpty(order.Server))
            {
                string Area = new ChargeInterface.MBJ.PayAndQueryMBJ().getAreaValue(order.AreaName, pid, AreaValueType.ShuWang);//游戏区域
                fs = System.Web.HttpUtility.UrlEncode(order.Server + "|" + order.ServerName, Encoding.UTF8);
            }

            StringBuilder str = new StringBuilder();
            str.AppendFormat("oid={0}", oid);
            str.AppendFormat("&cid={0}", cid);
            str.AppendFormat("&pid={0}", pid);
            str.AppendFormat("&pn={0}", pn);
            str.AppendFormat("&nb={0}", nb);
            str.AppendFormat("&fm={0}", fm);
            str.AppendFormat("&ru={0}", ru);
            str.AppendFormat("&at={0}", at);
            str.AppendFormat("&ct={0}", ct);
            str.AppendFormat("&fr={0}", fr);
            str.AppendFormat("&fs={0}", fs);
            str.AppendFormat("&rin={0}", rin);
            str.AppendFormat("&pip={0}", pip);
            str.AppendFormat("&info1={0}", info1);
            str.AppendFormat("&info2={0}", info2);
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
