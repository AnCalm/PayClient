using ChargeInterface.AntoInterface;
using ChargeInterface.XunTong;
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
    public class QueryXunTong : IQuery
    {
        public Order Query(Order order)
        {
            string MerchantID = PayAndQueryXunTong.merchantID;
            string key = PayAndQueryXunTong.key;
            string QueryUrl = PayAndQueryXunTong.QueryUrl;


            CookieContainer coockie = new CookieContainer();

            string username = MerchantID;//	接入代理用户名
            string type = "qb";//	查询类型（固定：qb）
            string sporderid = order.OrderInsideID;//	SP订单号,商户平台的订单号，最长32位（yyyyMMddHHmmss+8）
            string buyhaoma = order.TargetAccount;//	要进行查询的号码
            string sign = "";//	MD5组合数字签名方式：MD5(username={}&type={}&sporderid={}&buyhaoma={}&key=APIkey加密串均为小写,MD5输出为32位小写


            string strmd5 = "username=" + username + "&type=" + type + "&sporderid=" + sporderid + "&buyhaoma=" + buyhaoma + "&key=" + key; ;
            sign = Md5Helper.MD5Encrypt(strmd5);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("username={0}", username);
            str.AppendFormat("&type={0}", type);
            str.AppendFormat("&sporderid={0}", sporderid);
            str.AppendFormat("&buyhaoma={0}", buyhaoma);
            str.AppendFormat("&sign={0}", sign);


            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " XunTong 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " XunTong 订单查询:" + result, LogPathFile.Recharge.ToString());

            // 001 充值成功 002 充值失败 003 充值处理中 004，已冲正 005，充正中 006其他情况

            if (result.Equals("001") || result.Equals("004"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);

            }
            else if (result.Equals("002"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
            }
            else if (result.Equals("003") || result.Equals("005") || result.Equals("006"))
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
