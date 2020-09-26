using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 代充帐号（卡密）
    /// </summary>
    public class OrderChargeAccount
    {
        public int OrderChargeAccountID { get; set; }
        public int ChargeAccountTypeID { get; set; } //账号类型,卡类型
        public string ChargeAccount { get; set; }//代充账号，卡号
        public string ChargePassword { get; set; }//代充密码,卡密
        public string PayPassword { get; set; } //支付密码
        public int ParValue { get; set; } //卡密面值,账号总金额
        public decimal Balance { get; set; } //余额
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime LastUseTime { get; set; }//最后使用时间
        public bool isActive { get; set; }  //是否可用  
        public int UserNumber { get; set; }//使用次数
        public bool IsUsing { get; set; }//是否正在使用
    }
}
