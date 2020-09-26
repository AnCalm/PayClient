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

namespace ChargeInterface.Charge
{
    public class my_xunyou_com
    {
        public Order Charge(Order order)
        {
            try
            {
                decimal totalPrice = (decimal)order.BuyAmount;
                decimal totalPriceFixed = totalPrice;
                int isContinue = 0;

                
                string payment = "";
                if (!Getpayment((int)order.ProductParValue, ref payment))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "充值面值不合法";
                    return order;
                }

                if (CheckStrHelper.IsChinese(order.TargetAccount))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "充值帐号不能为中文";
                    return order;
                }

                if (order.TargetAccount.Length < 6 || order.TargetAccount.Length > 20)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "请输入6~20位字母和数字";
                    return order;
                }

                CookieContainer cookie = new CookieContainer();

                while (totalPrice > 0)
                {
                    #region 提交充值
                    if (isContinue > 3) //充值频繁，重试三次
                        break;

                    totalPrice--; ;

                    #region 帐号登录
                    string result = PostAndGet.HttpGetString_XY("http://my.xunyou.com/index.php/uCenter/getLoginId", "", ref cookie);
                    if (result.Contains("-1"))
                    {
                        if (!login(order, ref cookie))
                        {
                            order.RechargeMsg = "帐号登录失败";
                            totalPrice++;
                            isContinue++;
                            continue;
                        }
                    }
                    #endregion

                    #region 获取参数 订单确认
                    string postURl = "https://my.xunyou.com/index.php/payment/confirmPayment2014";
                    StringBuilder postData = new StringBuilder();
                    postData.AppendFormat("payment_object={0}",System.Web.HttpUtility.UrlEncode( payment,Encoding.Default));
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
                    result = PostAndGet.HttpPostString(postURl, postData.ToString(), ref cookie, "https://my.xunyou.com/pay/?eid=10612");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                       + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

                    if (result.Contains("用户不存在"))
                    {
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        order.RechargeMsg = "用户不存在";
                        return order;
                    }
                    if(result.Contains("请更换其他支付方式"))
                    {
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        order.RechargeMsg = "帐号未设置安全工具";
                        return order;
                    }

                    if (!result.Contains("确认订单"))
                    {
                        totalPrice++;
                        isContinue++;
                        continue;
                    }

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

                    if (orderChargeAccount==null)
                    {
                        order.RechargeMsg += "未取到v币帐号||";
                        totalPrice++;
                        isContinue++;
                        continue;
                    }
                    result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                   + ",V币钱包充值返回：" + result, LogPathFile.Recharge);
                    #endregion

                    #region 迅游订单查询
                    result = PostAndGet.HttpGetString_XY("https://my.xunyou.com/index.php/payment/getOrderState/" + spoid, "", ref cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                  + ",订单查询结果：" + result, LogPathFile.Recharge);

                    string xyOrderStatus = Regex.Match(result,@"{""state"":(.*?),").Groups[1].Value;
                    string xyfpaysuccess = Regex.Match(result, @"""fpaysuccess"":""(.*?)"",").Groups[1].Value;
                    

                    if (xyOrderStatus=="1")
                    {
                        if (xyfpaysuccess == "1")
                        {
                            order.RechargeMsg += spoid + "充值成功||";
                            order.SuccessfulAmount += order.ProductParValue;
                            orderChargeAccount.Balance = orderChargeAccount.Balance - order.ProductParValue;
                            SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        }
                        else if (xyfpaysuccess == "0")
                        {
                            order.RechargeMsg += spoid + "未支付成功||";
                            totalPrice++;
                            SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                            isContinue = isContinue+6;
                        }
                        else
                        {
                            order.RechargeMsg += spoid + "存疑||";
                            SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        }
                    }
                    else
                    {
                        order.RechargeMsg += spoid + "存疑||";
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }

                    #endregion

                    #region VB充值结果判断
                    //if (result.Contains("成功") || result.Contains("您已成功充值") || result.Contains("成功充值"))
                    //{
                    //    order.RechargeMsg += "充值成功||";
                    //    order.SuccessfulAmount += order.ProductParValue;
                    //    orderChargeAccount.Balance = orderChargeAccount.Balance - order.ProductParValue;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //}
                    //else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    //{
                    //    order.RechargeMsg += "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                    //    order.SuccessfulAmount += order.ProductParValue;
                    //    orderChargeAccount.Balance = orderChargeAccount.Balance - order.ProductParValue;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //}
                    //else if (result.Contains("操作失败"))
                    //{
                    //    order.RechargeMsg += "操作失败||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //    isContinue++;
                    //}
                    //else if (result.Contains("充值过于频繁"))
                    //{
                    //    order.RechargeMsg += "充值频繁||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //    isContinue++;
                    //}
                    //else if (result.Contains("验证码输入不正确"))
                    //{
                    //    order.RechargeMsg += "验证码输入不正确||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //    isContinue++;
                    //}
                    //else if (result.Contains("账户余额不足"))
                    //{
                    //    order.RechargeMsg += "账户余额不足||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    //}
                    //else if (result.Contains("支付密码不正确"))
                    //{
                    //    order.RechargeMsg += "支付密码不正确||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    //}
                    //else if (result.Contains("用户不存在"))
                    //{
                    //    order.RechargeMsg += "用户不存在||";
                    //    totalPrice++;
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    //}
                    //else
                    //{
                    //    order.RechargeMsg += "存疑||";
                    //    SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    //}
                    #endregion

                    #endregion
                }

                #region 订单状态判断
                if (order.SuccessfulAmount >= totalPriceFixed)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                else
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                }
                #endregion

                return order;
            }
            catch (Exception ex)
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",充值一场信息：" + ex.Message, LogPathFile.Exception);

                return order;
            }
        }
        bool login(Order order, ref CookieContainer cookie)
        {
            try
            {
                int loginCount = 0;
                while (loginCount < 5)
                {
                    cookie = new CookieContainer();

                    string result = PostAndGet.HttpGetString_XY("https://my.xunyou.com/u/", "", ref cookie);

                    OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.XunYou, false);
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
