using ChargeInterface.AntoInterface;
using ChargeInterface.RuiLian;
using ChargeInterface.SUP;
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
    public class PayRuiLian : ICharge
    {
        public Order Charge(Order order)
        {
            PayAndQueryRuiLian pay1 = new PayAndQueryRuiLian();

            string result = pay1.SubmitOrder(order);

            string state = Regex.Match(result, @"result=(.*?)&").Groups[1].Value;
            string msg = Regex.Match(result, @"&msg=(.*)").Groups[1].Value;

            if (state == "true")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (state.Equals("false") &&
                (msg.Contains("交易接口关闭") || msg.Contains("参数有误") || msg.Contains("商家帐号错误") || msg.Contains("商家状态错误") || msg.Contains("MD5校验失败") ||
                msg.Contains("商品编号错误") || msg.Contains("下单失败")))
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

            //交易接口关闭
            //[md5签名]参数有误
            //[客户ID]参数有误
            //[商品ID]参数有误
            //[数量]参数有误
            //商家帐号错误
            //商家状态错误
            //MD5校验失败
            //商品编号错误
            //下单失败,初始化连接失败
            //下单失败,商家余额不足:xx.xxx
            //下单失败,扣款失败,稍后重试
            //下单失败,写入订单失败
            //下单失败,接口运行异常

            #endregion

            return order;
        }
    }
}
