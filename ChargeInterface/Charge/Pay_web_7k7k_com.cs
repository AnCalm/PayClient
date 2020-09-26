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
    public class Pay_web_7k7k_com
    {
        public Order Charge(Order order)
        {
            try
            {
                int totalPrice = (int)order.BuyAmount;
                int totalPriceFixed = totalPrice;
                int isContinue = 0;

                if (CheckStrHelper.IsChinese(order.TargetAccount))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "用户名不合法(4-32位数字字母和_组合)";
                    return order;
                }

                //用户名不合法(4-32位数字字母和_组合)
                if (order.TargetAccount.Length < 4 || order.TargetAccount.Length > 32)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "用户名不合法(4-32位数字字母和_组合)";
                    return order;
                }

                if(order .BuyAmount%10!=0)
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "充值数量不正确";
                    return order;
                }

                CookieContainer cookie = new CookieContainer();
                string checkAccount = PostAndGet.HttpGetString("http://pay.web.7k7k.com/checkuser/?username=" + order.TargetAccount , "",
                    ref cookie, "http://pay.web.7k7k.com/?qq-pf-to=pcqq.c2c");
                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                      + ",帐号检测提交返回：" + checkAccount, LogPathFile.Recharge);

                string status = Regex.Match(checkAccount, @"""status"":""(.*?)""").Groups[1].Value;
                if (status.Equals("-1"))
                {
                    order.RechargeStatus = (int)OrderRechargeStatus.failure;
                    order.RechargeMsg = "帐号错误";
                    return order;
                }

                string uid = Regex.Match(checkAccount, @"""uid"":(.*?)}").Groups[1].Value;

                while (totalPrice > 0)
                {
                    if (isContinue >= 3) break;//重试三次

                    //totalPrice--;
                    string result = "存疑";
                    string msg = "";
                    string CardNumber = string.Empty;
                    int payNum = 0;
                    if (!GetPayNum(totalPrice, ref payNum))
                    {
                        isContinue++;
                        continue;
                    }
                    else
                    {
                        totalPrice = totalPrice - payNum;
                    }

                    if (!string.IsNullOrEmpty(order.RechargeMsg) || !string.IsNullOrEmpty(CardNumber))
                    {
                        System.Threading.Thread.Sleep(10 * 1000);
                    }

                    bool bo = ReCharge(order, payNum, uid, cookie, ref CardNumber, ref result, ref msg);

                    order.RechargeMsg += msg;
                    if (!string.IsNullOrEmpty(CardNumber))
                        order.ChargeAccountInfo += CardNumber + "||";

                    if (result.Contains("成功"))
                    {
                        order.SuccessfulAmount += payNum;
                    }
                    else if (result.Contains("失败"))
                    {
                        if (bo)
                        {
                            //重复提交
                            cookie = new CookieContainer();
                            totalPrice += payNum;
                            isContinue++;
                        }
                        else break; //订单失败直接返回
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

        bool ReCharge(Order order, int payNum, string payuid, CookieContainer cookie, ref string CardNumber, ref string status, ref string msg)
        {
            #region 参数提交

            string ordercollectUrl = "http://pay.web.7k7k.com/ordercollect";
            StringBuilder ordercollectStr = new StringBuilder();
            ordercollectStr.AppendFormat("payuid={0}", payuid);
            ordercollectStr.AppendFormat("&paywhere={0}", "2");
            string result = PostAndGet.HttpPostString_HX(ordercollectUrl, ordercollectStr.ToString(), ref cookie, "pay.web.7k7k.com", "http://pay.web.7k7k.com/?qq-pf-to=pcqq.c2c");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

            string cid = Regex.Match(result, @"""cid"":""(.*?)"",").Groups[1].Value;

            Cards cards = SQLCards.GetChargeCards(OrderChargeAccountType.MMCard, payNum);
            if (cards == null)
            {
                status = "失败";
                msg = "取卡失败||";
                return true;
            }
            CardNumber = cards.CardNumber;


            string payorderUrl = "http://pay.web.7k7k.com/payorder";
            StringBuilder payorderStr = new StringBuilder();

            payorderStr.AppendFormat("paywhere={0}", "2");
            payorderStr.AppendFormat("&paychannel={0}", "30");
            payorderStr.AppendFormat("&confirmusername={0}", order.TargetAccount);
            payorderStr.AppendFormat("&payuid={0}", payuid);
            payorderStr.AppendFormat("&gametext={0}", "1.%E9%80%89%E6%8B%A9%E6%B8%B8%E6%88%8F");
            payorderStr.AppendFormat("&gid={0}", "");
            payorderStr.AppendFormat("&servertext={0}", "2.%E9%80%89%E6%8B%A9%E5%8C%BA%E6%9C%8D");
            payorderStr.AppendFormat("&server_id={0}", "");
            payorderStr.AppendFormat("&cardnumber={0}", cards.CardNumber);
            payorderStr.AppendFormat("&cardpass={0}", cards.CardPassWord);
            payorderStr.AppendFormat("&kbyue={0}", "");
            payorderStr.AppendFormat("&passstatus={0}", "0");
            payorderStr.AppendFormat("&select_paytype={0}", "1");
            payorderStr.AppendFormat("&select_bank={0}", "ICBC-NET-B2C");
            payorderStr.AppendFormat("&category={0}", "1");
            payorderStr.AppendFormat("&cid={0}", cid);
            payorderStr.AppendFormat("&amt={0}", payNum);
            payorderStr.AppendFormat("&user_sta={0}", "true");

            result = PostAndGet.HttpPostString(payorderUrl, payorderStr.ToString(), ref cookie);
            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
             + ",米米卡提交返回：" + result, LogPathFile.Recharge);


            if (result.Contains("充值失败"))
            {
                status = "失败";
                msg = "充值失败(充值失败，请确认您的卡号密码后重试)||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.untreated;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if (result.Contains("充值成功"))
            {
                status = "成功";
                msg = cid + "充值成功||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if (result.Contains("米米卡余额不足"))
            {
                status = "失败";
                msg = "余额不足||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.failure;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if (result.Contains("提交失败"))
            {
                status = "失败";
                if (result.Contains("订单号重复"))
                    msg = "订单号重复||";
                else
                    msg = "提交失败||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.untreated;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else
            {
                status = "可疑";
                msg = "充值存疑";

                cards.ReChargeStatus = (int)OrderRechargeStatus.suspicious;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return false ;
            }

            #endregion
        }

        bool  GetPayNum(int totalNum ,ref int payNum)
        {
            bool result=true;
            if (totalNum >= 100)
                payNum = 100;
            else if (totalNum >= 50)
                payNum = 50;
            else if (totalNum >= 30)
                payNum = 30;
            else if (totalNum >= 10)
                payNum = 10;
            else
                result = false;

            return result;
        }
    }
}
