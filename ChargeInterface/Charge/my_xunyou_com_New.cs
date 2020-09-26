using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChargeInterface.ChargeHelper.Charge.VbiHelper;
using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using Common.UUWise;
using ChargeInterface.ChargeHelper.OrderHelper;
using ChargeInterface.AntoInterface;

namespace ChargeInterface.Charge
{
    public class my_xunyou_com_New : ICharge
    {
        public Order Charge(Order order)
        {
            List<Order> LstOrder = new List<Order>();

            for (int i = 0; i < order.BuyAmount; i++)
            {
                Order childOrder = Common.OrderHelper.GenerateChildOrder(order, i);
                LstOrder.Add(childOrder);
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ChargeOrder), childOrder);
            }

            foreach (var item in LstOrder)
            {
                while (item.RechargeStatus == (int)OrderRechargeStatus.processing) ;

                switch (item.RechargeStatus)
                {
                    case (int)OrderRechargeStatus.successful:
                        order.SuccessfulAmount++;
                        break;
                    case (int)OrderRechargeStatus.failure:
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        break;
                    case (int)OrderRechargeStatus.suspicious:
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                    default:
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                }
                order.RechargeMsg += item.RechargeMsg + "||";
                order.ChargeAccountInfo += item.ChargeAccountInfo + "||";
            }


            #region 订单状态判断
            if (order.SuccessfulAmount >= order.BuyAmount)
            {
                order.RechargeStatus = (int)OrderRechargeStatus.successful;
            }
            else if (order.SuccessfulAmount == 0 && order.RechargeStatus == (int)OrderRechargeStatus.failure)
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
            }
            #endregion

            return order;
        }
        void ChargeOrder(object obj)
        {
            Order order = (Order)obj;

            #region 订单判断
            string payment = "";
            if (!Getpayment((int)order.ProductParValue, ref payment))
            {
                order.RechargeMsg = "充值面值不合法";
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                return;
            }

            if (CheckStrHelper.IsChinese(order.TargetAccount))
            {
                order.RechargeMsg = "充值帐号不能为中文";
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                return;
            }

            if (order.TargetAccount.Length < 6 || order.TargetAccount.Length > 20)
            {
                order.RechargeMsg = "请输入6~20位字母和数字";
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                return;
            }
            #endregion

            int count = 0;

            while (order.RechargeStatus == (int)OrderRechargeStatus.processing)
            {
                if (count > 3)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    break;
                }

                string order_id = string.Empty;
                string msg = ReCharge(order, payment, ref order_id);
                string orderStatus = OrderStatusForXml.GetOrderStatus("xunyou", msg);

                order.RechargeMsg += order_id + msg + "||";
                
                switch (orderStatus)
                {
                    case "成功":
                        order.RechargeStatus = (int)OrderRechargeStatus.successful;
                        break;
                    case "失败":
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        break;
                    case "重复":
                        order.RechargeStatus = (int)OrderRechargeStatus.processing;
                        break;
                    case "可疑":
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                    default:
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                        break;
                }
                count++;
            }
        }
        string ReCharge(Order order, string payment,ref string order_id)
        {
            #region 帐号登录

            CookieContainer cookie = new CookieContainer();

            if (!login(order, ref cookie))
                return"帐号登录失败";
            
            #endregion

            #region 获取参数 订单确认
            string postURl = "https://my.xunyou.com/index.php/payment/confirmPayment2014";
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("payment_object={0}", System.Web.HttpUtility.UrlEncode(payment, Encoding.Default));
            postData.AppendFormat("&isconversion={0}", "2");
            postData.AppendFormat("&product_id={0}", "2");
            postData.AppendFormat("&payment_login={0}", order.TargetAccount);
            postData.AppendFormat("&usernameinput={0}", order.TargetAccount);
            postData.AppendFormat("&payment_login_confirm={0}", order.TargetAccount);
            postData.AppendFormat("&payment_card={0}", "ICBC-NET");
            postData.AppendFormat("&selected_product_name={0}", "%E8%BF%85%E6%B8%B8VIP");
            postData.AppendFormat("&user_mask1={0}", "");
            postData.AppendFormat("&user_mask2={0}", "");
            postData.AppendFormat("&addition_product_id={0}", "");
            postData.AppendFormat("&userid={0}", "");
            postData.AppendFormat("&sub_product_class={0}", "");
            postData.AppendFormat("&client={0}", "	0");
            postData.AppendFormat("&payment_method={0}", "28");
            postData.AppendFormat("&eid={0}", "10612");
            postData.AppendFormat("&gameid={0}", "0");
            postData.AppendFormat("&gid={0}", "0");
            postData.AppendFormat("&cdkey={0}", "0");
            postData.AppendFormat("&payment_card_account={0}", "");
            postData.AppendFormat("&payment_card_password={0}", "");
            postData.AppendFormat("&payment_bank={0}", "8001");
            postData.AppendFormat("&vertifycode={0}", "");
            postData.AppendFormat("&mobile_num={0}", "");
            postData.AppendFormat("&packid={0}", "0");
            postData.AppendFormat("&mac={0}", "");
            postData.AppendFormat("&pageid={0}", "4");
            postData.AppendFormat("&pwd={0}", "");
            postData.AppendFormat("&spid={0}", "");
            postData.AppendFormat("&spid2={0}", "");
            string result = PostAndGet.HttpPostString(postURl, postData.ToString(), ref cookie, "https://my.xunyou.com/pay/?eid=10612");
            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
               + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

            if (result.Contains("用户不存在"))
                return  "用户不存在";
            if (result.Contains("请更换其他支付方式"))
                return  "帐号未设置安全工具";
            if (!result.Contains("确认订单"))
                return  "确认订单失败";

            #endregion

            #region Vbi钱包充值


            string spid = Regex.Match(result, @"value=""(.*?)"" name=""spid""").Groups[1].Value;
            string spname = Regex.Match(result, @"value=""(.*?)"" name=""spname""").Groups[1].Value;
            string spoid = Regex.Match(result, @"value=""(.*?)"" name=""spoid""").Groups[1].Value;
            string spreq = Regex.Match(result, @"value=""(.*?)"" name=""spreq""").Groups[1].Value;
            string sprec = Regex.Match(result, @"value=""(.*?)"" name=""sprec""").Groups[1].Value;
            string userip = Regex.Match(result, @"value=""(.*?)"" name=""userip""").Groups[1].Value;
            string spcustom = Regex.Match(result, @"value=""(.*?)"" name=""spcustom""").Groups[1].Value;
            string spversion = Regex.Match(result, @"value=""(.*?)"" name=""spversion""").Groups[1].Value;
            string money = Regex.Match(result, @"value=""(.*?)"" name=""money""").Groups[1].Value;
            string urlcode = Regex.Match(result, @"value=""(.*?)"" name=""urlcode""").Groups[1].Value;
            string spmd5 = Regex.Match(result, @"value=""(.*?)"" name=""spmd5""").Groups[1].Value;
            string userid = Regex.Match(result, @"value=""(.*?)"" name=""userid""").Groups[1].Value;
            string payment_object = Regex.Match(result, @"value=""(.*?)"" name=""payment_object""").Groups[1].Value;
            string payment_orderid = Regex.Match(result, @"value=""(.*?)"" name=""payment_orderid""").Groups[1].Value;
            string payment_login = Regex.Match(result, @"value=""(.*?)"" name=""payment_login""").Groups[1].Value;
            string payment_product = Regex.Match(result, @"value=""(.*?)"" name=""payment_product""").Groups[1].Value;
            string payment_paymoney = Regex.Match(result, @"value=""(.*?)"" name=""payment_paymoney""").Groups[1].Value;


            StringBuilder vnetonePostdataBuilder = new StringBuilder();
            vnetonePostdataBuilder.AppendFormat("spid={0}", spid);
            vnetonePostdataBuilder.AppendFormat("&spname={0}", System.Web.HttpUtility.UrlEncode(spname, Encoding.Default));
            vnetonePostdataBuilder.AppendFormat("&spoid={0}", spoid);
            vnetonePostdataBuilder.AppendFormat("&spreq={0}", System.Web.HttpUtility.UrlEncode(spreq, Encoding.Default));
            vnetonePostdataBuilder.AppendFormat("&sprec={0}", System.Web.HttpUtility.UrlEncode(sprec, Encoding.Default));
            vnetonePostdataBuilder.AppendFormat("&userip={0}", userip);
            vnetonePostdataBuilder.AppendFormat("&spcustom={0}", spcustom);
            vnetonePostdataBuilder.AppendFormat("&spversion={0}", spversion);
            vnetonePostdataBuilder.AppendFormat("&money={0}", money);
            vnetonePostdataBuilder.AppendFormat("&urlcode={0}", urlcode);
            vnetonePostdataBuilder.AppendFormat("&spmd5={0}", spmd5);
            vnetonePostdataBuilder.AppendFormat("&userid={0}", userid);
            vnetonePostdataBuilder.AppendFormat("&payment_object={0}", System.Web.HttpUtility.UrlEncode(payment_object, Encoding.Default));
            vnetonePostdataBuilder.AppendFormat("&payment_orderid={0}", payment_orderid);
            vnetonePostdataBuilder.AppendFormat("&payment_login={0}", payment_login);
            vnetonePostdataBuilder.AppendFormat("&payment_product={0}", System.Web.HttpUtility.UrlEncode(payment_product, Encoding.Default));
            vnetonePostdataBuilder.AppendFormat("&payment_paymoney={0}", System.Web.HttpUtility.UrlEncode(payment_paymoney, Encoding.Default));


            OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.Vbi);

            if (orderChargeAccount == null)
                return  "未取到v币帐号";

            result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie);

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
           + ",V币钱包充值返回：" + result, LogPathFile.Recharge);
            #endregion

            #region 迅游订单查询
            result = PostAndGet.HttpGetString_XY("https://my.xunyou.com/index.php/payment/getOrderState/" + spoid, "", ref cookie);

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
          + ",订单查询结果：" + result, LogPathFile.Recharge);

            string xyOrderStatus = Regex.Match(result, @"{""state"":(.*?),").Groups[1].Value;
            string xyfpaysuccess = Regex.Match(result, @"""fpaysuccess"":""(.*?)"",").Groups[1].Value;

            order_id = spoid;

            if (xyOrderStatus == "1")
            {
                if (xyfpaysuccess == "1")
                {
                   
                    orderChargeAccount.Balance = orderChargeAccount.Balance - order.ProductParValue;
                    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                     return "充值成功";
                }
                else if (xyfpaysuccess == "0")
                {
                    
                    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    return "未支付成功";
                }
                else
                {
                    
                    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    return  "存疑";
                }
            }
            else
            {
                SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                return  "存疑";
            }

            #endregion
        } 

        bool login(Order order, ref CookieContainer cookie)
        {
            try
            {
                OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.XunYou, false);

                cookie = Common.CookieOperation.CookieHelper.ReadCookiesFromDisk(orderChargeAccount.ChargeAccount);

                string result = PostAndGet.HttpGetString_XY("http://my.xunyou.com/index.php/uCenter/getLoginId", "", ref cookie);
                if (!result.Contains("-1"))
                    return true;
                
                int loginCount = 0;
                while (loginCount < 5)
                {
                    result = PostAndGet.HttpGetString_XY("https://my.xunyou.com/u/", "", ref cookie);

                    string code = ""; //验证码
                    int codeid = 0;
                    WrapperHelp.GetCodeByByte_UU("https://my.xunyou.com/index.php/imageoutput/VertifyCode/ver_code_1/50/24", ref cookie, 1005, ref code, ref codeid);
                    string LoginData = "regfrom=uCenter&agree_rule=1&loginid=" + orderChargeAccount.ChargeAccount + "&password=" + orderChargeAccount.ChargePassword + "&code=" + code;
                    string LoginUrl = "https://my.xunyou.com/index.php/login/ajaxLoginGj";
                    result = PostAndGet.HttpPostString_XY(LoginUrl, LoginData, ref cookie, "my.xunyou.com", "https://my.xunyou.com/u/");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + orderChargeAccount.ChargeAccount + "||" + orderChargeAccount.ChargePassword
                        + ",帐号登录返回：" + result, LogPathFile.Recharge);

                    string msg = Regex.Match(result, @"""msg"":""(.*?)""").Groups[1].Value;
                    string encodingMsg = "";
                    TypeCast.GetString(msg,ref encodingMsg);

                    if (encodingMsg.Contains("登录成功"))
                    {
                        Common.CookieOperation.CookieHelper.WriteCookiesToDisk(orderChargeAccount.ChargeAccount, cookie);
                        return true;
                    }
                    else
                    {
                        if (encodingMsg.Contains("验证码错误") || result.Contains("验证码错误"))
                        {
                            WrapperHelp.reportError(codeid);
                        }

                        loginCount++;
                        Thread.Sleep(1 * 1000);
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        bool Getpayment(int productParValue, ref string payment)
        {
            // 1|5|2, 2|10|5, 3|15|7,4|20|10,5|30|20 (位数+V币+天数)

            bool result = true ;
            switch (productParValue)
            {
                case 5:
                    payment = "1|5|2";
                    break;
                case 10:
                    payment = "2|10|5";
                    break;
                case 15:
                    payment = "3|15|7";
                    break;
                case 20:
                    payment = "4|20|10";
                    break;
                case 30:
                    payment = "5|30|20";
                    break;
                default :
                    result = false;
                    break;
            }
            return result;
        }

    }
}
