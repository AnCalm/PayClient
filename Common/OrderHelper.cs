using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class OrderHelper
    {
        private static readonly object Locker = new object();
        private static int _sn = 0;

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns></returns>
        public static string GenerateId(DateTime? datetime, string str)
        {
            lock (Locker)  //lock 关键字可确保当一个线程位于代码的临界区时，另一个线程不会进入该临界区。
            {
                if (_sn == int.MaxValue)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }

                Thread.Sleep(100);
                if (datetime.HasValue)
                    return str + datetime.Value.ToString("yyyyMMddHHmmss") + _sn.ToString().PadLeft(10, '0');
                else
                    return str + DateTime.Now.ToString("yyyyMMddHHmmss") + _sn.ToString().PadLeft(10, '0');
            }
        }

        /// <summary>
        /// 生成子订单
        /// </summary>
        /// <param name="mainOrder"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Order GenerateChildOrder(Order mainOrder, int num)
        {
            Order childOrder = new Order()
            {
                OrderInsideID = mainOrder.OrderInsideID + "_" + num,
                OrderExternalID = mainOrder.OrderExternalID,
                ProductID = mainOrder.ProductID,
                ProductName = mainOrder.ProductName,
                ProductParValue = mainOrder.ProductParValue,
                ProductSalePrice = mainOrder.ProductSalePrice,
                TargetAccount = mainOrder.TargetAccount,
                TargetAccountType = mainOrder.TargetAccountType,
                TargetAccountTypeName = mainOrder.TargetAccountTypeName,
                BuyAmount = 1,
                TotalSalePrice = mainOrder.TotalSalePrice,
                Game = mainOrder.Game,
                GameName = mainOrder.GameName,
                Area = mainOrder.Area,
                AreaName = mainOrder.AreaName,
                Server = mainOrder.Server,
                ServerName = mainOrder.ServerName,
                RechargeMode = mainOrder.RechargeMode,
                RechargeModeName = mainOrder.RechargeModeName,
                StockMerchantId = mainOrder.StockMerchantId,
                StockMerchantName = mainOrder.StockMerchantName,
                CustomerIp = mainOrder.CustomerIp,
                CustomerRegion = mainOrder.CustomerRegion,
                DealDateTime = mainOrder.DealDateTime,
                StartDatetime = mainOrder.StartDatetime,
                EndDatetime = mainOrder.EndDatetime,
                RechargeStatus = mainOrder.RechargeStatus,
                SuccessfulAmount = mainOrder.SuccessfulAmount,
                ChargeAccountInfo = mainOrder.ChargeAccountInfo,
                RechargeMsg = mainOrder.RechargeMsg
            };

            return childOrder;
        }
    }
}
