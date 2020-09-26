using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using ChargeInterface.Xinqidian;
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
    public class PayXinqidian : ICharge
    {
        public Order Charge(Order order)
        {

            PayAndQueryXinqidian payXinqidian = new PayAndQueryXinqidian();

            string result = payXinqidian.SubmitOrder(order);

            string Code = Regex.Match(result, @"""Code"":(.*?),").Groups[1].Value;
            string Message = Regex.Match(result, @"""Message"":""(.*?)""").Groups[1].Value;

            if (Code == "1")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (Code == "1000" || Code == "1001" || Code == "1002" || Code == "1004"
                || Code == "1006" || Code == "1007" || Code == "1008" || Code == "1009" || Code == "1010")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = Message;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = Message;
                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }

            #region
            //1	响应成功	表示当前请求是否成功
            //1000	参数错误	必填参数为空
            //1001	暂不支持的手机号码	
            //1002	用户未配置商品	联系相关运营人员配置
            //1004	余额不足	
            //1005	用户订单号重复	
            //1006	时间戳已失效	
            //1007	无效的用户编号	
            //1008	用户已冻结	
            //1009	访问IP不在IP白名单内	
            //1010	签名错误	
            //1013	用户订单号不存在	
            //3003	提卡数量超出	用户单次提卡最多50张
            //提卡接口中可以给失败
            //3004	已超出当天用户限额	联系相关运营
            //提卡接口中可以给失败
            //3005	库存不足	提卡接口中可以给失败
            //3006	取卡失败	不能给失败
            //进行卡密订单查询 
            //9998	未知错误	下单或查单过程返回该错误，不能当成下单成功或订单失败处理，联系相关运营人员处理
            //9999	系统异常	下单或查单过程返回该错误，不能当成下单成功或订单失败处理，联系相关运营人员处理

            #endregion

            return order;
        }
    }
}
