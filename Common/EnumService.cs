using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace Common
{
    public class EnumService
    {
        public static string GetDescription(Enum obj)
        {
            string objName = obj.ToString();
            Type t = obj.GetType();
            FieldInfo fi = t.GetField(objName);
            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return arrDesc[0].Description;
        }

        public static string GetDescription(int status)
        {
            string description = "未处理";
            switch (status)
            {
                case 0:
                    description = GetDescription(OrderRechargeStatus.untreated);
                    break;
                case 1:
                    description = GetDescription(OrderRechargeStatus.processing);
                    break;
                case 2:
                    description = GetDescription(OrderRechargeStatus.successful);
                    break;
                case 3:
                    description = GetDescription(OrderRechargeStatus.failure);
                    break;
                case 4:
                    description = GetDescription(OrderRechargeStatus.suspicious);
                    break;
                case 5:
                    description = GetDescription(OrderRechargeStatus.Submit);
                    break;
            }

            return description;
        }
    }

    public enum OrderRechargeStatus
    {
        [Description("未处理")]
        untreated = 0,
        [Description("处理中")]
        processing = 1,
        [Description("充值成功")]
        successful = 2,
        [Description("充值失败")]
        failure = 3,
        [Description("充值存疑")]
        suspicious = 4,
        [Description("已提交")]
        Submit = 5,
    }

    public class  OrderChargeAccountType
    {
       /// <summary>
       /// V币
       /// </summary>
        public const string Vbi = "Vbi";

        /// <summary>
        /// 骏卡
        /// </summary>
        public const string JCard = "JCard";

        /// <summary>
        /// 电信卡
        /// </summary>
        public const string DxCard = "DxCard";

        /// <summary>
        /// 盛付通
        /// </summary>
        public const string SdCard = "SdCard";

        /// <summary>
        /// 红袖
        /// </summary>
        public const string HongXiu = "HongXiu";

        /// <summary>
        /// 迅游
        /// </summary>
        public const string XunYou = "XunYou";

        /// <summary>
        /// 纵游
        /// </summary>
        public const string ZYCard = "ZYCard";

        /// <summary>
        /// 米米卡
        /// </summary>
        public const string MMCard = "MMCard";

        /// <summary>
        /// 久游
        /// </summary>
        public const string JiuYou = "9you";

        /// <summary>
        /// 天宏
        /// </summary>
        public const string THCard = "THCard";
    }

    public class LogPathFile
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        public const string Exception = "Exception";

        /// <summary>
        /// 充值日志
        /// </summary>
        public const string Recharge = "Recharge";

        /// <summary>
        /// 其他日志
        /// </summary>
        public const string Other = "Other";

        /// <summary>
        /// 文件日志
        /// </summary>
        public const string FileEx = "FileEx";
    }

    public enum LogType
    {
        /// <summary>
        /// 此枚举指示每小时创建一个新的日志文件
        /// </summary>
        Hourly,
        /// <summary>
        /// 此枚举指示每天创建一个新的日志文件
        /// </summary>
        Daily,

        /// <summary>
        /// 此枚举指示每周创建一个新的日志文件
        /// </summary>
        Weekly,

        /// <summary>
        /// 此枚举指示每月创建一个新的日志文件
        /// </summary>
        Monthly,

        /// <summary>
        /// 此枚举指示每年创建一个新的日志文件
        /// </summary>
        Annually
    }

    public class MerchantCodeType
    {
        /// <summary>
        /// 数网
        /// </summary>
        public const string SW = "SW";

        /// <summary>
        /// 易约
        /// </summary>
        public const string SUP = "SUP";
    }

    public class AreaValueType
    {
        public const string KaMenWang = "12";
        public const string ShuWang = "13";
        public const string MBJ = "14";
        public const string BaiYou = "15";
        public const string ShuShan = "1021";
        public const string _99vip = "2023";
    }
}
