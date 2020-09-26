using DBOperation.SQLHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EntityDB;
using Common;
using Common.LogHelper;
using System.Text;

namespace NotifyOrderWeb
{
    public partial class NotifyFromPanSuo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "427ee21dec9b49779f2cde44c4c04607";

         	string businessId=Request["businessId"];	//商户号 由平台分配每个商户唯一的一个商户号
            string userOrderId = Request["userOrderId"];	//	商户订单号（流水号） 最大长度不超过32位的唯一流水号
            string status = Request["status"];	//	充值结果 01  成功  02 失败
            string mes = Request["mes"];	//	结果说明 对充值的结果予以说明，特别是充值失败的时候需要对具体充值失败原因简单说明一下
            string kmInfo = Request["kmInfo"];	//	卡密信息 json字符串数组，格式为[{“cardNo” : “1234567”,”cardPwd” : “7654321”,”outDate”:”2015-12-29”},{“cardNo” : “1234567”,”cardPwd” : “7654321” ,”outDate”:”2015-12-29”}],密码要加密（采用3DES加密方式）
            string payoffPriceTotal = Request["payoffPriceTotal"];	//	结算总金额 系统和进货平台结算金额
            string sign = Request["sign"];	//	签名 case(md5(businessId + userOrderId + status +密钥))


            WriteLog.Write("方法:PanSuo回调，userOrderId：" + userOrderId + " status:" + status +
                "mes:" + mes, LogPathFile.Other.ToString());

            string md5str = businessId + userOrderId + status + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (status.Equals("01"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);
                }
                else if (status.Equals("02"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                }
                else 
                {//充值中

                }

                new SQLOrder().UpdateNotifyOrderBySql(userOrderId, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:PanSuo回调 本地系统订单号:" + userOrderId +
              "签名验证错误，PanSuo数字签名" + sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("<receive>ok</receive>"); 
        }
    }
}