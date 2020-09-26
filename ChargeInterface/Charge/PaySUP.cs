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

namespace ChargeInterface.Charge
{
    public class PaySUP : ICharge
    {
        public Order Charge(Order order)
        {
            ManageSUP manageSUP = new ManageSUP();

            string result = manageSUP.SubmitOrder(order);

            string state = Regex.Match(result, @"<State>(.*?)</State>").Groups[1].Value;
            string SupOrderID = Regex.Match(result, @"<SupOrderID>(.*?)</SupOrderID>").Groups[1].Value;
            string State_Info = Regex.Match(result, @"  <State_Info>(.*?)</State_Info>").Groups[1].Value;

            if (state == "0")
            {
                string msg = string.Empty;
                string status = manageSUP.queryOrder(order,SupOrderID, ref msg);

                switch (status)
                {
                    case "未处理":
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                    case "充值成功":
                        order.RechargeStatus = (int)OrderRechargeStatus.successful;
                        break;
                    case "充值失败":
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        break;
                    default:
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                }

                order.RechargeMsg = msg;

            }
            else if (state == "2")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = "系统异常" + State_Info;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = "订单提交失败 " + State_Info;
            }

            return order;
        }
    }
}
