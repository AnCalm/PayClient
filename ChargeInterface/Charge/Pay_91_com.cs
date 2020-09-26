using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChargeInterface.ChargeHelper.Charge.VbiHelper;
using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;

namespace ChargeInterface.Charge
{
    public class Pay_91_com
    {
        public Order Charge(Order order)
        {
            try
            {
                CookieContainer cc = new CookieContainer();

                string sd = PostAndGet.HttpGetString_91y("http://member.91y.com/login/login.aspx?t=0.15243408223597132", "", ref cc);

                ChargeInterface.ChargeHelper.SlideVerification.SlideVerification.VerificationFor91y(cc);

                decimal totalPrice = (decimal)order.BuyAmount;
                decimal totalPriceFixed = totalPrice;
                int isContinue = 0;
                int pay_amount_Value = 0;

                if(order.ProductParValue==3)
                {
                    pay_amount_Value = 5;
                }
                else if (order.ProductParValue == 30)
                {
                    pay_amount_Value = 50;
                }
                else
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "充值数量错误";
                    return order;
                }

                CookieContainer cookie = new CookieContainer();
                string checkAccount = PostAndGet.HttpGetString_91y("https://pay.91y.com/servlet/do.ashx?a=changeuser&acc=" + order.TargetAccount + "&t=0.48772043911252916", "", ref cookie, "pay.91y.com", "https://pay.91y.com/phonecard/");
                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                      + ",帐号检测提交返回：" + checkAccount, LogPathFile.Recharge);

                //[{Accounts: 'c8ac1023fdf01aef080dac852f72fb1b',Nickname: '%e4%b8%9c%e6%96%b9%e8%88%9e%e6%94%92',GameID: '226124557'}]

                string Accounts = Regex.Match(checkAccount, @"Accounts: '(.*?)'").Groups[1].Value;
                string Nickname = Regex.Match(checkAccount, @"Nickname: '(.*?)'").Groups[1].Value;
                string GameID = Regex.Match(checkAccount, @"GameID: '(.*?)'").Groups[1].Value;
                if (Accounts.Equals("0"))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "帐号错误";
                    return order;
                }

                while (totalPrice > 0)
                {
                    #region 提交充值
                   
                    if (isContinue > 3) //充值频繁，重试三次
                        break;

                    totalPrice--;

                    #region 参数提交
                    string tradeverifyUrl = "https://pay.91y.com/tradeverify/";
                    StringBuilder tradeverifyStr = new StringBuilder();
                    tradeverifyStr.AppendFormat("user_qq={0}", "");
                    tradeverifyStr.AppendFormat("&pay_User={0}", Accounts);
                    tradeverifyStr.AppendFormat("&pd_FrpId={0}", "TEL-NET");
                    tradeverifyStr.AppendFormat("&pay_amount={0}", pay_amount_Value);
                    tradeverifyStr.AppendFormat("&pay_type={0}", "4");
                    tradeverifyStr.AppendFormat("&step={0}", "2");
                    string result = PostAndGet.HttpPostString_91y(tradeverifyUrl, tradeverifyStr.ToString(), ref cookie, null, "https://pay.91y.com/tel/");
                  
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                      + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

                    if(result.Contains("无法继续充值"))
                    {
                        order.RechargeMsg += "充值已达每日上限";
                        totalPrice++;
                        break;
                    }
                    if (result.Contains("请使用其他方式充值"))
                    {
                        order.RechargeMsg += "短信充值不能超过50，请使用其他方式充值";
                        totalPrice++;
                        break;
                    }

                    #endregion

                    #region 确认提交
                    string pay_amount = Regex.Match(result, @"id=""pay_amount""\s+name=""pay_amount""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_BankId = Regex.Match(result, @"id=""pay_BankId""\s+name=""pay_BankId""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_ItemUser = Regex.Match(result, @"id=""pay_User""\s+name=""pay_ItemUser""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_UserID = Regex.Match(result, @"id=""pay_UserID""\s+name=""pay_UserID""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_OrderId = Regex.Match(result, @"id=""pay_OrderId""\s+name=""pay_OrderId""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_ItemName = Regex.Match(result, @"id=""pay_ItemName""\s+name=""pay_ItemName""\s+value=""(.*?)""").Groups[1].Value;
                    string pay_Type = Regex.Match(result, @"id=""pay_Type""\s+name=""pay_Type""\s+value=""(.*?)""").Groups[1].Value;
                    string step = Regex.Match(result, @"id=""pay_step""\s+name=""step""\s+value=""(.*?)""").Groups[1].Value;

                    string servletUrl = "https://pay.91y.com/servlet/PayRequest.aspx";
                    StringBuilder servletStr = new StringBuilder();
                    servletStr.AppendFormat("pay_amount={0}", pay_amount);
                    servletStr.AppendFormat("&pay_BankId={0}", pay_BankId);
                    servletStr.AppendFormat("&pay_ItemUser={0}", pay_ItemUser);
                    servletStr.AppendFormat("&pay_UserID={0}", pay_UserID);
                    servletStr.AppendFormat("&pay_OrderId={0}", pay_OrderId);
                    servletStr.AppendFormat("&pay_ItemName={0}", pay_ItemName);
                    servletStr.AppendFormat("&pay_Type={0}", pay_Type);
                    servletStr.AppendFormat("&step={0}", step);
                    result = PostAndGet.HttpPostString_91y(servletUrl, servletStr.ToString(), ref cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                      + ",订单第二步提交返回：" + result, LogPathFile.Recharge);
                   

                    if (!result.Contains("订单提交"))
                    {
                        totalPrice ++;
                        continue;
                    }

                    #endregion

                    #region Vbi钱包充值

                    string spid = Regex.Match(result, @"name='spid'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spname = Regex.Match(result, @"name='spname'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spoid = Regex.Match(result, @"name='spoid'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spreq = Regex.Match(result, @"name='spreq'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string sprec = Regex.Match(result, @"name='sprec'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string userid = Regex.Match(result, @"name='userid'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string userip = Regex.Match(result, @"name='userip'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spmd5 = Regex.Match(result, @"name='spmd5'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spcustom = Regex.Match(result, @"name='spcustom'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string spversion = Regex.Match(result, @"name='spversion'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string money = Regex.Match(result, @"name='money'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;
                    string urlcode = Regex.Match(result, @"name='urlcode'\s+type=hidden\s+value='(.*?)'").Groups[1].Value;

                    StringBuilder vnetonePostdataBuilder = new StringBuilder();
                    vnetonePostdataBuilder.AppendFormat("spid={0}", spid);
                    vnetonePostdataBuilder.AppendFormat("&spname={0}", System.Web.HttpUtility.UrlEncode(spname, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&spoid={0}", spoid);
                    vnetonePostdataBuilder.AppendFormat("&spreq={0}", System.Web.HttpUtility.UrlEncode(spreq, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&sprec={0}", System.Web.HttpUtility.UrlEncode(sprec, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&userid={0}", userid);
                    vnetonePostdataBuilder.AppendFormat("&userip={0}", userip);
                    vnetonePostdataBuilder.AppendFormat("&spmd5={0}", spmd5);
                    vnetonePostdataBuilder.AppendFormat("&spcustom={0}", System.Web.HttpUtility.UrlEncode(spcustom, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&spversion={0}", spversion);
                    vnetonePostdataBuilder.AppendFormat("&money={0}", money);
                    vnetonePostdataBuilder.AppendFormat("&urlcode={0}", urlcode);

                    OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.Vbi);

                    if (orderChargeAccount == null)
                    {
                        order.RechargeMsg += "未取到v币帐号||";
                        totalPrice++;
                        isContinue++;
                        continue;
                    }

                    result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie);
                    #endregion

                    #region 充值结果判断
                    if (result.Contains("成功") || result.Contains("您已成功充值"))
                    {
                        order.RechargeMsg += "充值成功||";
                        order.SuccessfulAmount ++;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - pay_amount_Value;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    {
                        order.RechargeMsg += "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                        order.SuccessfulAmount ++;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - pay_amount_Value;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("操作失败"))
                    {
                        order.RechargeMsg += "操作失败||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("充值过于频繁"))
                    {
                        order.RechargeMsg += "充值频繁||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("验证码输入不正确"))
                    {
                        order.RechargeMsg += "验证码输入不正确||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("账户余额不足"))
                    {
                        order.RechargeMsg += "账户余额不足||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else if (result.Contains("支付密码不正确"))
                    {
                        order.RechargeMsg += "支付密码不正确||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else if (result.Contains("用户不存在"))
                    {
                        order.RechargeMsg += "用户不存在||";
                        totalPrice ++;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else
                    {
                        order.RechargeMsg += "存疑||";
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    #endregion

                    #endregion
                }

                #region 订单状态判断
                if (order.SuccessfulAmount >= totalPriceFixed)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                else if (order.SuccessfulAmount == 0 && order.RechargeMsg.Contains("充值已达每日上限"))
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
            catch (Exception ex)
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",充值异常信息：" + ex.Message, LogPathFile.Exception);

                return order;
            }
        }
    }
}
