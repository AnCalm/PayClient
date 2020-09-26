using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargeInterface.ChargeHelper.Charge.VbiHelper;
using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using System.Net;
using System.Text.RegularExpressions;
using ChargeInterface.ChargeHelper.OrderHelper;

namespace ChargeInterface.Charge
{
    public class Pay_91y_com_gamecard_New
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
            CookieContainer cookie = new CookieContainer();
            string checkAccount = PostAndGet.HttpGetString("https://pay.91y.com/servlet/do.ashx?a=changeuser&acc=" + order.TargetAccount + "&t=0.48772043911252916", "", ref cookie, "https://pay.91y.com/tel/");
            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                  + ",帐号检测提交返回：" + checkAccount, LogPathFile.Recharge);

            string Accounts = Regex.Match(checkAccount, @"Accounts: '(.*?)'").Groups[1].Value;
            string Nickname = Regex.Match(checkAccount, @"Nickname: '(.*?)'").Groups[1].Value;
            string GameID = Regex.Match(checkAccount, @"GameID: '(.*?)'").Groups[1].Value;
            if (Accounts.Equals("0"))
            {
                order.RechargeMsg = "帐号错误";
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                return;
            }

            int count = 0;

            while (order.RechargeStatus == (int)OrderRechargeStatus.processing)
            {
                if (count > 3)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    break;
                }

                string orderMsg = ReCharge(order, Accounts, cookie);
                string orderStatus = OrderStatusForXml.GetOrderStatus("91y", orderMsg);

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
        string ReCharge(Order order, string Accounts, CookieContainer cookie)
        {
            #region 参数提交

            string tradeverifyUrl = "https://pay.91y.com/tradeverify/";
            StringBuilder tradeverifyStr = new StringBuilder();
            tradeverifyStr.AppendFormat("user_qq={0}", "");
            tradeverifyStr.AppendFormat("&pay_User={0}", Accounts);
            tradeverifyStr.AppendFormat("&pd_FrpId={0}", "ZONGY-NET");
            tradeverifyStr.AppendFormat("&pay_amount={0}", (int)order.ProductParValue);
            tradeverifyStr.AppendFormat("&pay_type={0}", "5");
            tradeverifyStr.AppendFormat("&pay_subtype={0}", "0");
            tradeverifyStr.AppendFormat("&step={0}", "2");
            string result = PostAndGet.HttpPostString_HX(tradeverifyUrl, tradeverifyStr.ToString(), ref cookie, "pay.91y.com", "https://pay.91y.com/gamecard/");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

            if (result.Contains("无法继续充值"))
            {
                string msg = "充值已达每日上限";
                order.RechargeMsg = msg;
                return msg;
            }

            if (result.Contains("用户不存在"))
            {
                string msg = "用户不存在";
                order.RechargeMsg = msg;
                return msg;
            }

            if (result.Contains("所选金额会超出每日限额"))
            {
                string msg = "所选金额会超出每日限额";
                order.RechargeMsg = msg;
                return msg;
            }

            if (result.Contains("订单提交失败"))
            {
                string msg = "订单提交失败";
                order.RechargeMsg = msg;
                return msg;
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
            result = PostAndGet.HttpPostString(servletUrl, servletStr.ToString(), ref cookie);

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",订单第二步提交返回：" + result, LogPathFile.Recharge);

            if (!result.Contains("确认提交"))
            {
                string msg = "确认提交订单失败";
                order.RechargeMsg = msg;
                return msg;
            }


            #endregion

            #region 纵游卡提交

            Cards cards = SQLCards.GetChargeCards(OrderChargeAccountType.ZYCard, (decimal)order.ProductParValue);

            if (cards == null)
            {
                string msg = "取卡失败";
                order.RechargeMsg = msg;
                return msg;
            }

            string a = "yeepay";
            string orderId = Regex.Match(result, @"id=""pay_OrderId""\s+name=""pay_OrderId""\s+value=""(.*?)""").Groups[1].Value;
            string paymoney = Regex.Match(result, @"id=""pay_amount""\s+name=""pay_amount""\s+value=""(.*?)""").Groups[1].Value;
            string cardType = "ZONGY-NET";
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
            result = PostAndGet.HttpGetString(yeepayUrl, yeepayStr.ToString(), ref cookie, "https://pay.91y.com/yeepay/Index.shtml");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",纵游卡提交返回：" + result, LogPathFile.Recharge);

            if (result.Contains("输入字符串的格式不正确"))
            {
                string msg = "输入字符串的格式不正确";
                order.RechargeMsg = msg;
                return msg;
            }

            #endregion

            #region 查询卡密结果

            int queryCount = 0;
        ReQuery:
            if (queryCount > 10)
            {
                string msg = "查询卡密充值结果超时";
                order.RechargeMsg = orderId + msg;

                SQLCards.UpdateCards_ByMultiple(cards, (int)OrderRechargeStatus.successful, orderId + msg);

                return msg;
            }
            queryCount++;
            StringBuilder yeepaycheck = new StringBuilder();
            yeepaycheck.AppendFormat("a={0}", "yeepaycheck");
            yeepaycheck.AppendFormat("&orderId={0}", orderId);
            yeepaycheck.AppendFormat("&userID={0}", payUserID);
            yeepaycheck.AppendFormat("&t={0}", t);
            result = PostAndGet.HttpGetString(yeepayUrl, yeepaycheck.ToString(), ref cookie);

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
         + ",纵游卡提交结果查询返回：" + result, LogPathFile.Recharge);

            if (result.Contains("充值失败"))
            {
                string msg = "充值失败(充值失败，请确认您的卡号密码后重试)";
                order.RechargeMsg = orderId + msg; ;

                SQLCards.UpdateCards_ByMultiple(cards, (int)OrderRechargeStatus.successful, orderId + msg);

                return msg;
            }
            else if (result.Contains("充值成功"))
            {
                string msg = "充值成功";
                order.RechargeMsg = orderId + msg;

                SQLCards.UpdateCards_ByMultiple(cards, (int)OrderRechargeStatus.successful, orderId + msg);
                return msg;
            }
            else if (result.Contains("提交失败"))
            {
                string msg = "";

                if (result.Contains("订单号重复"))
                    msg = "订单号重复||";
                else
                    msg = "订单提交失败||";

                order.RechargeMsg = msg;

                SQLCards.UpdateCards_ByMultiple(cards, (int)OrderRechargeStatus.untreated, msg);

                return msg;
            }
            else
            {
                System.Threading.Thread.Sleep(1000);
                goto ReQuery;
            }
            #endregion 查询卡密结果
        } 
    }
}

