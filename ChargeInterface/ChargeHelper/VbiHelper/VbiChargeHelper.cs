using Common;
using Common.LogHelper;
using Common.UUWise;
using EntityDB;
using MSScriptControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChargeInterface.ChargeHelper.Charge.VbiHelper
{
    public class VbiChargeHelper
    {

        /// <summary>
        /// 调用JavaScript 方法
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="pars">方法参数</param>
        /// <param name="jspath">要调用方法的JavaScript文件路径</param>
        /// <returns></returns>
        static object RunScript(string method, string[] pars, string jspath)
        {
            try
            {
                MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();
                sc.Language = "javascript";

                string javascript1 = System.IO.File.ReadAllText(jspath);
                sc.AddCode(javascript1);
                string temppars = "";
                foreach (string s in pars)
                {
                    temppars += "'" + s + "',";
                }
                //去除最后一个,
                temppars = temppars.Remove(temppars.LastIndexOf(","));
                temppars += ")";
                string mainCons = method + "(" + temppars;
                object obj = sc.Eval(mainCons);
                return obj;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        /// <summary>
        /// Vbi支付密码
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetPwdMethod(string[] args)
        {
            try
            {
                return RunScript("hex_md5", args, AppDomain.CurrentDomain.BaseDirectory + "\\VbiMd5.js").ToString();

                //Type obj = Type.GetTypeFromProgID("ScriptControl");
                //if (obj == null) return null;
                //object ScriptControl = Activator.CreateInstance(obj);
                //obj.InvokeMember("Language", System.Reflection.BindingFlags.SetProperty, null, ScriptControl, new object[] { "JavaScript" });
                //string js = System.IO.File.ReadAllText(@"E:\PayClient\PayClient\PayClient\Common\VbiHelper\VbiMd5.js");
                //obj.InvokeMember("AddCode", System.Reflection.BindingFlags.InvokeMethod, null, ScriptControl, new object[] { js });
                //return obj.InvokeMember("Eval", System.Reflection.BindingFlags.InvokeMethod, null, ScriptControl, new object[] { "hex_md5(" + args[0] + ")" }).ToString();

            //string javascript1 = System.IO.File.ReadAllText(@"E:\PayClient\PayClient\PayClient\Common\VbiHelper\VbiMd5.js");
           

            // System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();

            //parameters.GenerateInMemory = true;

            // System.CodeDom.Compiler.CodeDomProvider _provider = new Microsoft.JScript.JScriptCodeProvider();

            // System.CodeDom.Compiler.CompilerResults results = _provider.CompileAssemblyFromSource(parameters, javascript1);
 
            // System.Reflection.Assembly assembly = results.CompiledAssembly;

            // Type _evaluateType = assembly.GetType("aa.JScript");

            // object obj = _evaluateType.InvokeMember("hex_md5", System.Reflection.BindingFlags.InvokeMethod,
            // null, null, args);

            // return obj.ToString();
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public string GetTimeByJs(string[] args)
        {
            Type obj = Type.GetTypeFromProgID("ScriptControl");
            if (obj == null) return null;
            object ScriptControl = Activator.CreateInstance(obj);
            obj.InvokeMember("Language", System.Reflection.BindingFlags.SetProperty, null, ScriptControl, new object[] { "JavaScript" });
            string js = System.IO.File.ReadAllText(@"E:\PayClient\PayClient\PayClient\Common\VbiHelper\VbiMd5.js");
            obj.InvokeMember("AddCode", System.Reflection.BindingFlags.InvokeMethod, null, ScriptControl, new object[] { js });
            return obj.InvokeMember("Eval", System.Reflection.BindingFlags.InvokeMethod, null, ScriptControl, new object[] { "hex_md5(" + args [0]+ ")" }).ToString();
        }    


        /// <summary>
        /// V币钱包充值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="order"></param>
        /// <param name="orderChargeAccount"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string VbiCharge_Mix(string data, Order order, OrderChargeAccount orderChargeAccount, CookieContainer cookie)
        {
            try
            {
                 
                #region 固话钱包

                string result = PostAndGet.HttpPostString("http://s2.vnetone.com/Default.aspx", data, ref cookie);

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",订单第三步提交返回：" + result, LogPathFile.Recharge);

                if (result.Contains("操作失败"))
                {
                    return "操作_Failure";
                }

                string __VIEWSTATE = Regex.Match(result, @"id=""__VIEWSTATE"" value=""(.*?)"" />").Groups[1].Value;
                string __EVENTVALIDATION = Regex.Match(result, @"id=""__EVENTVALIDATION"" value=""(.*?)"" />").Groups[1].Value;
                string txtVBCard = "15%E4%BD%8D%E6%95%B0%E5%AD%97"; //Regex.Match(result, @"id=""txtVBCard"" value=""(.*?)"" />").Groups[1].Value;

                #endregion

                #region 获取代充帐号
                string txtQBCard = orderChargeAccount.ChargeAccount; //代充账号
                string txtQBPwd = orderChargeAccount.PayPassword; //支付密码
                order.ChargeAccountInfo = order.ChargeAccountInfo + txtQBCard + "," + txtQBPwd + "||";
                #endregion

                #region 钱包最后一步提交
                int reChargeCount = 0;  //验证码重试次数
            ReCharge:
                string txtQBValCode = ""; //验证码
                int codeid = 0;
                WrapperHelp.GetCodeByByte_UU("http://s2.vnetone.com/Validate/GetImageCodeComWallet.aspx?time=1480514113434434", ref cookie, 1004, ref txtQBValCode, ref codeid);

                StringBuilder PostdataBuilder = new StringBuilder();
                PostdataBuilder.AppendFormat("__VIEWSTATE={0}", System.Web.HttpUtility.UrlEncode(__VIEWSTATE, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&__EVENTVALIDATION={0}", System.Web.HttpUtility.UrlEncode(__EVENTVALIDATION, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&txtVBCard={0}", txtVBCard);
                PostdataBuilder.AppendFormat("&txtVBCardPwd={0}", "");
                PostdataBuilder.AppendFormat("&txtValCode={0}", "");
                PostdataBuilder.AppendFormat("&txtQBCard={0}", System.Web.HttpUtility.UrlEncode(txtQBCard, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&txtQBPwd={0}", VbiChargeHelper.GetPwdMethod(new string[] { txtQBPwd }));
                PostdataBuilder.AppendFormat("&txtQBValCode={0}", txtQBValCode);
                PostdataBuilder.AppendFormat("&btnWalletSubmit.x={0}", "62");
                PostdataBuilder.AppendFormat("&btnWalletSubmit.y={0}", "10");

                result = PostAndGet.HttpPostString("http://s2.vnetone.com/Default.aspx", PostdataBuilder.ToString(), ref cookie);

                if (result.Contains("验证码输入不正确") && reChargeCount < 10)
                {
                    WrapperHelp.reportError(codeid);
                    reChargeCount++;
                    goto ReCharge;
                }

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",订单第四步提交参数：" + PostdataBuilder.ToString() + ",订单第四步提交返回:" + result, LogPathFile.Recharge);
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                   + ",充值异常信息：" + ex.Message, LogPathFile.Exception);
                return "充值异常";
            }
        }

        public static string VbiCharge_Max(string data, Order order, OrderChargeAccount orderChargeAccount, CookieContainer cookie)
        {
            try
            {

                #region 固话钱包

                string result = PostAndGet.HttpPostString("http://s2.vnetone.com/Default.aspx", data, ref cookie);

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",订单第三步提交返回：" + result, LogPathFile.Recharge);

                if (result.Contains("操作失败"))
                {
                    return "操作失败_F";
                }

                string __VIEWSTATE = Regex.Match(result, @"id=""__VIEWSTATE"" value=""(.*?)"" />").Groups[1].Value;
                string __EVENTVALIDATION = Regex.Match(result, @"id=""__EVENTVALIDATION"" value=""(.*?)"" />").Groups[1].Value;

                #endregion

                #region 获取代充帐号
                string txtQBCard = orderChargeAccount.ChargeAccount; //代充账号
                string txtQBPwd = orderChargeAccount.PayPassword; //支付密码
                order.ChargeAccountInfo = order.ChargeAccountInfo + txtQBCard + "," + txtQBPwd + "||";
                #endregion

                #region 钱包最后一步提交
                int reChargeCount = 0;  //验证码重试次数
            ReCharge:
                string txtQBValCode = ""; //验证码
                int codeid = 0;
                WrapperHelp.GetCodeByByte_UU("http://s2.vnetone.com/Validate/GetImageCodeSinWallet.aspx?time=148311417206464", ref cookie, 1004, ref txtQBValCode, ref codeid);

                StringBuilder PostdataBuilder = new StringBuilder();
                PostdataBuilder.AppendFormat("__VIEWSTATE={0}", System.Web.HttpUtility.UrlEncode(__VIEWSTATE, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&__EVENTVALIDATION={0}", System.Web.HttpUtility.UrlEncode(__EVENTVALIDATION, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&uid9qb={0}", System.Web.HttpUtility.UrlEncode(txtQBCard, Encoding.UTF8));
                PostdataBuilder.AppendFormat("&zfpwd9qb={0}", VbiChargeHelper.GetPwdMethod(new string[] { txtQBPwd }));
                PostdataBuilder.AppendFormat("&TextBox8={0}", txtQBValCode);
                PostdataBuilder.AppendFormat("&ImageButton1.x={0}", "62");
                PostdataBuilder.AppendFormat("&ImageButton1.y={0}", "10");

                result = PostAndGet.HttpPostString("http://s2.vnetone.com/Default.aspx", PostdataBuilder.ToString(), ref cookie, "http://s2.vnetone.com/Default.aspx");

                if (result.Contains("验证码输入不正确") && reChargeCount < 10)
                {
                    WrapperHelp.reportError(codeid);
                    reChargeCount++;
                    goto ReCharge;
                }

                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                    + ",订单第四步提交参数：" + PostdataBuilder.ToString() + ",订单第四步提交返回:" + result, LogPathFile.Recharge);
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                WriteLog.Write("订单号:" + order.OrderInsideID + ",代充商品：" + order.ProductName + "代充帐号：" + order.TargetAccount
                   + ",充值异常信息：" + ex.Message, LogPathFile.Exception);
                return "充值异常";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="order"></param>
        /// <param name="orderChargeAccount"></param>
        /// <param name="cookie"></param>
        /// <param name="PayNum">充值数量</param>
        /// <returns></returns>
        public static string VbiCharge(string data, Order order, OrderChargeAccount orderChargeAccount, CookieContainer cookie,int PayNum=0)
        {
            if(PayNum>=100)
            {
                return VbiCharge_Max(data, order, orderChargeAccount, cookie);
            }
            else
            {
                return VbiCharge_Mix(data, order, orderChargeAccount, cookie);
            }
        }
    }
}
