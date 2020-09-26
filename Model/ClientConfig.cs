using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 挂机配置类
    /// </summary>
    class ClientConfig
    {
        public int ClientConfigID { get; set; }
        public string MerchantCode { get; set;} //商户编号
        public string MerchantKey { get; set; } //商户密钥
        public string GetOrdersURL { get; set; } //获取订单地址
        public string NotifyOrderURL { get; set; }//通知订单地址
        public string WinSkin { get; set; }//窗体皮肤
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime UpdateTime { get; set; }//最后一次修改时间
    }
}
