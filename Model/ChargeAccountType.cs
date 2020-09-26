using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 代充帐号（卡密）类型
    /// </summary>
    public  class ChargeAccountType
    {
        public int ChargeAccountTypeID { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }//创建时间
    }
}
