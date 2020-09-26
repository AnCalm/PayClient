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
    public class Pay_91y_com_gamecard
    {
        public Order Charge(Order order)
        {
            try
            {
                int totalPrice = (int)order.BuyAmount;
                int totalPriceFixed = totalPrice;
                int isContinue = 0;

                CookieContainer cookie = new CookieContainer();
                string checkAccount = PostAndGet.HttpGetString_91y("https://pay.91y.com/servlet/do.ashx?a=changeuser&acc=" + order.TargetAccount + "&t=0.48772043911252916", "", ref cookie, "pay.91y.com", "https://pay.91y.com/gamecard/");
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
                    if (isContinue >= 3) break;//重试三次
                    
                    totalPrice--;
                    string result = "存疑";
                    string msg = "";
                    string CardNumber = string.Empty;

                    bool bo = ReCharge(order, (int)order.ProductParValue, Accounts, cookie,ref CardNumber, ref result, ref msg);

                    order.RechargeMsg += msg;
                    if (!string.IsNullOrEmpty(CardNumber))
                    order.ChargeAccountInfo += CardNumber + "||";

                    if(result.Contains("成功"))
                    {
                        order.SuccessfulAmount++;
                    }
                    else if (result.Contains("失败"))
                    {
                        if (bo)
                        {
                            //重复提交
                            cookie = new CookieContainer();
                            totalPrice++;                                                                                     
                            isContinue++;
                        }
                        else break; // 订单失败直接返回
                    }
                    else
                    {
                        break;
                    }
                }

                #region 订单状态判断
                if (order.SuccessfulAmount >= totalPriceFixed)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                }
                else if (order.SuccessfulAmount == 0
                    && (order.RechargeMsg.Contains("充值已达每日上限") || order.RechargeMsg.Contains("所选金额会超出每日限额") ||
                    order.RechargeMsg.Contains("用户不存在")))
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

        bool ReCharge(Order order, int payNum, string Accounts, CookieContainer cookie, ref string CardNumber,ref string status, ref string msg)
        {
            #region 参数提交

            string tradeverifyUrl = "https://pay.91y.com/tradeverify/";
            StringBuilder tradeverifyStr = new StringBuilder();
            //tradeverifyStr.AppendFormat("user_qq={0}", "");
            tradeverifyStr.AppendFormat("pay_User={0}", Accounts);
            tradeverifyStr.AppendFormat("&pd_FrpId={0}", "TIANHONG-NET");
            tradeverifyStr.AppendFormat("&pay_amount={0}", (int)order.ProductParValue);
            tradeverifyStr.AppendFormat("&pay_type={0}", "5");
            tradeverifyStr.AppendFormat("&pay_subtype={0}", "0");
            tradeverifyStr.AppendFormat("&step={0}", "2");
            string result = PostAndGet.HttpPostString_91y(tradeverifyUrl, tradeverifyStr.ToString(), ref cookie, "pay.91y.com", "https://pay.91y.com/gamecard/");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

            if (result.Contains("无法继续充值"))
            {
                status="失败";
                msg = "充值已达每日上限||";
                return false ;
            }

            if (result.Contains("用户不存在"))
            {
                status = "失败";
                msg = "用户不存在||";
                return false;
            }

            if (result.Contains("所选金额会超出每日限额"))
            {
                status = "失败";
                msg = "所选金额会超出每日限额||";
                return false;
            }

            if (result.Contains("订单提交失败"))
            {
                status = "失败";
                msg = "订单提交失败||";
                return true;
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

            string servletUrl = "https://pay.91y.com/yeepay/Index.shtml";

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

            if (!result.Contains("确认提交"))
            {
                status = "失败";
                msg ="确认提交订单失败||";
                return true;
            }

            #endregion

            #region 纵游卡提交

            Cards cards = SQLCards.GetChargeCards(OrderChargeAccountType.THCard, (decimal)order.ProductParValue);

            if (cards == null)
            {
                status = "失败";
                msg = "取卡失败||";
                return true;
            }
            CardNumber = cards.CardNumber;

            string a = "yeepay";
            string orderId = Regex.Match(result, @"id=""pay_OrderId""\s+name=""pay_OrderId""\s+value=""(.*?)""").Groups[1].Value;
            string paymoney = Regex.Match(result, @"id=""pay_amount""\s+name=""pay_amount""\s+value=""(.*?)""").Groups[1].Value;
            string cardType = "TIANHONG-NET";
            string payUser = Regex.Match(result, @"id=""pay_User""\s+name=""pay_ItemUser""\s+value=""(.*?)""").Groups[1].Value;
            string payUserID = Regex.Match(result, @"id=""pay_UserID""\s+name=""pay_UserID""\s+value=""(.*?)""").Groups[1].Value;
            string cardId = cards.CardNumber;
            string cardPsw = cards.CardPassWord;
            string t = "0.08686665393995474";

            string yeepayUrl = "https://pay.91y.com/servlet/do.ashx";
            StringBuilder yeepayStr = new StringBuilder();
            yeepayStr.AppendFormat("a={0}", a);
            yeepayStr.AppendFormat("&orderId={0}", orderId);
            yeepayStr.AppendFormat("&paymoney={0}", paymoney);
            yeepayStr.AppendFormat("&cardType={0}", cardType);
            yeepayStr.AppendFormat("&payUser={0}", payUser);
            yeepayStr.AppendFormat("&payUserID={0}", payUserID);
            yeepayStr.AppendFormat("&cardId={0}", cardId);
            yeepayStr.AppendFormat("&cardPsw={0}", cardPsw);
            yeepayStr.AppendFormat("&t={0}", t);
            result = PostAndGet.HttpGetString_91y(yeepayUrl, yeepayStr.ToString(), ref cookie,null, "https://pay.91y.com/yeepay/Index.shtml");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",纵游卡提交返回：" + result, LogPathFile.Recharge);

            if(result.Contains("输入字符串的格式不正确"))
            {
                status = "失败";
                msg = "输入字符串的格式不正确||";
                return true;
            }
            //if (result != "YES")
            //{
            //    status = "失败";
            //    msg ="卡密提交失败||";
            //    return true;
            //}
            
            #endregion

            #region 查询卡密结果

            int queryCount = 0;
        ReQuery:
            if (queryCount>10)
            {
                status = "成功";
                msg = orderId + "查询卡密充值结果超时||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            queryCount++;
            StringBuilder yeepaycheck = new StringBuilder();
            yeepaycheck.AppendFormat("a={0}", "yeepaycheck");
            yeepaycheck.AppendFormat("&orderId={0}", orderId);
            yeepaycheck.AppendFormat("&userID={0}", payUserID);
            yeepaycheck.AppendFormat("&t={0}", t);
            result = PostAndGet.HttpGetString_91y(yeepayUrl, yeepaycheck.ToString(), ref cookie);

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
         + ",纵游卡提交结果查询返回：" + result, LogPathFile.Recharge);

            if (result.Contains("充值失败"))
            {
                status = "成功";
                msg = orderId + "充值失败(充值失败，请确认您的卡号密码后重试)||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if(result.Contains("充值成功"))
            {
                status = "成功";
                msg = orderId+"充值成功||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if (result.Contains("提交失败"))
            {
                status = "失败";
                if (result.Contains("订单号重复"))
                    msg = orderId + "订单号重复||";
                else
                    msg = orderId + "提交失败||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.untreated;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else
            {
                System.Threading.Thread.Sleep(1000);
                goto ReQuery;
            }

            #endregion
        }

    }
}
