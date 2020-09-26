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
    public partial class NotifyFromRuiLian : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string key = "8af8ef3102bd5361ce69a7c12989a4dd";


            string ste = Request["ste"]; //	订单状态	必选	String	是	订单状态0为成功，1为失败
            string cid = Request["cid"]; //	商家ID	必选	String	是	商家在我们系统注册的ID
            string oid = Request["oid"]; //	商家订单号	必选	String	是	商家系统生成的ID
            string pn = Request["pn"]; //	充值帐号	必选	String	是	充值帐号
            string sign = Request["sign"]; //	MD5签名	必选	String	是	原串拼接规则:ste+cid+oid+pn+key 例:0+test+101447975+123456+test
            string info1 = Request["info1"]; //	商家自定义	必选	String	否	可选参数，原样返回（下单商品为卡密商品类型时，这个字段将存放卡密，提取时需用标准3DES解密算法解Decrypt（info1,key）
            string info2 = Request["info2"]; //	商家自定义	必选	String	否	可选参数，原样返回


            WriteLog.Write("方法:RuiLian回调，OrderID：" + oid + " cid:" + cid +
                "State:" + ste , LogPathFile.Other.ToString());

            string md5str = ste + cid + oid + pn + key;

            string checkSign = Md5Helper.MD5Encrypt(md5str);

            if (sign == checkSign)
            {
                int RechargeStatus = (int)OrderRechargeStatus.processing;
                string RechargeMsg = string.Empty;

                if (ste.Equals("1"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.successful;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful);
                }
                else if (ste.Equals("2"))
                {
                    RechargeStatus = (int)OrderRechargeStatus.failure;
                    RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure);
                }
                else 
                {//充值中

                }

                new SQLOrder().UpdateNotifyOrderBySql(oid, RechargeStatus, RechargeMsg);
            }
            else
            {
                WriteLog.Write("方法:RuiLian回调 本地系统订单号:" + oid +
              "签名验证错误，Kamen数字签名" + sign + "本地签名:" + checkSign, LogPathFile.Other.ToString());
            }


            Response.Write("success"); 
        }
    }
}