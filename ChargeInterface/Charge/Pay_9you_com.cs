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
    public class Pay_9you_com : ICharge
    {
        public Order Charge(Order order)
        {
            try
            {
                CookieContainer cookie = new CookieContainer();
                if (!login(order, ref cookie))
                { }
            }
            catch (Exception)
            {
                throw;
            }

            return order;

        }

        bool login(Order order, ref CookieContainer cookie)
        {
            try
            {
                //OrderChargeAccount orderChargeAccount = SQLOrderChargeAccount.GetChargeAccount(OrderChargeAccountType.JiuYou, false);

                OrderChargeAccount orderChargeAccount = new OrderChargeAccount()
                {
                    ChargeAccount = "15072412234",
                    ChargePassword = "357440019"
                };

                cookie = Common.CookieOperation.CookieHelper.ReadCookiesFromDisk(orderChargeAccount.ChargeAccount);

                string result = string.Empty;
                result = PostAndGet.HttpGetString("http://pay.9you.com/pay", "", ref cookie);
                if (result.Contains("欢迎您") && result.Contains("退出"))
                     return true;

                int loginCount = 0;
            
                while (loginCount < 5)
                {
                    cookie = new CookieContainer();

                    //ssosessionid=974ad182-625f-4783-afbc-9c67c105df7b; domain=.9you.com; path=/
                    Cookie ck = new Cookie
                    {
                        Domain = ".9you.com",
                        Name = "ssosessionid",
                        Value = "06514a9c-de8a-48f9-adbd-4986b7a92949",
                        Path="/"
                    };
                    cookie.Add(ck);

                    
                    result = PostAndGet.HttpGetString_9Y("https://login.passport.9you.com/","", ref cookie,"login.passport.9you.com");

                    StringBuilder Data = new StringBuilder();
                    Data.AppendFormat("id={0}", "null");
                    Data.AppendFormat("&userName={0}", orderChargeAccount.ChargeAccount);
                    Data.AppendFormat("&password={0}", orderChargeAccount.ChargePassword);
                    Data.AppendFormat("&sourceUrl={0}", "login.jsp");
                    Data.AppendFormat("&continue={0}", "null");
                    Data.AppendFormat("&userIp={0}", "null");
                    Data.AppendFormat("&mw={0}", "");
                    Data.AppendFormat("&ekeyPassword={0}", "");
                    Data.AppendFormat("&otpPassword={0}", "");
                    Data.AppendFormat("&s={0}", "null");


                    result = PostAndGet.HttpPostString_9Y("https://login.passport.9you.com/checkCode", 
                        Data.ToString(), ref cookie, "login.passport.9you.com", "https://login.passport.9you.com/");

                    WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + orderChargeAccount.ChargeAccount + "||" + orderChargeAccount.ChargePassword
                       + ",帐号登录返回：" + result, LogPathFile.Recharge);

                    result = PostAndGet.HttpGetString("http://pay.9you.com/pay", "", ref cookie);

                    if (result.Contains("欢迎您") && result.Contains("退出"))
                    {
                        //登录成功
                        Common.CookieOperation.CookieHelper.WriteCookiesToDisk(orderChargeAccount.ChargeAccount, cookie);
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
    }
}
