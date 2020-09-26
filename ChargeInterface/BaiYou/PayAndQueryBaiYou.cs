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

namespace ChargeInterface.BaiYou
{
    public class PayAndQueryBaiYou
    {
        public const string submitUrl = "http://api.xuniwl.com/Api/Pay";
        public const string queryUrl = "http://api.xuniwl.com/Api/QueryOrder";
        public const string key = "bl9kmfdjkxmyff0";
        public const string merchantID = "10009";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order, "http://116.62.44.48/NotifyFormBaiYou.aspx", AreaValueType.BaiYou);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " BaiYou 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " BaiYou 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        public string setPostDate(Order order, string responseUrl, string iD)
        {
            string MerchantID = merchantID;	//商家编号	Y
            string MerchantOrderID = order.OrderInsideID;	//商家订单编号（不允许重复）	Y
            string ProductID = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);	//商品编号	Y
            string BuyAmount = order.BuyAmount.ToString();	//充值数量	Y
            string TargetAccount = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8);	//充值账户	N
            string TargetAccountType = System.Web.HttpUtility.UrlEncode(order.TargetAccountType, Encoding.UTF8);	//充值账户类型	N
            string TargetAccountTypeName = System.Web.HttpUtility.UrlEncode(order.TargetAccountTypeName, Encoding.UTF8);	//充值账户类型名称	N
            string RechargeMode = System.Web.HttpUtility.UrlEncode(order.RechargeMode, Encoding.UTF8);	//充值方式	N
            string RechargeModeName = System.Web.HttpUtility.UrlEncode(order.RechargeModeName, Encoding.UTF8);	//充值方式名称	N
            string Game = System.Web.HttpUtility.UrlEncode(order.Game, Encoding.UTF8); ;	//充值游戏	N
            string GameName = System.Web.HttpUtility.UrlEncode(order.GameName, Encoding.UTF8);	//充值游戏名称	N
            string Area = new MBJ.PayAndQueryMBJ().getAreaValue(order.AreaName, ProductID, iD);//游戏区域	N
            string AreaName = System.Web.HttpUtility.UrlEncode(order.AreaName, Encoding.UTF8);		//游戏区域名称	N
            string Server = System.Web.HttpUtility.UrlEncode(order.Server, Encoding.UTF8);	//游戏服务器	N
            string ServerName = System.Web.HttpUtility.UrlEncode(order.ServerName, Encoding.UTF8);	//游戏服务器名称	N
            string CustomerIP = order.CustomerIp;	//客户IP/客户区域，用来确定消费区域，只需传一个即可。如果都传以IP为准，如果都不传则默认区域为全国。	N
            string CustomerRegion = System.Web.HttpUtility.UrlEncode(order.CustomerRegion, Encoding.UTF8);
            string ResponseUrl = System.Web.HttpUtility.UrlEncode(responseUrl, Encoding.UTF8);	//接收异步通知订单状态的Url	N
            string Sign = string.Empty;//数字签名	Y

            string MD5Str = MerchantID + MerchantOrderID + ProductID + BuyAmount + order.TargetAccount + key;

            if (order.RechargeModeName.Contains("帐号直充"))
                RechargeMode = "1";
            else if (order.RechargeModeName.Contains("点数寄售"))
                RechargeMode = "2";
            else if (order.RechargeModeName.Contains("魔兽世界游戏时间"))
                RechargeMode = "WOW_TIME";

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
    }
}
