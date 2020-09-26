using ChargeInterface.AntoInterface;
using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChargeInterface.Charge
{
    public class pay_hanyou_com : ICharge
    {
        public Order Charge(Order order)
        {
            try
            {
                int totalPrice = (int)order.BuyAmount;
                int totalPriceFixed = totalPrice;
                int isContinue = 0;

                CookieContainer cookie = new CookieContainer();

                while (totalPrice > 0)
                {
                    if (isContinue >= 3) break;//重试三次

                    totalPrice--;
                    string result = "存疑";
                    string msg = "";
                    string CardNumber = string.Empty;

                    bool bo = ReCharge(order, (int)order.ProductParValue, cookie, ref CardNumber, ref result, ref msg);

                    order.RechargeMsg += msg;
                    if (!string.IsNullOrEmpty(CardNumber))
                        order.ChargeAccountInfo += CardNumber + "||";

                    if (result.Contains("成功"))
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

        bool ReCharge(Order order, int payNum, CookieContainer cookie, ref string CardNumber, ref string status, ref string msg)
        {

            #region 参数提交

            //nickname=可是地方和高科技&channelid=6&channelname=纵游一卡通&money=50&yeepay_type=ZY&v=1496835901593
            StringBuilder checkStr = new StringBuilder();
            checkStr.AppendFormat("nickname={0}", order.TargetAccount);
            checkStr.AppendFormat("&channelid={0}", "6");
            checkStr.AppendFormat("&channelname={0}", "纵游一卡通");
            checkStr.AppendFormat("&money={0}", (int)payNum);
            checkStr.AppendFormat("&yeepay_type={0}", "ZY");
            checkStr.AppendFormat("&v={0}", "1496835901593");
            string result = PostAndGet.HttpPostString_HY("http://pay.hanyou.com/pay.do", checkStr.ToString(), ref cookie, "pay.hanyou.com", "http://pay.hanyou.com/");
            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                  + ",帐号检测提交返回：" + result, LogPathFile.Recharge);

            //{"msg":"该昵称不存在","code":1,"nickName":"可是地方和高科技"}

            if (result.Contains("该昵称不存在"))
            {
                status = "失败";
                msg = "用户不存在||";
                return false;
            }

            if (!result.Contains("确认充值"))
            {
                status = "失败";
                msg = "确认提交订单失败||";
                return true;
            }


            string channelid = Regex.Match(result, @"name=""channelid"" value=""(.*?)"" />").Groups[1].Value;
            string userid = Regex.Match(result, @"name=""userid"" value=""(.*?)"" />").Groups[1].Value;
            string nickname = Regex.Match(result, @"name=""nickname"" value=""(.*?)"" />").Groups[1].Value;
            string unionid = Regex.Match(result, @"name=""unionid"" value=""(.*?)"" />").Groups[1].Value;
            string money = Regex.Match(result, @"name=""money"" value=""(.*?)"" />").Groups[1].Value;
            string yeepay_type = Regex.Match(result, @"name=""yeepay_type"" value=""(.*?)"" />").Groups[1].Value;

            StringBuilder placeorderStr = new StringBuilder();
            placeorderStr.AppendFormat("channelid={0}", channelid);
            placeorderStr.AppendFormat("&userid={0}", userid);
            placeorderStr.AppendFormat("&nickname={0}", nickname);
            placeorderStr.AppendFormat("&unionid={0}", unionid);
            placeorderStr.AppendFormat("&money={0}", money);
            placeorderStr.AppendFormat("&yeepay_type={0}", yeepay_type);

            result = PostAndGet.HttpPostString_HY("http://pay.hanyou.com/order/placeorder.do", placeorderStr.ToString(), ref cookie, "pay.hanyou.com", "http://pay.hanyou.com/");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",订单第一步提交返回：" + result, LogPathFile.Recharge);


            #endregion

            #region 纵游卡提交

            Cards cards = SQLCards.GetChargeCards(OrderChargeAccountType.ZYCard, (decimal)order.ProductParValue);

            if (cards == null)
            {
                status = "失败";
                msg = "取卡失败||";
                return true;
            }
            CardNumber = cards.CardNumber;


            string bizType = Regex.Match(result, @"name=\\""bizType\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string merchantNo = Regex.Match(result, @"name=\\""merchantNo\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string merchantOrderNo = Regex.Match(result, @"name=\\""merchantOrderNo\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string requestAmount = Regex.Match(result, @"name=\\""requestAmount\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string url = Regex.Match(result, @"name=\\""url\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string cardCode = Regex.Match(result, @"name=\\""cardCode\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string productName = Regex.Match(result, @"name=\\""productName\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;
            string hmac = Regex.Match(result, @"name=\\""hmac\\"" value=\\""(.*?)\\""\/>").Groups[1].Value;

            string yeeykUrl = "http://www.yeeyk.com/yeex-xcard-app/createOrder";
            StringBuilder yeeykStr = new StringBuilder();
            yeeykStr.AppendFormat("bizType={0}", bizType);
            yeeykStr.AppendFormat("&merchantNo={0}", merchantNo);
            yeeykStr.AppendFormat("&merchantOrderNo={0}", merchantOrderNo);
            yeeykStr.AppendFormat("&requestAmount={0}", requestAmount);
            yeeykStr.AppendFormat("&url={0}", System.Web.HttpUtility.UrlEncode(url, Encoding.Default));
            yeeykStr.AppendFormat("&cardCode={0}", cardCode);
            yeeykStr.AppendFormat("&productName={0}", System.Web.HttpUtility.UrlEncode(productName, Encoding.UTF8));
            yeeykStr.AppendFormat("&hmac={0}", hmac);
            result = PostAndGet.HttpPostString(yeeykUrl, yeeykStr.ToString(), ref cookie, "http://pay.hanyou.com/");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",纵游卡收卡台提交返回：" + result, LogPathFile.Recharge);



            result = PostAndGet.HttpPostString("http://www.yeeyk.com/yeex-xcard-app/card/cardRule?cardType=ZY", "", ref cookie, "http://www.yeeyk.com/yeex-xcard-app/createOrder");

            string acquiringURL = "http://www.yeeyk.com/yeex-xcard-app/acquiring";
            //?amount=1&cardNo=703015036002529&cardType=ZY&customerNumber=10011829538&password=339745869503044&payAmount=50.00&requestId=yeeyk_201706071950562364486631
            StringBuilder acquiringStr = new StringBuilder();
            acquiringStr.AppendFormat("amount={0}", "1");
            acquiringStr.AppendFormat("&cardNo={0}", cards.CardNumber);
            acquiringStr.AppendFormat("&cardType={0}", "ZY");
            acquiringStr.AppendFormat("&customerNumber={0}", merchantNo);
            acquiringStr.AppendFormat("&password={0}", cards.CardPassWord);
            acquiringStr.AppendFormat("&payAmount={0}", payNum.ToString() + ".00");
            acquiringStr.AppendFormat("&requestId={0}", merchantOrderNo);

            result = PostAndGet.HttpPostString(acquiringURL+"?"+ acquiringStr.ToString(),"", ref cookie, "http://www.yeeyk.com/yeex-xcard-app/createOrder");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
              + ",纵游卡提交返回：" + result, LogPathFile.Recharge);

            if (!result.Contains("收单成功"))
            {
                status = "失败";
                msg = "纵游卡提交失败||";
                return true;
            }

            #endregion


            System.Threading.Thread.Sleep(5 * 1000);

            string orderno = merchantOrderNo.Substring(merchantOrderNo.IndexOf('_') + 1);
            #region 查询卡密结果

            string queryResultURL = "http://www.yeeyk.com/yeex-xcard-app/queryResult?" + "customerNumber=" + merchantNo + "&requestId=" + merchantOrderNo;
            //?customerNumber=10011829538&requestId=yeeyk_201706071950562364486631

            //http://www.yeeyk.com/yeex-xcard-app/queryResult?customerNumber=10011829538&requestId=yeeyk_201706122324593034531264

            result = PostAndGet.HttpPostString(queryResultURL, "", ref cookie, "http://www.yeeyk.com/yeex-xcard-app/createOrder");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
         + ",纵游卡提交结果查询返回：" + result, LogPathFile.Recharge);

            string code = Regex.Match(result, @"""code"" : ""(.*?)""").Groups[1].Value;


            string confirmResultURL = "http://www.yeeyk.com/yeex-xcard-app/confirmResult?orderKey=" + code;
            result = PostAndGet.HttpGetString_9Y(confirmResultURL, "", ref cookie, "www.yeeyk.com","http://www.yeeyk.com/yeex-xcard-app/createOrder");

            WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
             + ",纵游卡提交结果查询最终返回：" + result, LogPathFile.Recharge);


            if (result.Contains("充值成功") || result.Contains("订单支付成功"))
            {
                status = "成功";
                msg = orderno + "充值成功||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else if (result.Contains("订单支付失败"))
            {
                status = "失败";
                msg = orderno + "订单支付失败||";
                cards.ReChargeStatus = (int)OrderRechargeStatus.untreated;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }
            else
            {
                status = "成功";
                msg = orderno + "订单提交成功查询结果失败||";

                cards.ReChargeStatus = (int)OrderRechargeStatus.successful;
                cards.ReChargeMsg += msg;
                SQLCards.UpdateCards_ByMultiple(cards);
                return true;
            }

            #endregion
        }
    }
}
