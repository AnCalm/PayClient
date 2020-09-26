using ChargeInterface.AntoInterface;
using ChargeInterface.RuiLian;
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
   public  class QueryRuiLian : IQuery
    {
       public Order Query(Order order)
       {
           string MerchantID = PayAndQueryRuiLian.merchantID;
           string key = PayAndQueryRuiLian.key;
           string QueryUrl = PayAndQueryRuiLian.QueryUrl;

           CookieContainer coockie = new CookieContainer();

           string strmd5 = MerchantID + order.OrderInsideID + key;
           string sign = Md5Helper.MD5Encrypt(strmd5);
           StringBuilder str = new StringBuilder();
           str.AppendFormat("cid={0}", MerchantID); //商家编号
           str.AppendFormat("&oid={0}", order.OrderInsideID); //商家订单编号
           str.AppendFormat("&sign={0}", sign);  //数字签名


           WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " Pay1 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

           string result = PostAndGet.HttpPostString(QueryUrl, str.ToString(), ref coockie);

           WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " Pay1 订单查询:" + result, LogPathFile.Recharge.ToString());

           string state = Regex.Match(result, @"result=(.*?)&").Groups[1].Value;
           string msg = Regex.Match(result, @"&msg=(.*)").Groups[1].Value;


           if (state.Equals("true"))
           {
               if (msg == "1")
               {
                   order.RechargeStatus = (int)OrderRechargeStatus.successful;
                   order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);

               }
               else if (msg == "0")
               {
                   order.RechargeStatus = (int)OrderRechargeStatus.failure;
                   order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
               }
           }
           else if (state.Equals("false"))
           {
               if (msg == "参数有误" || msg == "商家帐号错误" || msg == "校验失败" || msg == "订单不存在")
               {
                   order.RechargeStatus = (int)OrderRechargeStatus.failure;
                   order.RechargeMsg = msg;
               }
               else
               {
                   order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                   order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious);
               }
           }
           else
           {
               order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
               order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.suspicious);
           }

           return order;
       }

        //订单状态，1为成功，0为失败,2充值中
        //[商家编号]参数有误
        //[商家的订单号]参数有误
        //商家帐号错误
        //MD5校验失败
        //订单不存在
        //初始化连接失败
    }
}
