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
using Common.UUWise;

namespace ChargeInterface.Charge
{
    public class pay_100bt_com
    {
        public Order Charge(Order order)
        {
            try
            {
                decimal totalPrice = (decimal)(order.ProductParValue * order.BuyAmount);
                decimal totalPriceFixed = totalPrice;
                int isContinue = 0;

                while (totalPrice > 0)
                {
                    if(isContinue>3)
                    {//充值频繁，重试三次
                        break;
                    }

                    decimal aobiMoney = 0; //澳币金额
                    decimal rmb = 0;  //实际付款金额
                    if (totalPrice >= 20)
                    {
                        aobiMoney = 20;
                        rmb = 30;
                        totalPrice = totalPrice - aobiMoney;
                    }
                    else if (totalPrice >= 10)
                    {
                        aobiMoney = 10;
                        rmb = 15;
                        totalPrice = totalPrice - aobiMoney;
                    }

                    int checkAccountNum = 0;
                    string captcha = "";  //验证码
                    int codeid = 0;   //验证码id
                    string result = "";
                    CookieContainer cookie = new CookieContainer(); ;
                    do
                    {
                        #region 验证帐号，第一步提交
                        if (checkAccountNum > 10)
                        {
                            order.RechargeMsg = "验证码错误";
                            break;
                        }

                        cookie = new CookieContainer();
                        result = PostAndGet.HttpGetString("http://pay.100bt.com/balance/phone.jsp", "", ref cookie);
                      
                        WrapperHelp.GetCodeByByte_UU("http://pay.100bt.com/imagecaptcha.action?actionType=2&rnd=1480340570946", ref  cookie, 1004, ref captcha, ref codeid);

                        StringBuilder checkPostdataBuilder = new StringBuilder();
                        checkPostdataBuilder.AppendFormat("Account={0}", order.TargetAccount);
                        checkPostdataBuilder.AppendFormat("&ConfirmAccount={0}", order.TargetAccount);
                        checkPostdataBuilder.AppendFormat("&OpId={0}", 100);
                        checkPostdataBuilder.AppendFormat("&captcha={0}", captcha);
                        checkPostdataBuilder.AppendFormat("&GameId={0}", 0);
                        checkPostdataBuilder.AppendFormat("&AobiMoney={0}", aobiMoney);
                        checkPostdataBuilder.AppendFormat("&RMB={0}", rmb);
                        checkPostdataBuilder.AppendFormat("&PayType={0}", 37);
                        checkPostdataBuilder.AppendFormat("&rnd={0}", "1480313845556");
                        checkPostdataBuilder.AppendFormat("&payType={0}", 37);

                        result = PostAndGet.HttpPostString("http://pay.100bt.com/check.action", checkPostdataBuilder.ToString(), ref cookie);

                        WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                            + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

                        if (result.Contains("帐号不存在"))
                        {
                            order.RechargeStatus = (int)OrderRechargeStatus.failure;
                            order.RechargeMsg = "帐号不存在";
                            return order;
                        }
                        checkAccountNum++;
                        #endregion

                    } while (result.Contains("验证码错误"));

                    #region 确认提交
                    //Account=24875958&ConfirmAccount=24875958&logos=2&province_select=0&VPayNumber=&VPayPassword=&AobiMoney=20&captcha=1933&OpId=100&payType=37&GameId=0&Position=1
                    StringBuilder orderpayPostdataBuilder = new StringBuilder();
                    orderpayPostdataBuilder.AppendFormat("Account={0}", order.TargetAccount);
                    orderpayPostdataBuilder.AppendFormat("&ConfirmAccount={0}", order.TargetAccount);
                    orderpayPostdataBuilder.AppendFormat("&logos={0}", 2);
                    orderpayPostdataBuilder.AppendFormat("&province_select={0}", 0);
                    orderpayPostdataBuilder.AppendFormat("&VPayNumber={0}", 0);
                    orderpayPostdataBuilder.AppendFormat("&VPayPassword={0}", 10);
                    orderpayPostdataBuilder.AppendFormat("&AobiMoney={0}", aobiMoney);
                    orderpayPostdataBuilder.AppendFormat("&captcha={0}", captcha);
                    orderpayPostdataBuilder.AppendFormat("&OpId={0}", "100");
                    orderpayPostdataBuilder.AppendFormat("&payType={0}", 37);
                    orderpayPostdataBuilder.AppendFormat("&GameId={0}", 0);
                    orderpayPostdataBuilder.AppendFormat("&Position={0}", 1);

                    result = PostAndGet.HttpPostString("http://pay.100bt.com/orderpay.jsp", orderpayPostdataBuilder.ToString(), ref cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                              + ",订单第二步提交返回：" + result, LogPathFile.Recharge);
                    #endregion

                    if (!result.Contains("正在连接中"))
                    {
                        totalPrice += aobiMoney;
                        continue;
                    }

                    #region Vbi钱包充值


                    string spid = Regex.Match(result, @"name='spid'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spname = Regex.Match(result, @"name='spname'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spoid = Regex.Match(result, @"name='spoid'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spreq = Regex.Match(result, @"name='spreq'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string sprec = Regex.Match(result, @"name='sprec'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string userid = Regex.Match(result, @"name='userid'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string userip = Regex.Match(result, @"name='userip'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spmd5 = Regex.Match(result, @"name='spmd5'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spcustom = Regex.Match(result, @"name='spcustom'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string spversion = Regex.Match(result, @"name='spversion'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string money = Regex.Match(result, @"name='money'\s+type=hidden value='(.*?)'>").Groups[1].Value;
                    string urlcode = Regex.Match(result, @"name='urlcode'\s+type=hidden value='(.*?)'>").Groups[1].Value;
  
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
                        order.RechargeMsg += "账户余额不足";
                        totalPrice += aobiMoney;
                        break;
                    }
                    result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie);
                    #endregion
   
                    #region 充值结果判断
                    if (result.Contains("成功"))
                    {
                        order.RechargeMsg += "充值成功||";
                        order.SuccessfulAmount += aobiMoney;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - rmb;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    {
                        order.RechargeMsg += "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                        order.SuccessfulAmount += aobiMoney;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - rmb;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("操作失败"))
                    {
                        order.RechargeMsg += "操作失败||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("充值过于频繁"))
                    {
                        order.RechargeMsg += "充值频繁||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("验证码输入不正确"))
                    {
                        order.RechargeMsg += "验证码输入不正确||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("账户余额不足"))
                    {
                        order.RechargeMsg += "账户余额不足||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false,false);
                    }
                    else if (result.Contains("支付密码不正确"))
                    {
                        order.RechargeMsg += "支付密码不正确||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else if (result.Contains("用户不存在"))
                    {
                        order.RechargeMsg += "用户不存在||";
                        totalPrice += aobiMoney;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else
                    {
                        order.RechargeMsg += "存疑||";
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    #endregion
                }

                #region 订单状态判断
                if (order.SuccessfulAmount == totalPriceFixed)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                //else if (order.SuccessfulAmount == 0)
                //{
                //    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                //}
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
  
    }
}
