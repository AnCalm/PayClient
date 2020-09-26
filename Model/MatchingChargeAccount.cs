using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 订单匹配代充帐号（卡密）
    /// </summary>
    public class MatchingChargeAccount
    {
        public int MatchingChargeAccountID { get; set; }
        public int OrderID { get; set; }
        public int OrderChargeAccountID { get; set; }
        public DateTime CreateTime { get; set; }//创建时间

    }
}
