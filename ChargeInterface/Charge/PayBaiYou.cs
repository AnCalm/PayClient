using ChargeInterface.AntoInterface;
using ChargeInterface.BaiYou;
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
    class PayBaiYou : ICharge
    {
        public Order Charge(Order order)
        {
            PayAndQueryBaiYou payBaiYou = new PayAndQueryBaiYou();

            string result =payBaiYou.SubmitOrder(order);

            string state = Regex.Match(result, @"<state>(.*?)</state>").Groups[1].Value;
            string stateInfo = Regex.Match(result, @"<state-info>(.*?)</state-info>").Groups[1].Value;
            string BaiYouOrderId = Regex.Match(result, @"<order-id>(.*?)</order-id>").Groups[1].Value;

            if (state.Equals("101"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
                order.RechargeMsg = stateInfo + "-" + BaiYouOrderId;
                order.IsNotify = true;
                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }
            else if (state.Equals("201") || state.Equals("202") || state.Equals("203") || state.Equals("301") || state.Equals("302") || state.Equals("304") ||
                state.Equals("305") || state.Equals("306") || state.Equals("307") || state.Equals("401") || state.Equals("402") || state.Equals("405") || state.Equals("501"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = stateInfo + "-" + BaiYouOrderId;
                order.IsNotify = true;
                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }
            else if (state.Equals("102"))
            {//充值中
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = stateInfo + "-" + BaiYouOrderId;

                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }

            #region errorCode
            //101	交易成功
            //102	充值中
            //103	可疑订单
            //201	参数不合法
            //202	签名错误
            //203	充值帐号不存在
            //301	商家不存在
            //302	商家已被禁用
            //304	IP地址不合法
            //305	账户余额不足
            //306	订单号重复
            //307	订单不存在
            //401	商品不存在
            //405	购买数量不合法
            //501	取消订单
            //601	渠道问题（详见状态描述）
            //901	未知原因（详见状态描述）

            #endregion


            return order;
        }
    }
}
