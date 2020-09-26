using ChargeInterface.AntoInterface;
using ChargeInterface.Xinqidian;
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
    public class QueryXinqidian : IQuery
    {
        public Order Query(Order order)
        {
            string MerchantID = PayAndQueryXinqidian.merchantID;
            string key = PayAndQueryXinqidian.key;
            string QueryUrl = PayAndQueryXinqidian.QueryUrl;


            CookieContainer coockie = new CookieContainer();

            string UserId = MerchantID;//	用户编号	必选	String	是	
            string UserOrderId = order.OrderInsideID;//	用户订单号	必选	String	是	
            string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");	//时间戳	必选	String	是	格式：yyyyMMddHHmmss 允许最大时间误差为60分钟
            string Sign = "";	//签名字符串	必选	String	否	加密后的字符串

            string strmd5 = "timestamp" + TimeStamp + "userid" + UserId + "userorderid" + UserOrderId + "key" + key;
            Sign = Md5Helper.GetMD5String_Default(strmd5);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("UserId={0}", UserId);
            str.AppendFormat("&UserOrderId={0}", UserOrderId);
            str.AppendFormat("&TimeStamp={0}", TimeStamp);
            str.AppendFormat("&Sign={0}", Sign);


            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " Xinqidian 订单加密参数:" + strmd5.ToString() + " Xinqidian 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " Xinqidian 订单查询:" + result, LogPathFile.Recharge.ToString());




            string Code = Regex.Match(result, @"""Code"":(.*?),").Groups[1].Value;
            string Status = Regex.Match(result, @"""Status"":""(.*?)"",").Groups[1].Value;
            string StatusMsg = Regex.Match(result, @"""Message"":""(.*?)""").Groups[1].Value;
            string OrderId = Regex.Match(result, @"""OrderId"":""(.*?)""").Groups[1].Value;

            if (Code.Equals("1") && Status.Equals("1"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
                if (StatusMsg == "响应成功")
                    StatusMsg = EnumService.GetDescription(OrderRechargeStatus.successful);

                order.RechargeMsg = StatusMsg + OrderId;

            }
            else if (Code.Equals("1") && Status.Equals("2"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                if (StatusMsg == "响应成功")
                    StatusMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                order.RechargeMsg = StatusMsg + OrderId;
            }
            else
            {
                //充值中
            }


            //只有当返回Code=1 并且 响应Content中Status为1时可作成功处理
            //当返回Code=1并且响应Content中Status为2时可作失败处理

            return order;
        }
    }
}
