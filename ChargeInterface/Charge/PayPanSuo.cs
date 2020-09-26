using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using ChargeInterface.PanSuo;
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
    public class PayPanSuo : ICharge
    {
        public Order Charge(Order order)
        {
            PayAndQueryPanSuo pay1 = new PayAndQueryPanSuo();

            string result = pay1.SubmitOrder(order);

            string state = Regex.Match(result, @"<result>(.*?)</result>").Groups[1].Value;
            string msg = Regex.Match(result, @"<mes>(.*?)</mes>").Groups[1].Value;

            if (state == "01")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (state == "02")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = msg;
                order.IsNotify = true;
                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = msg;

                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }

           
            #region errorCode
          

            #endregion

            return order;
        }
    }
}
