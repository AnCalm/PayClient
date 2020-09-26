using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using ChargeInterface.XunTong;
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
    public class PayXunTong : ICharge
    {
        public Order Charge(Order order)
        {
            PayAndQueryXunTong pay1 = new PayAndQueryXunTong();

            string result = pay1.SubmitOrder(order);

            string state = Regex.Match(result, @"<ret>(.*?)</ret>").Groups[1].Value;
            string msg = Regex.Match(result, @"<msg>(.*?)</msg>").Groups[1].Value;

            if (state == "000")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (state.Equals("001") || state.Equals("002") || state.Equals("003") || state.Equals("004") || state.Equals("005") || state.Equals("007")
                || state.Equals("009") || state.Equals("010") || state.Equals("011") || state.Equals("012") || state.Equals("013") || state.Equals("014")
                || state.Equals("015") || state.Equals("016") || state.Equals("017") || state.Equals("018") || state.Equals("019") || state.Equals("021") 
                || state.Equals("022"))
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

            //<?xml version="1.0" encoding="utf-8"?> 
            //<Root> 
            //<message> 
            //<ret>返回响应吗，具体含义参考响应吗说明，返回非000 则无data节点</ret> 
            //<msg>返回信息说明</msg> 
            //</message> 
            //<data>
            //<username>原值返回</username>
            //<type>原值返回</type>
            //< sporderid >原值返回</ sporderid >
            //<money>消费金额</money>
            //<usermoney>账户剩余金额</usermoney>
            //<jkorderid>平台订单号</jkorderid>
            //</data>
            //</Root>



            #region errorCode
            //000	提交成功
            //001	账号不存在
            //002	账号被锁定，无法充值
            //003	接口已绑定IP，当前IP未通过验证
            //004	商品不存在或商品当前暂停充值
            //005	客户IP地址被屏蔽
            //006	未知错误（不做失败判定）
            //007	请求方式非POST
            //008	订单重复提交，按已提交处理
            //009	购买数量错误，该商品不支持当前数量
            //010	校验签名错误
            //011	代理账户余额不足
            //012	充值类型不正确
            //013	不具备该商品购买权限
            //014	客户IP地址不合法，或内网IP，或非中国大陆区域IP
            //015	账户金额状态被锁定，无法充值
            //016	用户状态锁定，无法充值
            //017	号码错误
            //018	无法确认要充值号码的区域
            //019	订单长度不符
            //020	订单不存在（不作为退款，建议设置为可疑或者再次查询）
            //021	参数错误
            //022	必要参数传送不可为空
            //023	余额查询过于频繁
            //999	系统维护

            #endregion

            return order;
        }
    }
}
