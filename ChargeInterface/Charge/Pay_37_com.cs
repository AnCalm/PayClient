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
    public class Pay_37_com
    {
        public Order Charge(Order order)
        {
            try
            {
                CookieContainer cookie = new CookieContainer();

                decimal totalPrice = (decimal)(order.BuyAmount*2);
                decimal totalPriceFixed = totalPrice;
                int isContinue = 0;

                if (order.BuyAmount % 5 != 0)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "购买数量不合法，请提交5的倍数";
                    return order;
                }
             
                while (totalPrice > 0)
                {
                    #region 提交充值
                    int chargeVbiNum=0;
                    
                    if (isContinue > 3) //充值频繁，重试三次
                        break;

                    if (!GetChargeNum((int)totalPrice, ref chargeVbiNum))
                       break ;

                    totalPrice = totalPrice - chargeVbiNum;
                   
                    string result = "";
                    

                    #region 获取参数 帐号判断
                    result = PostAndGet.HttpGetString("http://pay.37.com/", "", ref cookie);
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                       + ",充值页面提交返回：" + result, LogPathFile.Recharge);
                  
                    string f_token = Regex.Match(result, @"name=\""f_token\"" id=\""confirm_f_token\"" value=\""(.*?)\""\>").Groups[1].Value;
                    string g_f_token = Regex.Match(result, @"g_f_token : '(.*?)',").Groups[1].Value;
                    
                    StringBuilder checkgetDataBuilder = new StringBuilder();
                    checkgetDataBuilder.AppendFormat("action={0}", "check_user");
                    checkgetDataBuilder.AppendFormat("&user_name={0}", order.TargetAccount);
                    result = PostAndGet.HttpGetString("http://pay.37.com/controller/user.php", checkgetDataBuilder.ToString(), ref cookie);
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                      + ",订单第一步提交返回：" + result, LogPathFile.Recharge);
                    

                    string msg = Regex.Match(result, @"""msg"":""(.*?)""}").Groups[1].Value;
                    string str = "";
                    TypeCast.GetString(msg, ref  str);
                    if (str.Contains("请求异常"))
                    {
                        //order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        //order.RechargeMsg = "帐号不存在";
                        //return order;
                        order.RechargeMsg += "帐号不存在||";
                        totalPrice++;
                        break;

                    }
                    #endregion

                    #region 确认提交
                    StringBuilder orderPostdataBuilder = new StringBuilder();
                    orderPostdataBuilder.AppendFormat("user_name={0}", order.TargetAccount);
                    orderPostdataBuilder.AppendFormat("&game_id={0}", "100000");
                    orderPostdataBuilder.AppendFormat("&server_id={0}", "100000"); ;
                    orderPostdataBuilder.AppendFormat("&money={0}", chargeVbiNum);
                    orderPostdataBuilder.AppendFormat("&pay_type={0}", "17");
                    orderPostdataBuilder.AppendFormat("&pay_for={0}", "platform");
                    orderPostdataBuilder.AppendFormat("&f_token={0}", g_f_token);
                    orderPostdataBuilder.AppendFormat("&phone={0}", "");
                    orderPostdataBuilder.AppendFormat("&actor={0}", "");
                    orderPostdataBuilder.AppendFormat("&pay_bank={0}", "");
                    orderPostdataBuilder.AppendFormat("&envelope_id={0}", "");
                    orderPostdataBuilder.AppendFormat("&safe_code={0}", "");
                    orderPostdataBuilder.AppendFormat("&pay_referer={0}", "");
                    orderPostdataBuilder.AppendFormat("&action={0}", "create_order");
                    orderPostdataBuilder.AppendFormat("&ajax={0}", "0");

                    result = PostAndGet.HttpPostString("http://pay.37.com/controller/order_v2.php", orderPostdataBuilder.ToString(), ref cookie);

                    string order_id = Regex.Match(result, @"order_id=(.*?)&").Groups[1].Value;

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                     + ",订单第二步提交返回：" + result, LogPathFile.Recharge);


                    StringBuilder payPostdataBuilder = new StringBuilder();
                    payPostdataBuilder.AppendFormat("action={0}", "go_pay");
                    payPostdataBuilder.AppendFormat("&order_id={0}", order_id);
                    payPostdataBuilder.AppendFormat("&f_token={0}", f_token);
                    payPostdataBuilder.AppendFormat("&p_code={0}", "");
                    payPostdataBuilder.AppendFormat("&th_cardno={0}", "");
                    payPostdataBuilder.AppendFormat("&th_cardpwd={0}", "");

                    result = PostAndGet.HttpPostString("http://pay.37.com/controller/paygate.php", payPostdataBuilder.ToString(), ref cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                     + ",订单第三步提交返回：" + result, LogPathFile.Recharge);


                    if (!result.Contains("正在跳转"))
                    {
                        totalPrice += chargeVbiNum;
                        isContinue++;
                        continue;
                    }

                    #endregion

                    #region Vbi钱包充值

                    string spid = Regex.Match(result, @"<input name='spid'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string spname = Regex.Match(result, @"<input name='spname'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string spoid = Regex.Match(result, @"<input name='spoid'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string spreq = Regex.Match(result, @"<input name='spreq'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string sprec = Regex.Match(result, @"<input name='sprec'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string userid = Regex.Match(result, @"<input name='userid'\s+type=hidden\s+value=""(.*?)""").Groups[1].Value;
                    string userip = Regex.Match(result, @"<input name='userip'\s+type=hidden\s+value=""(.*?)""").Groups[1].Value;
                    string spmd5 = Regex.Match(result, @"<input name='spmd5'\s+type=hidden\s+value=""(.*?)""").Groups[1].Value;
                    string spcustom = Regex.Match(result, @"<input name='spcustom'\s+type=hidden\s+value=""(.*?)""").Groups[1].Value;
                    string spversion = Regex.Match(result, @"<input name='spversion'\s+type=hidden\s+value=""(.*?)""").Groups[1].Value;
                    string money = Regex.Match(result, @"<input name='money'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;
                    string urlcode = Regex.Match(result, @"<input name='urlcode'\s+type=hidden\s+value=""(.*?)""\s+>").Groups[1].Value;

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

                   result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie, chargeVbiNum);
                    #endregion

                    #region 充值结果判断
                    if (result.Contains("成功") || result.Contains("您已成功充值")|| result.Contains("成功充值"))
                    {
                        order.RechargeMsg += spoid+"充值成功||";
                        order.SuccessfulAmount += chargeVbiNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    {
                        order.RechargeMsg += spoid + "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                        order.SuccessfulAmount += chargeVbiNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("操作失败"))
                    {
                        order.RechargeMsg += spoid + "操作失败||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("充值过于频繁"))
                    {
                        order.RechargeMsg += "充值频繁||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("验证码输入不正确"))
                    {
                        order.RechargeMsg += spoid + "验证码输入不正确||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("账户余额不足"))
                    {
                        order.RechargeMsg += "账户余额不足||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else if (result.Contains("支付密码不正确"))
                    {
                        order.RechargeMsg += spoid + "支付密码不正确||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else if (result.Contains("用户不存在"))
                    {
                        order.RechargeMsg += spoid + "用户不存在||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else
                    {
                        order.RechargeMsg += spoid + "存疑||";
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
                else if (order.SuccessfulAmount <= 0)
                {
                    if (order.RechargeMsg.Contains("帐号不存在") || order.RechargeMsg.Contains("用户不存在"))
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


        /// <summary>
        /// 提交数量
        /// </summary>
        /// <param name="totalGameNum">剩余要充值的总游戏币数量</param>
        /// <param name="chargeVbiNum">当前要充值的V币数量</param>
        bool GetChargeNum(int totalGameNum, ref int chargeVbiNum)
        {
            bool status = true;

            if (totalGameNum>=100)
            {
                chargeVbiNum = 100;
            }
            else if (totalGameNum >= 50)
            {
                chargeVbiNum = 50;
            }
            else if (totalGameNum >= 30)
            {
                chargeVbiNum = 30;
            }
            else if (totalGameNum >= 20)
            {
                chargeVbiNum = 20;
            }
            else if (totalGameNum >= 15)
            {
                chargeVbiNum = 15;
            }
            else if (totalGameNum >= 10)
            {
                chargeVbiNum = 10;
            }
            else
            {
                status = false;
            }

            return status;
        }
    }
}
