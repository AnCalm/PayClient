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

namespace ChargeInterface.ZhiXin
{
    public class PayAndQueryZhiXin
    {
        public const string submitUrl = "http://api.julives.com:9080/zxpaycore/v2/preorder";
        public const string QueryUrl = "http://query.julives.com:9080/zxpaycore/v2/query";
        public const string key = "duZBIBf3XTNXcBw9VY5K6zMVxTAKBjNn";
        public const string merchantID = "101101414";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (order.ProductID == "20815")
            {
                str = setPostDate2(order);
            }
            else
            {
                if (string.IsNullOrEmpty(str))
                    str = setPostDate(order);
            }

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " ZhiXin 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString_utf8(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " ZhiXin 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        /// <summary>
        /// 5.3	Q币订单受理
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        string setPostDate(Order order)
        {
            string mrch_no = merchantID;	//	商户编号	商户编号	是	20
            string request_time = DateTime.Now.ToString("yyyyMMddHHmmss");	//	请求时间	格式：yyyyMMddHHmmss	是	14
            string client_order_no = order.OrderInsideID;	//	商户订单号	商户订单号，商户自己生成，保证唯一。最大长度不超过32位	是	32
            string product_type = "6";	//	产品类型	6:Q币充值	是	10
            string account = order.TargetAccount;	//	玩家账号	是	20
            string amount = order.BuyAmount.ToString();	//	充值规格	Q币数量 若充值1Q币，则提交1	是	10
            string other_param = order.CustomerIp;	//	其他参数	客户端IP地址	是	20
            string notify_url ="http://116.62.44.48/NotifyFromZhiXin.aspx";	//	回调地址	回调地址 若为空值，则无回调通知	否	128
            string sign = "";	//	签名数据	数据签名	是	32


            //if (other_param.Contains(":"))
            //{
            //    other_param = "115.239.168.199";
            //}


            string md5str = "account"+account+"amount"+amount+"client_order_no"+client_order_no+"mrch_no"+mrch_no+"notify_url"+notify_url+"other_param"+other_param+"product_type"+product_type+"request_time"+request_time + key;
            sign = Md5Helper.MD5Encrypt(md5str);

            string json = "{\"mrch_no\":\"" + mrch_no + "\",\"request_time\":\"" + request_time + "\",\"client_order_no\":\"" + client_order_no + "\",\"product_type\":\"" + product_type + "\",\"account\":\"" + account + "\",\"amount\":\"" + amount + "\",\"other_param\":\"" + other_param + "\",\"notify_url\":\"" + notify_url + "\",\"sign\":\"" + sign + "\"}";

            return json;
        }

        /// <summary>
        /// 5.4	订单提交接口(数娱产品)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        string setPostDate2(Order order)
        {
            string mrch_no = merchantID;	//	商户代号	商户代号	是	20
            string request_time = DateTime.Now.ToString("yyyyMMddHHmmss");	//	请求时间	格式：yyyyMMddHHmmss	是	14
            string client_order_no = order.OrderInsideID;	//	商户订单号	商户订单号，商户自己生成，保证唯一。最大长度不超过32位	是	32
            string account = order.TargetAccount;	//	业务号码	充值号码	是	20
            string product_code = "";	//	产品编码	产品编码	是	10
            string buy_num = (order.BuyAmount*100).ToString();	//	购买数量	正常商品，传递数量1；任意充商品，传递具体购买数量；	是	3
            string other_param = "";	//	其他参数	其他参数，可传空值	是	128
            string notify_url = "http://116.62.44.48/NotifyFromZhiXin.aspx";	//	回调地址	回调地址 若为空值，则平台不发起异步通知	是	128
            string sign = "";	//	签名数据	数字签名	是	32

            //if (other_param.Contains(":"))
            //{
            //    other_param = "115.239.168.199";
            //}

            if (order.AreaName.Contains("安卓"))
            {
                product_code = "I18901-1";
            }
            else if (order.AreaName.Contains("苹果") || order.AreaName.Contains("IOS"))
            {
                product_code = "I18902-1";
            }

            string md5str = "account" + account +"buy_num"+buy_num+"client_order_no" + client_order_no + "mrch_no" 
                + mrch_no + "notify_url" + notify_url + "other_param" + other_param + "product_code" + product_code + "request_time" + request_time + key;

            sign = Md5Helper.MD5Encrypt(md5str);

            string json = "{\"mrch_no\":\"" + mrch_no + "\",\"request_time\":\"" + request_time + "\",\"client_order_no\":\"" + client_order_no
               + "\",\"account\":\"" + account + "\",\"product_code\":\"" + product_code
                + "\",\"buy_num\":\"" + buy_num + "\",\"other_param\":\"" + other_param + "\",\"notify_url\":\"" + notify_url + "\",\"sign\":\"" + sign + "\"}";

            return json;
        }
    }
}
