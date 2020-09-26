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
    public class www_vpay8_com_WY : ICharge
    {
        public Order Charge(Order order)
        {
            try
            {
                CookieContainer cookie = new CookieContainer();

                decimal totalAmount = (decimal)order.BuyAmount;
                decimal totalAmountNum = totalAmount;
                int isContinue = 0;

                if (CheckStrHelper.IsChinese(order.TargetAccount))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "用户名不合法";
                    return order;
                }

                order.TargetAccount = CheckChargeAccount(order.TargetAccount);

                string radPayType = "1";
                if (order.RechargeModeName.Contains("帐号直充") || order.RechargeModeName.Contains("通用点")
                    || order.RechargeModeName.Contains("游戏点数") || order.RechargeModeName.Contains("帐号充值"))
                    radPayType = "1";
                else if (order.RechargeModeName.Contains("寄售点") || order.RechargeModeName.Contains("点数寄售")
                    || order.RechargeModeName.Contains("点卡交易/寄售"))
                    radPayType = "2";

                while (totalAmount > 0)
                {
                    #region 提交充值
                    int chargeNum = 0;
                    int vibiNum = 0;

                    if (isContinue > 3) //充值频繁，重试三次
                        break;

                    if (!GetChargeNum((int)totalAmount, ref chargeNum))
                        break;

                    totalAmount = totalAmount - chargeNum;
                    vibiNum = chargeNum * 2;

                    string result = "";


                    #region 获取参数 帐号判断

                    result = PostAndGet.HttpGetString("http://www.vpay8.com/Fetch/wy/wpay.aspx", "", ref cookie);

                    string __VIEWSTATE = Regex.Match(result, @"id=""__VIEWSTATE""\s+value=""(.*?)"" />").Groups[1].Value;
                    string __EVENTVALIDATION = Regex.Match(result, @"id=""__EVENTVALIDATION""\s+value=""(.*?)"" />").Groups[1].Value;


                    StringBuilder checkgetDataBuilder = new StringBuilder();
                   
                    checkgetDataBuilder.AppendFormat("__VIEWSTATE={0}",System.Web.HttpUtility.UrlEncode( __VIEWSTATE));
                    checkgetDataBuilder.AppendFormat("&__EVENTVALIDATION={0}",System.Web.HttpUtility.UrlEncode( __EVENTVALIDATION));
                    checkgetDataBuilder.AppendFormat("&DropDownList1={0}", chargeNum*10);
                    checkgetDataBuilder.AppendFormat("&radPayType={0}", radPayType);
                    checkgetDataBuilder.AppendFormat("&qqnum={0}", order.TargetAccount);
                    checkgetDataBuilder.AppendFormat("&qqnum2={0}", order.TargetAccount);
                    checkgetDataBuilder.AppendFormat("&Button2={0}", "%E6%8F%90%E4%BA%A4%E8%AE%A2%E5%8D%95");

                    result = PostAndGet.HttpPostString("http://www.vpay8.com/Fetch/wy/wpay.aspx", checkgetDataBuilder.ToString(), ref cookie, "http://www.vpay8.com/Fetch/wy/wpay.aspx");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "提交参数：" + checkgetDataBuilder.ToString()
                      + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

                    if (result.Contains("请输入正确的网易通行证账号"))
                    {
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        order.RechargeMsg = "帐号不存在";
                        return order;
                    }

                    //string href = Regex.Match(result, @"<a href=""(.*?)"">").Groups[1].Value;

                    ////http://www.vpay8.com/Fetch/wy/WSubmit.aspx?orderid=WY170313223559213118&pv=1&v=30&qq=357440019@qq.com&s=1788a04956cd27cc45de0d83f9ef010c

                    //result = PostAndGet.HttpGetString(System.Web.HttpUtility.UrlDecode(href, Encoding.Default), checkgetDataBuilder.ToString(), ref cookie);

                    //WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    // + ",订单第二步提交返回：" + result, LogPathFile.Recharge);

                    #endregion

                    #region Vbi钱包充值

                    string spid = Regex.Match(result, @"<input name='spid'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string spname = Regex.Match(result, @"<input name='spname'\s+type=""hidden""\s+value='(.*?)'> ").Groups[1].Value;
                    string spoid = Regex.Match(result, @"<input name='spoid'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string spreq = Regex.Match(result, @"<input name='spreq'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string sprec = Regex.Match(result, @"<input name='sprec'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string userid = Regex.Match(result, @"<input name='userid'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string userip = Regex.Match(result, @"<input name='userip'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string spmd5 = Regex.Match(result, @"<input name='spmd5'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string spcustom = Regex.Match(result, @"<input name='spcustom'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string spversion = Regex.Match(result, @"<input name='spversion'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string urlcode = Regex.Match(result, @"<input name='urlcode'\s+type=hidden\s+value='(.*?)' >").Groups[1].Value;
                    string spzf = Regex.Match(result, @"<input name='spzf'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    string money = Regex.Match(result, @"<input name='money'\s+type=""hidden""\s+value='(.*?)' >").Groups[1].Value;
                    

                    StringBuilder vnetonePostdataBuilder = new StringBuilder();
                    vnetonePostdataBuilder.AppendFormat("spid={0}", spid);
                    vnetonePostdataBuilder.AppendFormat("&spname={0}", System.Web.HttpUtility.UrlEncode(spname, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&spoid={0}", spoid);
                    vnetonePostdataBuilder.AppendFormat("&spreq={0}", System.Web.HttpUtility.UrlEncode(spreq, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&sprec={0}", System.Web.HttpUtility.UrlEncode(sprec, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&userid={0}", System.Web.HttpUtility.UrlEncode(userid, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&userip={0}", userip);
                    vnetonePostdataBuilder.AppendFormat("&spmd5={0}", spmd5);
                    vnetonePostdataBuilder.AppendFormat("&spcustom={0}", System.Web.HttpUtility.UrlEncode(spcustom, Encoding.UTF8));
                    vnetonePostdataBuilder.AppendFormat("&spversion={0}", spversion);
                    vnetonePostdataBuilder.AppendFormat("&urlcode={0}", urlcode);
                    vnetonePostdataBuilder.AppendFormat("&spzf={0}", spzf);
                    vnetonePostdataBuilder.AppendFormat("&money={0}", money);

                    OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.Vbi);

                    if (orderChargeAccount == null)
                    {
                        order.RechargeMsg += "未取到v币帐号||";
                        isContinue++;
                        continue;
                    }

                    result = VbiChargeHelper.VbiCharge(vnetonePostdataBuilder.ToString(), order, orderChargeAccount, cookie, vibiNum);
                    #endregion

                    #region 充值结果判断
                    if (result.Contains("成功") || result.Contains("您已成功充值") || result.Contains("成功充值"))
                    {
                        order.RechargeMsg += spoid + "充值成功||";
                        order.SuccessfulAmount += chargeNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    {
                        order.RechargeMsg += spoid + "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                        order.SuccessfulAmount += chargeNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("操作失败"))
                    {
                        order.RechargeMsg += spoid + "操作失败||";
                        totalAmount += chargeNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("充值过于频繁"))
                    {
                        order.RechargeMsg += "充值频繁||";
                        totalAmount += chargeNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("验证码输入不正确"))
                    {
                        order.RechargeMsg += spoid + "验证码输入不正确||";
                        totalAmount += chargeNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("账户余额不足"))
                    {
                        order.RechargeMsg += "账户余额不足||";
                        totalAmount += chargeNum; ;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else if (result.Contains("支付密码不正确"))
                    {
                        order.RechargeMsg += spoid + "支付密码不正确||";
                        totalAmount += chargeNum; ;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else if (result.Contains("用户不存在"))
                    {
                        order.RechargeMsg += spoid + "用户不存在||";
                        totalAmount += chargeNum; ;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                        isContinue++;
                    }
                    else if (result.Contains("操作_Failure"))
                    {
                        order.RechargeMsg += spoid + "帐号错误||";
                        totalAmount += chargeNum; ;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else
                    {
                        order.RechargeMsg += spoid + "存疑||";
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    #endregion

                    #endregion
                }

                #region 订单状态判断
                if (order.SuccessfulAmount >= totalAmountNum)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                else if (order.SuccessfulAmount <= 0)
                {
                    if (order.RechargeMsg.Contains("帐号错误") || order.RechargeMsg.Contains("用户不存在"))
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    else
                        order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
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

        bool GetChargeNum(int totalNum, ref int chargeNum)
        {
            bool status = true;

            if (totalNum>15 && totalNum < 25)
            {
                chargeNum = 10;
            }
            else
            {
                if (totalNum >= 15)
                {
                    chargeNum = 15;
                }
                else if (totalNum >= 10)
                {
                    chargeNum = 10;
                }
                else
                {
                    status = false;
                }
            }
            return status;
        }


        string CheckChargeAccount(string chargeAccount)
        {
            if (chargeAccount.Contains("@"))
                return chargeAccount;
            else
            {
                chargeAccount += "@163.com";
                return chargeAccount;
            }
        }
    }
}
