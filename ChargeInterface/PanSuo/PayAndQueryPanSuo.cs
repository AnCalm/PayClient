using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ChargeInterface.PanSuo
{
    public class PayAndQueryPanSuo
    {
        public const string submitUrl = "http://ddjs.666sup.com/Service/OrderReceive.ashx";
        public const string QueryUrl = "http://xxcx.666sup.com/Service/CommOrderQry.ashx";
        public const string key = "427ee21dec9b49779f2cde44c4c04607";
        public const string merchantID = "Num10333";

        public string SubmitOrder(Order order, string str = null)
        {
            CookieContainer coockie = new CookieContainer();

            if (string.IsNullOrEmpty(str))
                str = setPostDate(order);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " PanSuo 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

            string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

            WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " PanSuo 提交返回:" + result, LogPathFile.Recharge.ToString());

            return result;
        }

        string setPostDate(Order order)
        {
            string businessId = merchantID; //商户号	由SUP系统分配每个商户唯一的一个商户号
            string userOrderId = order.OrderInsideID; //	商户订单号（流水号） 由商户自定义，最大长度不超过32位的唯一流水号
            string goodsId = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode); //	用户商品编号 SUP系统商品编号，由运营人员告知
            string userName = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8); //	充值用户名 一般是通行证账号或QQ号
            string gameName = ""; //	游戏名称 用户充值游戏名称。见游戏名称规则。例：QQ传“Q币”，话费传“话费”
            string gameAcct = ""; //	游戏账号 如果充值需要填，则要传值。例如充值魔兽世界 除通行证账号外还要传游戏账号，见游戏账号规则

            string gameArea = ""; //	游戏区 如果充值需要填，则要传值。见游戏区服规则

            string gameType = ""; //	充值类型 见充值类型规则

            string acctType = ""; //	账号类型 见账号类型规则。例：QQ传“QQ号”，话费传“手机号”

            string goodsNum = order.BuyAmount.ToString(); //	充值数量 整数，充值总数量，单位是元 
            string gameSrv = ""; //	游戏服 如果充值需要填，则要传值。见游戏区服规则
            string orderIp = order.CustomerIp; //	充值ip 真实玩家充值请求ip 
            string noticeUrl = System.Web.HttpUtility.UrlEncode("http://116.62.44.48/NotifyFromPanSuo.aspx", Encoding.UTF8); //	异步通知地址 该url地址不能带任何参数

            string sign = ""; // 签名 md5 32位小写，做参数验证用 md5(businessId + userOrderId+ goodsId + goodsNum + orderIp + key)


            string md5str = businessId + userOrderId + goodsId + goodsNum + orderIp + key;
            sign = Md5Helper.MD5Encrypt(md5str);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("businessId={0}", businessId);
            str.AppendFormat("&userOrderId={0}", userOrderId);
            str.AppendFormat("&goodsId={0}", goodsId);
            str.AppendFormat("&userName={0}", userName);
            str.AppendFormat("&gameName={0}", gameName);
            str.AppendFormat("&gameAcct={0}", gameAcct);
            str.AppendFormat("&gameArea={0}", gameArea);
            str.AppendFormat("&gameType={0}", gameType);
            str.AppendFormat("&acctType={0}", acctType);
            str.AppendFormat("&goodsNum={0}", goodsNum);
            str.AppendFormat("&gameSrv={0}", gameSrv);
            str.AppendFormat("&orderIp={0}", orderIp);
            str.AppendFormat("&noticeUrl={0}", noticeUrl);
            str.AppendFormat("&sign={0}", sign);


            return str.ToString();
        }


       public  string getGameValue(string gameName)
        {
            if (string.IsNullOrEmpty(gameName))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "SWProduct.xml");    //加载Xml文件 
            XmlElement root = doc.DocumentElement;   //获取根节点 
            XmlNodeList GameNodes = root.GetElementsByTagName("Game"); //获取GameList子节点集合 
            foreach (XmlNode node in GameNodes)
            {
                //string Code = ((XmlElement)node).GetAttribute("Code");   //获取Code属性值 
                foreach (var item in node.ChildNodes)
                {
                    string GameName = ((XmlElement)node).GetElementsByTagName("GameName")[0].InnerText;
                    string GameValue = ((XmlElement)node).GetElementsByTagName("GameValue")[0].InnerText;

                    if (GameName.Equals(gameName))
                        return GameValue;
                }
            }

            return null;
        }

       public bool isChinese(string str)
       {
           for (int i = 0; i < str.Length; i++)
           {
               if ((int)str[i] > 127)
               {
                   return true;
               }
           }
           return false;
       }
    }
}
