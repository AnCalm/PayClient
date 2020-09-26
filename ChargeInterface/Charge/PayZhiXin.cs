using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using ChargeInterface.ZhiXin;
using Common;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChargeInterface.Charge
{
    public class PayZhiXin : ICharge
    {
        public Order Charge(Order order)
        {
            PayAndQueryZhiXin pay1 = new PayAndQueryZhiXin();

            string result = pay1.SubmitOrder(order);

            string code = Regex.Match(result, @"""code"":""(.*?)""").Groups[1].Value;
            string message = Regex.Match(result,@"""message"":""(.*?)""").Groups[1].Value;

            if (code == "2")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (code == "600" || code == "602" || code == "603" || code == "606" || code == "622"
            || code == "623" || code == "624" || code == "615" || code == "637" || code == "715")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = message;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = message;

                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }

            return order;
        }
    }
}
