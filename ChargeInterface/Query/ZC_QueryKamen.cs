using ChargeInterface.AntoInterface;
using ChargeInterface.Kamen;
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
    public class ZC_QueryKamen : IQuery
    {
        public Order Query(Order order)
        {
            int num = 0;
            string key = PayAndQueryKamen.key;
            string CustomerId = PayAndQueryKamen.merchantID;
            string queryUrl = PayAndQueryKamen.queryUrl;

            while (num < 5)
            {
                CookieContainer coockie = new CookieContainer();
                string strmd5 ="CustomerId="+CustomerId+"CustomerOrderNo="+order.OrderInsideID+key;
                string sign = Md5Helper.EncryptMd5_Kamen(strmd5);
                StringBuilder str = new StringBuilder();
                str.AppendFormat("CustomerId={0}", CustomerId); //云接口商户编号
                str.AppendFormat("&CustomerOrderNo={0}", order.OrderInsideID); //客户外部系统订单号
                str.AppendFormat("&Sign={0}", sign);  //数字签名(MerchantID+OrderID+key)


                WriteLog.Write("方法:Query，订单号：" + order.OrderInsideID + " Kamen 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

                string result = PostAndGet.HttpPostString(queryUrl, str.ToString(), ref coockie);

                WriteLog.Write("方法:Charge，订单号：" + order.OrderInsideID + " Kamen 订单查询:" + result, LogPathFile.Recharge.ToString());

                //订单状态（未处理、处理中、可疑、成功、失败）
                string OrderStatus = Regex.Match(result, @"<OrderStatus>(.*?)</OrderStatus>").Groups[1].Value;
                string Description = Regex.Match(result, @"<Description>(.*?)</Description>").Groups[1].Value;
                string OrderNo = Regex.Match(result, @"<OrderNo>(.*?)</OrderNo>").Groups[1].Value;

                switch (OrderStatus)
                {
                    case "可疑":
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        order.RechargeMsg = Description + "-" + OrderNo;
                        return order;
                    case "成功":
                        order.RechargeStatus = (int)OrderRechargeStatus.successful;
                        order.RechargeMsg = Description + "-" + OrderNo;
                        return order;
                    case "失败":
                        if (Description.Contains("站点余额不足"))
                        {
                            order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        }
                        else
                        {
                            order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        }

                        order.RechargeMsg = Description + "-" + OrderNo;
                        return order;
                        
                    case "未处理":
                    case "处理中":
                    default:
                        break;
                }

                num++;

                System.Threading.Thread.Sleep(2 * 1000);
            }
            return order;
        }
    }
}
