using Common;
using Common.LogHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ChargeInterface.SUP
{
    public class ManageSUP
    {
        string submitUrl = "http://hphy.eyuesale.com/api/SaveOrder";
        string getCatalogUrl = "http://hphy.eyuesale.com/api/QueryAllCatalog";
        string key = "75e2ae34888644289751ee1082538d2b";
        string merchantID = "10002";
        CookieContainer coockie;

        public string SubmitOrder(Order order, string str = null)
        {
            coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " sup 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " sup 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {
            string catagory = string .Empty ; ;
            string productNo = getProductNo(order.ProductID);
           
            string ChargeAccount = "";
            string RoleName = "";
            string[] arr = order.TargetAccount.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr != null)
            {
                ChargeAccount = arr[0];
                if (arr.Length > 1)
                    RoleName = arr[1];
            }

            if (string.IsNullOrEmpty(ChargeAccount))
                ChargeAccount = order.TargetAccount;

            string md5str = merchantID + catagory + order.BuyAmount + ChargeAccount + key;
            string Sign = Md5Helper.GetMD5String_utf8(md5str);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("MerchantID={0}", merchantID); //商家编号
            str.AppendFormat("&Catagory={0}", catagory);  //目录（对接的每一套系统此参数都是有差异的,具体数据请通过接口获取）
            str.AppendFormat("&ProductNo={0}", productNo); //商品编号（关联的商品编号，如果为‘0’系统自动分配）
            str.AppendFormat("&BuyNumber={0}", order.BuyAmount); //购买数量（每个充值号购买数量）
            str.AppendFormat("&ChargeAccount={0}", System.Web.HttpUtility.UrlEncode(ChargeAccount, Encoding.UTF8)); //充值号（多个用','分隔）
            str.AppendFormat("&Sign={0}", Sign);      //数字签名，MD5加密（MerchantID+Catagory+BuyNumber+ChargeAccount   +key）
            str.AppendFormat("&OrderID={0}", order.OrderInsideID);   //合作商自定义的下游订单编号
            str.AppendFormat("&FaceValue={0}", Convert.ToInt16( order.ProductParValue)); //订单面值(默认为0)
            str.AppendFormat("&GameName={0}",System.Web.HttpUtility.UrlEncode( order.GameName,Encoding.UTF8));  //充值名称（如DNF点券）
            str.AppendFormat("&Area={0}", System.Web.HttpUtility.UrlEncode(order.AreaName,Encoding.UTF8));      //充值区域
            str.AppendFormat("&Srv={0}", System.Web.HttpUtility.UrlEncode(order.ServerName, Encoding.UTF8));       //充值服务器
            str.AppendFormat("&Otherinfo={0}", ""); //其他信息
            str.AppendFormat("&ReturnUrl={0}", "http://116.62.44.48/NotifyFromSUP.aspx"); //回调地址(详细说明见下方)
            str.AppendFormat("&RechargeType={0}", ""); //充值类型(具体数据参考文档最后)
            str.AppendFormat("&BuyerIp={0}", "");      //买家IP地址
            str.AppendFormat("&OrderType={0}", "0");   //订单类型(1:官方卡密 0:接口供货),
            str.AppendFormat("&RoleName={0}", RoleName);    //角色名称(部分游戏有)

            return str.ToString();
        }

        public string getCatalog(string catalogName)
        {
            coockie = new CookieContainer();
            string strmd5 = merchantID + key;
            string sign = Md5Helper.GetMD5String_utf8(strmd5);
            StringBuilder str = new StringBuilder();
            str.AppendFormat("MerchantI={0}", merchantID);
            str.AppendFormat("&Sign={0}", sign);  //数字签名(MerchantID+ key)

            string result = PostAndGet.HttpPostString(getCatalogUrl, str.ToString(), ref coockie);

            Regex re = new Regex(@"<No>(.*?)</No>\s+<Name>(.*?)</Name>
", RegexOptions.None);
            MatchCollection mc = re.Matches(result);
            foreach (Match ma in mc)
            {
                if (ma.Groups[2].Value.ToString().Contains("catalogName"))
                {
                    return ma.Groups[1].Value;
                }
            }

            return null;
        }

        public string getProductNo(string ProductID)
        {
            string productNo = "";
            switch (ProductID)
            {
                case "78302":
                    productNo = "1037";
                    break;
                default:
                    break;
            }

            return productNo;
        }

        public string queryOrder(Order order,string merchantOrderID, ref string msg)
        {
            int num = 0;
            string status = "未处理";

            while (num < 5)
            {
                

                coockie = new CookieContainer();
                string strmd5 = merchantID + merchantOrderID + key;
                string sign = Md5Helper.GetMD5String_utf8(strmd5);
                StringBuilder str = new StringBuilder();
                str.AppendFormat("MerchantID={0}", merchantID);
                str.AppendFormat("&OrderID={0}", merchantOrderID); //订单编号(多个订单以','分隔)
                str.AppendFormat("&Sign={0}", sign);  //数字签名(MerchantID+OrderID+key)
                str.AppendFormat("&MerchantType={0}", "0"); //商家类型（默认为0）:0进货商、1供货商
                str.AppendFormat("&OrderIDType={0}", "0"); //状态（默认为0）：0易约销售系统生成的订单编号、1合作商自定义的下游订单编号


                WriteLog.Write("方法:Charge，订单号：" + order.OrderInsideID + " sup 订单查询参数:" + str.ToString(), LogPathFile.Recharge.ToString());

                string result = PostAndGet.HttpPostString("http://hphy.eyuesale.com/api/QueryOrder", str.ToString(), ref coockie);

                WriteLog.Write("方法:Charge，订单号：" + order.OrderInsideID + " sup 订单查询:" + result, LogPathFile.Recharge.ToString());

                status = Regex.Match(result, @"<Status>(.*?)</Status>").Groups[1].Value;
                msg = Regex.Match(result, @"<DetailMsg>(.*?)</DetailMsg>").Groups[1].Value;

                if (status == "未处理")
                {
                    System.Threading.Thread.Sleep(2 * 1000);
                    num++;
                }
                if (status == "充值成功" || status == "充值失败")
                    break;

                if (num > 5)
                {
                    status = "可疑";
                    msg = "查询订单结果失败";
                }
            }
            return status;
        }




        #region  订单提交返回状态（非订单充值状态）

        // 0

        //成功

        //1

        //失败

        //2

        //系统异常

        //3

        //参数有误

        //4

        //商家编号不存在

        //5

        //密钥错误

        //6

        //商家编号应为数字

        //101

        //进货Api未开通

        //102

        //商品目录不正确

        //103

        //最晚处理时间格式有误

        //104

        //进货功能未开通

        //105

        //账号余额不足

        //106

        //无关联供货商品

        //107

        //供货价格太高

        //108

        //商品目录应为数字

        //109

        //购买数量应为数字

        //110

        //商品编号应为数字

        //111

        //最晚处理时间应大于当前时间

        //112

        //最晚处理时间应大于当前时间

        //113

        //最晚处理时间应在30分钟以后

        //114

        //禁售时间段内

        //115

        //订单面值有误

        //116

        //订单编号最长为32位

        //117

        //订单编号重复

        //201

        //商品编号有误

        //202

        //订单数量不在范围内

        //203

        //供货Api未开通

        //204

        //供货功能未开通

        //205

        //订单数量应为数字

        //301

        //订单状态有误

        //302

        //订单编号有误

        //303

        //订单处理失败处理超时

        //304

        //订单已处理或订单状态有误

        //305

        //订单状态应为数字

        //401

        //商家状态有误

        //402

        //订单查询无结果
        #endregion
    }
}
