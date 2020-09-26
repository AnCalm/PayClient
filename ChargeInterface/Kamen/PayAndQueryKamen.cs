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

namespace ChargeInterface.Kamen
{
    public class PayAndQueryKamen
    {
        public const string submitUrl = "http://api.kamennet.com/Api/Order/Charge.aspx";
        public const string queryUrl = "http://api.kamennet.com/api/order/Query.aspx";
        public const string key = "918BB8A545017EB8B5E26D9F62AD33E1";
        public const string merchantID = "804130";

         public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Kamen 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Kamen 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

         string setPostDate(Order order)
         {
             string CustomerId = merchantID;	//int	是	云接口商户编号
             string CustomerOrderNo = order.OrderInsideID;	//string	是	客户外部系统订单号
             string ProductId = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);	//int	是	要充值的商品编号
             string BuyNum = order.BuyAmount.ToString(); ;	//int	是	购买数量
             string ChargeAccount = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8);	//string	是	要充值的游戏账号/手机号/电话/QQ号等
             string ChargePassword = "";	//string	否	要充值的账号的密码，部分游戏需要。
             string ChargeGame = string.Empty;   //string	否	要充值的游戏名称，部分游戏需要
             string ChargeRegion = string.Empty; //string	否	要充值的游戏区，部分游戏需要
             string ChargeType = string.Empty;//string	否	要充值的游戏类型，部分游戏需要

             if (order.ProductID.Equals("14913"))
                 ChargeGame = System.Web.HttpUtility.UrlEncode("Q币", Encoding.UTF8);
             else
                 ChargeGame = System.Web.HttpUtility.UrlEncode(order.GameName, Encoding.UTF8);


             if (order.GameName.Contains("懒人听书-备用"))
             {
                 ChargeGame = System.Web.HttpUtility.UrlEncode("懒人听书", Encoding.UTF8);

                 if (order.AreaName.Contains("IOS充值"))
                     ChargeRegion = System.Web.HttpUtility.UrlEncode("苹果端", Encoding.UTF8);
                 if (order.AreaName.Contains("安卓充值"))
                     ChargeRegion = System.Web.HttpUtility.UrlEncode("安卓端", Encoding.UTF8);
             }
             else
             {
                 ChargeRegion = System.Web.HttpUtility.UrlEncode(order.AreaName, Encoding.UTF8);
             }
            

             if (order.ProductID.Equals("14961"))
             {//网易一卡通按元充值-k2
                 if(order.RechargeModeName.Contains("点数寄售"))
                     ChargeType = System.Web.HttpUtility.UrlEncode("点卡交易", Encoding.UTF8);
                 if (order.RechargeModeName.Contains("帐号直充"))
                     ChargeType = System.Web.HttpUtility.UrlEncode("游戏点数", Encoding.UTF8);
             }
             else 
             {
                 ChargeType = System.Web.HttpUtility.UrlEncode(order.RechargeModeName, Encoding.UTF8);
             }
             if (order.ProductID.Equals("14913"))
             {
                 ChargeType = "";
             }

             string ChargeServer = System.Web.HttpUtility.UrlEncode(order.Server, Encoding.UTF8);	//string	否	要充值的游戏服务器，部分游戏需要    
             string NotifyUrl = System.Web.HttpUtility.UrlEncode("http://116.62.44.48/NotifyFromKamen.aspx", Encoding.UTF8);	//string	否	第三方异步接收地址，用于接收订单成功/失败的状态
             string BuyerIp = System.Web.HttpUtility.UrlEncode(order.CustomerIp, Encoding.UTF8);	//string	否	购买者IP
             string RoleName = "";	//string	否	要充值游戏的角色名称，部分游戏需要
             string RemainingNumber = "";//string	否	要充值游戏的剩余数量，部分游戏需要
             string ContactType = "";	//string	否	要充值游戏的联系电话，部分游戏需要
             string ContactQQ = "";	//string	否	要充值游戏的联系QQ，部分游戏需要
             string Sign = "";	//string	是	签名

             StringBuilder str = new StringBuilder();

             str.AppendFormat("CustomerId={0}", CustomerId);
             str.AppendFormat("&CustomerOrderNo={0}", CustomerOrderNo);
             str.AppendFormat("&ProductId={0}", ProductId);
             str.AppendFormat("&BuyNum={0}", BuyNum);
             str.AppendFormat("&ChargeAccount={0}", ChargeAccount);

             if (!string.IsNullOrEmpty(ChargePassword))
                 str.AppendFormat("&ChargePassword={0}", ChargePassword);
             if (!string.IsNullOrEmpty(ChargeGame))
                 str.AppendFormat("&ChargeGame={0}", ChargeGame);
             if (!string.IsNullOrEmpty(ChargeRegion))
                 str.AppendFormat("&ChargeRegion={0}", ChargeRegion);
             if (!string.IsNullOrEmpty(ChargeServer))
                 str.AppendFormat("&ChargeServer={0}", ChargeServer);
             if (!string.IsNullOrEmpty(ChargeType))
                 str.AppendFormat("&ChargeType={0}", ChargeType);
             if (!string.IsNullOrEmpty(NotifyUrl))
                 str.AppendFormat("&NotifyUrl={0}", NotifyUrl);
             if (!string.IsNullOrEmpty(BuyerIp))
                 str.AppendFormat("&BuyerIp={0}", BuyerIp);
             if (!string.IsNullOrEmpty(RoleName))
                 str.AppendFormat("&RoleName={0}", RoleName);
             if (!string.IsNullOrEmpty(RemainingNumber))
                 str.AppendFormat("&RemainingNumber={0}", RemainingNumber);
             if (!string.IsNullOrEmpty(ContactType))
                 str.AppendFormat("&ContactType={0}", ContactType);
             if (!string.IsNullOrEmpty(ContactQQ))
                 str.AppendFormat("&ContactQQ={0}", ContactQQ);


             string md5str = str.ToString().Replace("&", "") + key;
             Sign = Md5Helper.EncryptMd5_Kamen(md5str);

             str.AppendFormat("&Sign={0}", Sign);  // 签名
             return str.ToString();
         }


        //switch (supProductID)
        //{
        //    case "14883":
        //        kamenProductID = "1258442";
        //        break;
        //    case "14884":
        //        kamenProductID = "1258444";
        //        break;
        //    default:
        //        break;
        //}
    }
}
