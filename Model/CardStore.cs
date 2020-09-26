using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CardStore
    {
        public int ID { get; set; }
        public int CardTypeID { get; set; }
        public string CardTypeDescription { get; set; }
        public int CardValue { get; set; }
        public int TotalCount { get; set; }
        public int UntreatedCount { get; set; }
        public int ProcessingCount { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailureCount { get; set; }
        public int SuspiciousCount { get; set; }

        //[Description("未处理")]
        //untreated = 0,
        //[Description("处理中")]
        //processing = 1,
        //[Description("充值成功")]
        //successful = 2,
        //[Description("充值失败")]
        //failure = 3,
        //[Description("充值存疑")]
        //suspicious = 4,
    }
}
