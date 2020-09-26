using ChargeInterface.AntoInterface;
using ChargeInterface.PanSuo;
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
    public class QueryPanSuo : IQuery
    {
        public Order Query(Order order)
        {
            string MerchantID = PayAndQueryPanSuo.merchantID;
            string key = PayAndQueryPanSuo.key;
            string QueryUrl = PayAndQueryPanSuo.QueryUrl;


            CookieContainer coockie = new CookieContainer();


            string businessId = MerchantID;// 商户号	由SUP系统分配每个商户唯一的一个商户号
            string userOrderId = order.OrderInsideID;//	商户订单号（流水号）	最大长度不超过32位的唯一流水号
            string payoffPriceTotal = (order.BuyAmount * order.ProductParValue).ToString();	//结算总金额	系统和进货平台结算金额
            string sign = ""; //签名 case(md5(businessId + userOrderId +key)) MD5加密后小写


            string strmd5 = businessId + userOrderId + key;
            sign = Md5Helper.MD5Encrypt(strmd5);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("businessId={0}", businessId);
            str.AppendFormat("&userOrderId={0}", userOrderId);
            str.AppendFormat("&payoffPriceTotal={0}", payoffPriceTotal);
            str.AppendFormat("&sign={0}", sign);


            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " PanSuo 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " PanSuo 订单查询:" + result, LogPathFile.Recharge.ToString());

            //01	成功（订单最终状态）  
            //02	失败（订单最终状态）  
            //03	处理中（需要等待异步通知结果）
            //04	订单不存在
            //05	未知错误
            //06	签名错误
            //07	参数有误


            string state = Regex.Match(result, @"<result>(.*?)</result>").Groups[1].Value;
            string msg = Regex.Match(result, @"<mes>(.*?)</mes>").Groups[1].Value;

            if (state.Equals("01"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);

            }
            else if (result.Equals("02") || result.Equals("04") || result.Equals("06"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                if (string.IsNullOrEmpty(msg))
                    order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                else
                    order.RechargeMsg = msg;
            }
            else if (result.Equals("03"))
            {
                //充值中
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious);

            }

            return order;
        }
    }
}
