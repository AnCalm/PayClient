using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using Newtonsoft.Json;
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

namespace ChargeInterface.Fulu
{
    /// <summary>
    /// 福禄带票 
    /// </summary>
    public class PayAndQueryFulu2
    {
        public const string submitUrl = "http://openapi.fulu.com/api/getway";
        public const string queryUrl = "http://openapi.fulu.com/api/getway";
        public const string key = "e017b189ab704546b8cad3ba916592f4";
        public const string appkey = "g3LdSBDtTFduGW0NuxlqxfdtncTpEeVsEqagqFxh8iCRUEK6ePRV2LO8fpM+pgZ9";
        //public const string merchantID = "804130";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Fulu2 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString_utf8(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " Fulu2 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        public string QueryOrder(Order order)
        {
            CookieContainer coockie = new CookieContainer();

            string  str = setPostDate_Query(order);

            WriteLog.Write("方法:QueryOrder，订单号：" + order.OrderInsideID + " Fulu2 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString_utf8(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:QueryOrder，订单号：" + order.OrderInsideID + " Fulu2 订单查询:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {
            

            //公共请求参数
            string app_key = appkey;//	String	是	32	i4esv1l+76l/7NQCL3QudG90Fq+YgVfFGJAWgT+7qO1Bm9o/adG/1iwO2qXsAXNB	开放平台分配给商户的app_key
            string method = "fulu.order.direct.add";//	String	是	128	fulu.order.direct.add	接口方法名称
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//	string	是	192	2014-07-24 03:07:50	时间戳，格式为：yyyy-MM-dd HH:mm:ss
            string version = "2.0";//	string	是	3	2.0	调用的接口版本
            string format = "json";//	String	是	10	json	接口请求或响应格式
            string charset = "utf-8";//	String	是	10	utf-8	请求使用的编码格式，如utf-8等
            string sign_type = "md5";//	String	是	10	md5	签名加密类型，目前仅支持md5
            string sign = "";//	String	是	344	详见示例	签名串，签名规则详见右侧《常见问题》中的“ 3.签名计算规则说明 ”
            string app_auth_token = "";//	String	是	40		授权码，固定值为“”
            string biz_content = "";//	string	是		{"remaining_number":"","charge_type":"Q币","customer_order_no":"201906281030191013526","product_id":"10000001","charge_password":"","charge_ip":"192.168.1.100","charge_game_name":"三国群英传","charge_game_srv":"逐鹿中原","contact_qq":"","charge_account":"888888","buy_num":"1","contact_tel":"","charge_game_region":"电信一区","charge_game_role":"赵云"}	

            #region  请求参数
            string remaining_number = "";//	int	否	20		剩余数量
            string charge_type = "";//	string	否	20	Q币	充值类型
            string customer_order_no = order.OrderInsideID;//	string	是	50	201906281030191013526	外部订单号
            string product_id = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode);//	int	是	20	10000001	商品编号
            string charge_password = "";//	string	否	50		充值密码，部分游戏类要传
            string charge_ip = order.CustomerIp;//	string	否	20	192.168.1.100	下单真实Ip，区域商品要传
            string charge_game_name = order.GameName;//	string	否	50	三国群英传	充值游戏名称
            //string charge_game_name = "";
            string charge_game_srv = order.ServerName;//	string	否	50	逐鹿中原	充值游戏服
            //string charge_game_srv = "";
            string contact_qq = "";//	string	否	50		联系QQ
            string charge_account =order.TargetAccount;//	string	是	50	888888	充值账号
            string buy_num = order.BuyAmount.ToString();//	int	是	10	1	购买数量
            string contact_tel = "";//	string	否	15		联系电话
            string charge_game_region = order.AreaName;//	string	否	50	电信一区	充值游戏区
            //string charge_game_region = "";
            string charge_game_role = "";//	string	否	50	赵云	充值游戏角色

            if (order.GameName.Contains("懒人听书"))
            {
                //ChargeGame = System.Web.HttpUtility.UrlEncode("懒人听书", Encoding.UTF8);

                if (order.AreaName.Contains("IOS充值"))
                    charge_game_region = "苹果端";
                if (order.AreaName.Contains("安卓充值"))
                    charge_game_region = "安卓端";
            }

            Dictionary<string,string> dic = new Dictionary<string,string> ();

            //{"remaining_number":"","charge_type":"Q币","customer_order_no":"201906281030191013526","product_id":"10000001","charge_password":"",
            //    "charge_ip":"192.168.1.100","charge_game_name":"三国群英传","charge_game_srv":"逐鹿中原","contact_qq":"","charge_account":"888888"
            //    "buy_num":"1","contact_tel":"","charge_game_region":"电信一区","charge_game_role":"赵云"}	

            dic.Add("remaining_number", remaining_number);
            dic.Add("charge_type", charge_type);
            dic.Add("customer_order_no", customer_order_no);
            dic.Add("product_id", product_id);
            dic.Add("charge_password", charge_password);
            dic.Add("charge_ip", charge_ip);
            dic.Add("charge_game_name", charge_game_name);
            dic.Add("charge_game_srv", charge_game_srv);
            dic.Add("contact_qq", contact_qq);
            dic.Add("charge_account", charge_account);
            dic.Add("buy_num", buy_num);
            dic.Add("contact_tel", contact_tel);
            dic.Add("charge_game_region", charge_game_region);
            dic.Add("charge_game_role", charge_game_role);

            biz_content = JavaScriptConvert.SerializeObject(dic);

            #endregion

            #region  MD5加密
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("app_key", app_key);
            dictionary.Add("method", method);
            dictionary.Add("timestamp", timestamp);
            dictionary.Add("version", version);
            dictionary.Add("format", format);
            dictionary.Add("charset", charset);
            dictionary.Add("sign_type", sign_type);
            dictionary.Add("app_auth_token", app_auth_token);
            dictionary.Add("biz_content", biz_content);

            string jsonData =JavaScriptConvert.SerializeObject(dictionary);

            var chars = jsonData.ToCharArray();
            Array.Sort(chars);

            string data = new string(chars) + key;

            sign = Md5Helper.Md5_Fulu(data).ToLower();
            #endregion

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("app_key", app_key);
            dic2.Add("method", method);
            dic2.Add("timestamp", timestamp);
            dic2.Add("version", version);
            dic2.Add("format", format);
            dic2.Add("charset", charset);
            dic2.Add("sign_type", sign_type);
            dic2.Add("sign", sign);
            dic2.Add("app_auth_token", app_auth_token);
            dic2.Add("biz_content", biz_content);

            string resultData = JavaScriptConvert.SerializeObject(dic2);

            //                 {
            //  "app_key": "i4esv1l+76l/7NQCL3QudG90Fq+YgVfFGJAWgT+7qO1Bm9o/adG/1iwO2qXsAXNB",
            //  "method": "fulu.order.direct.add",
            //  "timestamp": "2019-07-27 16:44:30",
            //  "version": "2.0",
            //  "format": "json",
            //  "charset": "utf-8",
            //  "sign_type": "md5",
            //  "sign": "375d05b41775e1ddf2098504444af1e2",
            //  "app_auth_token": "",
            //  "biz_content": "{\"customer_order_no\":\"201906281030191013526\",\"product_id\":10000001,\"charge_account\":\"888888\",\"buy_num\":1,\"charge_ip\":\"192.168.1.100\",\"charge_game_name\":\"三国群英传\",\"charge_game_region\":\"电信一区\",\"charge_game_srv\":\"逐鹿中原\",\"charge_type\":\"Q币\"}"
            //}
            return resultData;
        }

        string setPostDate_Query(Order order)
        {

            //公共请求参数
            string app_key = appkey;//	String	是	32	i4esv1l+76l/7NQCL3QudG90Fq+YgVfFGJAWgT+7qO1Bm9o/adG/1iwO2qXsAXNB	开放平台分配给商户的app_key
            string method = "fulu.order.info.get";//	String	是	128	fulu.order.info.get		接口方法名称
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//	string	是	192	2014-07-24 03:07:50	时间戳，格式为：yyyy-MM-dd HH:mm:ss
            string version = "2.0";//	string	是	3	2.0	调用的接口版本
            string format = "json";//	String	是	10	json	接口请求或响应格式
            string charset = "utf-8";//	String	是	10	utf-8	请求使用的编码格式，如utf-8等
            string sign_type = "md5";//	String	是	10	md5	签名加密类型，目前仅支持md5
            string sign = "";//	String	是	344	详见示例	签名串，签名规则详见右侧《常见问题》中的“ 3.签名计算规则说明 ”
            string app_auth_token = "";//	String	是	40		授权码，固定值为“”
            string biz_content = "";//	string	是		{"customer_order_no":"201906281030191013526"}	


            //请求参数
            string customer_order_no = order.OrderInsideID;	//string	是	50	201906281030191013526	外部订单号

            string str = "{\"customer_order_no\":\"" + customer_order_no + "\"}";

            biz_content = str.ToString();

            #region  MD5加密
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("app_key", app_key);
            dictionary.Add("method", method);
            dictionary.Add("timestamp", timestamp);
            dictionary.Add("version", version);
            dictionary.Add("format", format);
            dictionary.Add("charset", charset);
            dictionary.Add("sign_type", sign_type);
            dictionary.Add("app_auth_token", app_auth_token);
            dictionary.Add("biz_content", biz_content);

            string jsonData = JavaScriptConvert.SerializeObject(dictionary);

            var chars = jsonData.ToCharArray();
            Array.Sort(chars);

            string data = new string(chars) + key;

            sign = Md5Helper.Md5_Fulu(data).ToLower();
            #endregion


            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("app_key", app_key);
            dic2.Add("method", method);
            dic2.Add("timestamp", timestamp);
            dic2.Add("version", version);
            dic2.Add("format", format);
            dic2.Add("charset", charset);
            dic2.Add("sign_type", sign_type);
            dic2.Add("sign", sign);
            dic2.Add("app_auth_token", app_auth_token);
            dic2.Add("biz_content", biz_content);

            string resultData = JavaScriptConvert.SerializeObject(dic2);

            // "app_key": "i4esv1l+76l/7NQCL3QudG90Fq+YgVfFGJAWgT+7qO1Bm9o/adG/1iwO2qXsAXNB",
            //"method": "fulu.order.info.get",
            //"timestamp": "2019-07-27 16:44:30",
            //"version": "2.0",
            //"format": "json",
            //"charset": "utf-8",
            //"sign_type": "md5",
            //"sign": "375d05b41775e1ddf2098504444af1e2",
            //"app_auth_token": "",
            //"biz_content": "{\"customer_order_no\":\"201906281030191013526\"}"


            return resultData.ToString();
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
