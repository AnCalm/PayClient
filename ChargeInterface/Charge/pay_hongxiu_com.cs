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

namespace ChargeInterface.Charge
{
   public  class pay_hongxiu_com
    {
        public Order Charge(Order order)
        {
            try
            {
                decimal totalPrice = (decimal)(order.BuyAmount * 2);
                decimal totalPriceFixed = totalPrice;
                int isContinue = 0;

                while (totalPrice > 0)
                {
                    #region 提交充值
                    int chargeVbiNum = 0;

                    if (isContinue > 3) //充值频繁，重试三次
                        break;

                    if (!GetChargeNum((int)totalPrice, ref chargeVbiNum))
                        break;
                    totalPrice = totalPrice - chargeVbiNum;


                    string result = "";
                    CookieContainer cookie = new CookieContainer();

                    #region 帐号登录
                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/model/GetUserBaseInfo.aspx?roundmun=0.8916430823085395", "", ref cookie);
                    if (string.IsNullOrEmpty(result))
                    {
                        if (!login(order, ref cookie))
                        {
                            totalPrice += chargeVbiNum;
                            order.RechargeMsg = "帐号登录失败";
                            continue;
                        }
                    }
                    #endregion

                    #region 获取参数 订单确认
                    

                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/model/GetUserBaseInfo.aspx?roundmun=0.8916430823085395", "", ref cookie);
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                       + ",获取登录信息：" + result, LogPathFile.Recharge);

                    string [] arrLoginAccount=result.Split('|');
                    if(arrLoginAccount.Length<2)
                    {
                        break;
                    }
                    string txtUserid = arrLoginAccount[0];
                    string txtusername = arrLoginAccount[1];

                    result = PostAndGet.HttpGetString_HX("http://hxzz.hxcdn.net/?d=pay.hongxiu.com&hxid=" + txtUserid + "&f=/default.shtml&q=&aid=0&bid=0", "", ref cookie, "hxzz.hxcdn.net", "http://pay.hongxiu.com/default.shtml");

                    result = PostAndGet.HttpGetString_HX("http://pay.hongxiu.com/charge/guhua.shtml", "", ref cookie, "pay.hongxiu.com");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                       + ",充值页面提交返回：" + result, LogPathFile.Recharge);

                    StringBuilder payTypeDataBuilder = new StringBuilder();
                    payTypeDataBuilder.AppendFormat("usertype={0}", "1");
                    payTypeDataBuilder.AppendFormat("&txtotherusername={0}",System.Web.HttpUtility.UrlEncode( order.TargetAccount,Encoding.UTF8));
                    payTypeDataBuilder.AppendFormat("&txtreotherusername={0}", System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8));
                    payTypeDataBuilder.AppendFormat("&txtuserid={0}", txtUserid);
                    payTypeDataBuilder.AppendFormat("&txtusername={0}", txtusername);
                    result = PostAndGet.HttpPostString_HX("http://pay.hongxiu.com/charge/cp.aspx?paytype=votev2", payTypeDataBuilder.ToString(), ref cookie, "pay.hongxiu.com");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                        + ",订单第一步提交返回：" + result, LogPathFile.Recharge);

                    if (result.Contains("此用户不存在"))
                    {
                        order.RechargeStatus = (int)OrderRechargeStatus.failure;
                        order.RechargeMsg = "此用户不存在";
                        return order;
                    }

                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/charge/TelChargeV2.aspx", "", ref cookie, "http://pay.hongxiu.com/charge/cp.aspx?paytype=votev2");
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                         + ",订单第二步提交返回：" + result, LogPathFile.Recharge);

                    string __VIEWSTATE = Regex.Match(result, @"id=""__VIEWSTATE"" value=""(.*?)""\s+/>").Groups [1].Value ;
                    string __EVENTVALIDATION = Regex.Match(result, @"id=""__EVENTVALIDATION"" value=""(.*?)""\s+/>").Groups[1].Value;

                    StringBuilder chargeV2DataBuilder = new StringBuilder();
                    payTypeDataBuilder.AppendFormat("__VIEWSTATE={0}", __VIEWSTATE);
                    payTypeDataBuilder.AppendFormat("&__EVENTVALIDATION={0}", __EVENTVALIDATION);
                    payTypeDataBuilder.AppendFormat("&userid={0}", order.TargetAccount);
                    payTypeDataBuilder.AppendFormat("&btnSubmit={0}", "=%E6%8F%90++%E4%BA%A4");
                    payTypeDataBuilder.AppendFormat("&txtusername={0}", chargeVbiNum);
                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/charge/TelChargeV2.aspx", chargeV2DataBuilder.ToString(), ref cookie);
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                        + ",订单第二步确认提交返回：" + result, LogPathFile.Recharge);

                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/charge/ChargeStep3.aspx", "", ref cookie);
                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                        + ",订单第三步提交返回：" + result, LogPathFile.Recharge);
                    //if (!result.Contains("充值确认"))
                    //{
                    //    totalPrice += chargeVbiNum;
                    //    continue;
                    //}

                    #endregion

                    #region Vbi钱包充值

                    string spid = Regex.Match(result, @"name='spid'\s+value='(.*?)'>").Groups[1].Value;
                    string spname = Regex.Match(result, @"name='spname'\s+value='(.*?)'>").Groups[1].Value;
                    string spoid = Regex.Match(result, @"name='spoid'\s+value='(.*?)'>").Groups[1].Value;
                    string spreq = Regex.Match(result, @"name='spreq'\s+value='(.*?)'>").Groups[1].Value;
                    string sprec = Regex.Match(result, @"name='sprec'\s+value='(.*?)'>").Groups[1].Value;
                    string userid = Regex.Match(result, @"name='userid'\s+value='(.*?)'>").Groups[1].Value;
                    string userip = Regex.Match(result, @"name='userip'\s+value='(.*?)'>").Groups[1].Value;
                    string spmd5 = Regex.Match(result, @"name='spmd5'\s+value='(.*?)'>").Groups[1].Value;
                    string spcustom = Regex.Match(result, @"name='spcustom'\s+value='(.*?)'>").Groups[1].Value;
                    string spversion = Regex.Match(result, @"name='spversion'\s+value='(.*?)'>").Groups[1].Value;
                    string money = Regex.Match(result, @"name='money'\s+value='(.*?)'>").Groups[1].Value;
                    string urlcode = Regex.Match(result, @"name='urlcode'\s+value='(.*?)'>").Groups[1].Value;

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
                        order.SuccessfulAmount += chargeVbiNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("请五分钟后再登陆商户网站进行帐户查询"))
                    {
                        order.RechargeMsg += "充值成功(请五分钟后再登陆商户网站进行帐户查询)||";
                        order.SuccessfulAmount += chargeVbiNum;
                        orderChargeAccount.Balance = orderChargeAccount.Balance - chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                    }
                    else if (result.Contains("操作失败"))
                    {
                        order.RechargeMsg += "操作失败||";
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
                        order.RechargeMsg += "验证码输入不正确||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false);
                        isContinue++;
                    }
                    else if (result.Contains("账户余额不足"))
                    {
                        order.RechargeMsg += "账户余额不足||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else if (result.Contains("支付密码不正确"))
                    {
                        order.RechargeMsg += "支付密码不正确||";
                        totalPrice += chargeVbiNum;
                        SQLOrderChargeAccount.UpdateChargeAccount(orderChargeAccount, false, false);
                    }
                    else if (result.Contains("用户不存在"))
                    {
                        order.RechargeMsg += "用户不存在||";
                        totalPrice += chargeVbiNum;
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

                    string result = PostAndGet.HttpGetString("http://login.sns.hongxiu.com/comlogin.aspx?url=http%3A//www.hongxiu.com/", "", ref cookie);
             
                    string comLoginUrl = "http://login.sns.hongxiu.com/comlogin.aspx?url=http://pay.hongxiu.com";
                    OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.HongXiu,false);
                    string  comLoginData = "htmlUserName=" + orderChargeAccount.ChargeAccount + "&htmlPassword=" + orderChargeAccount.ChargePassword + "&iskeeplogin=true&x=43&y=10&postcontent=";
                    result = PostAndGet.HttpPostString_HX(comLoginUrl, comLoginData, ref cookie,"login.sns.hongxiu.com", "http://pay.hongxiu.com/default.shtml");

                    result = PostAndGet.HttpGetString("http://pay.hongxiu.com/model/GetUserBaseInfo.aspx?roundmun=0.5841341522289687", "", ref cookie);

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + orderChargeAccount.ChargeAccount + "||" + orderChargeAccount.ChargePassword
                        + ",帐号登录返回：" + result, LogPathFile.Recharge);

                    if (result.Contains(orderChargeAccount.ChargeAccount))
                    {
                        return true;
                    }
                    else
                    {
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
        /// <summary>
        /// 提交数量
        /// </summary>
        /// <param name="totalGameNum">剩余要充值的总v币数量</param>
        /// <param name="chargeVbiNum">当前要充值的V币数量</param>
        bool GetChargeNum(int totalGameNum, ref int chargeVbiNum)
        {
            //1元=50红袖币（支持面额：2元、5元、10元、15元、20元、30元）
            bool status = true;

            if (totalGameNum >= 30)
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
            else if (totalGameNum >= 5)
            {
                chargeVbiNum = 5;
            }
            else if (totalGameNum >= 2)
            {
                chargeVbiNum = 2;
            }
            else
            {
                status = false;
            }

            return status;
        }
    }
}
