using ChargeInterface.AntoInterface;
using ChargeInterface.ZhiXin;
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

namespace ChargeInterface.Query
{
    public class QueryZhiXin : IQuery
    {
        public Order Query(Order order)
        {
            string MerchantID = PayAndQueryZhiXin.merchantID;
            string key = PayAndQueryZhiXin.key;
            string QueryUrl = PayAndQueryZhiXin.QueryUrl;


            CookieContainer coockie = new CookieContainer();

            string dateStr = DateTime.Now.ToString("yyyyMMddHHmmss");

            string mrch_no = MerchantID;//		商户编号	商户代号	是	20
            string request_time = dateStr;//	请求时间	格式：yyyyMMddHHmmss	是	14
            string client_order_no = order.OrderInsideID;//		商户订单号		是	32
            string order_time = dateStr;//		订单下单时间：yyyyMMddHHmmss
            //注意：
            //1. 充值平台从order_time字段截取订单日期T(yyyyMMdd)，查询该日期的订单数据。若无订单，则尝试查询(T-1)及(T+1)日期的订单数据。
            //2. 仅提供近3个月的订单数据	是	14
            string sign = MerchantID;//		签名数据	数据签名	是	32




            string strmd5 = "client_order_no" + client_order_no + "mrch_no" + mrch_no + "order_time" + order_time + "request_time" + request_time + key;
            sign = Md5Helper.MD5Encrypt(strmd5);

            string json = "{\"mrch_no\":\"" + mrch_no + "\",\"request_time\":\"" + request_time + "\",\"client_order_no\":\"" + client_order_no + "\",\"order_time\":\"" + order_time + "\",\"sign\":\"" + sign + "\"}";

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " ZhiXin 订单查询参数:" + json, LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString_utf8(QueryUrl, json, ref coockie);

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " ZhiXin 订单查询:" + result, LogPathFile.Recharge.ToString());

            // 1:充值中 2:充值成功 6:充值失败

            string code = Regex.Match(result, @"""code"":""(.*?)""").Groups[1].Value;
            string up_order_no = Regex.Match(result, @"""up_order_no"":""(.*?)""").Groups[1].Value;
            string recharge_status = Regex.Match(result, @"""recharge_status"":""(.*?)""").Groups[1].Value;
            string desc = Regex.Match(result, @"""desc"":""(.*?)""").Groups[1].Value;
            string message = Regex.Match(result, @"""message"":""(.*?)""").Groups[1].Value;

            if (code=="2")
            {
                if (recharge_status == "2")
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                    order.RechargeMsg = desc + up_order_no;
                }
                else if (recharge_status == "6")
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = desc + up_order_no;
                }
                else if (recharge_status == "1")
                {
                    //充值中
                } 
            }
            else if (code == "600" || code == "602" || code == "603" || code == "606" || code == "622"
                || code == "623" || code == "624" || code == "615" || code == "637" || code == "715")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = message + up_order_no;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious);

            }

            #region Code
            //2	操作成功	预受理	提交成功，订单充值中
            //600	商户禁用,接口已关闭	预受理	失败
            //602	订单提交失败，未充值	预受理	失败
            //603	请求数据格式错误	预受理	失败
            //606	数据签名错误	预受理	失败
            //622	商户不存在	预受理	失败
            //623	通道维护	预受理	失败
            //624	产品未配置	预受理	失败
            //615	号码归属地未配置	预受理	失败
            //637	流量充值未配置	预受理	失败
            //751	IP地址限制	预受理	失败
            //625	重复订单号	预受理	存疑
            //查询订单状态或人工核实
            //其他返回码	网络问题导致提交异常或其他返回码	预受理	存疑
            //查询订单状态或人工核实
            #endregion

            #region recharge_status
            //2	操作成功	订单状态查询	操作成功，根据recharge_status判定订单状态
            //recharge_status:1,充值中
            //recharge_status:2,充值成功
            //recharge_status:6,充值失败
            //626	未查询到订单信息	订单状态查询	1.	请检查order_time(订单下单时间)字段是否准确，该字段影响订单查询结果。
            //                                  2.	在订单提交10分钟后，订单查询接口仍未查询到订单信息，订单可作失败处理
            //603	请求数据不正确或查询异常	订单状态查询	再次查询或人工核实。若返回该状态码，不能做订单失败处理。
            #endregion

            return order;
        }
    }
}
