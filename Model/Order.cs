using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   /// <summary>
   /// 订单
   /// </summary>
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderInsideID{ get; set; } //系统订单号
        public string OrderExternalID{ get; set; }//商户订单号 
        public string ProductID{ get; set; }//商品编号
        public string ProductName{ get; set; }//商品名称
        public decimal ProductParValue{ get; set; }//商品面值
        public decimal ProductSalePrice{ get; set; }//商品销售单价
        public string TargetAccount{ get; set; }//充值帐号
        public string TargetAccountType{ get; set; }//充值帐号类型
        public string TargetAccountTypeName{ get; set; }//充值帐号类型名称
        public int  BuyAmount{ get; set; }//购买数量
        public decimal TotalSalePrice{ get; set; }//订单总额
        public string Game{ get; set; }//游戏类型
        public string GameName{ get; set; }//游戏类型名称
        public string Area{ get; set; }//充值区域
        public string AreaName{ get; set; }//充值区域名称
        public string Server{ get; set; }//充值服务器
        public string ServerName{ get; set; }//充值服务器名称
        public string RechargeMode{ get; set; }//充值方式
        public string RechargeModeName{ get; set; }//充值方式名称
        public string StockMerchantId{ get; set; }//进货商家编号
        public string StockMerchantName{ get; set; }//进货商家名称
        public string CustomerIp{ get; set; }//客户IP
        public string CustomerRegion{ get; set; }//客户区域
        public DateTime DealDateTime{ get; set; }//订单交易时间

        public DateTime StartDatetime{ get; set; }  //订单开始时间
        public DateTime EndDatetime{ get; set; }    //订单结束时间
        public int RechargeStatus{ get; set; }  //订单充值状态
        public decimal SuccessfulAmount{ get; set; }   //订单成功金额
        public string  ChargeAccountInfo{ get; set; } //订单代充账号(卡密)信息
        public string RechargeMsg { get; set; }  //充值结果描述
    }

}
