using ChargeInterface.AntoInterface;
using ChargeInterface.Kamen;
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
    public class PayKamen : ICharge
    {
        public Order Charge(Order order)
        {

            PayAndQueryKamen payKamen = new PayAndQueryKamen();

            string result = payKamen.SubmitOrder(order);


            if (result.Contains("ErrorCode"))
            {
                string errorCode = Regex.Match(result, @"<ErrorCode>(.*?)</ErrorCode>").Groups[1].Value;
                string errorMsg = Regex.Match(result, @"<ErrorMsg>(.*?)</ErrorMsg>").Groups[1].Value;

                if (errorCode.Equals("1014"))  //可疑
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                    order.RechargeMsg = errorMsg;
                    new GetAndNotifySUPOrders().notigyOrderToSUP(order);

                }
                else //失败
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = errorMsg;
                }
            }
            else  //提交成功
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }

            #region errorCode
            //1001	缺少必要参数
            //1002	参数格式不正确
            //1003	签名错误
            //1005	客户ID不存在
            //1006	商品不存在或无法购买
            //1007	商品在禁售时间段内，无法购买。
            //1008	站点黑名单
            //1009	商品黑名单
            //1010	商品类型错误
            //1011	库存不足
            //1012	账户余额不足
            //1013	站点余额不足以支付
            //1014	生成订单失败
            //1015	客户外部系统订单号已存在
            //1016(下单返回)	访问IP不在IP白名单内，限制购买
            //1019	该帐号没有充值区域商品权限
            //1020	该商品x分钟内只能购买一次！
            //1021	商品正在维护
            //1022	获取库存信息失败
            //1024	服务器维护中
            //2000	消费保护
            //1016(查询返回)	客户外部系统订单号不存在
            //2400	不存在该站点编号
            #endregion


            return order;
        }
    }
}
