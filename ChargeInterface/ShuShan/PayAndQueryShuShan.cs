using ChargeInterface.AntoInterface;
using ChargeInterface.SUP;
using Common;
using Common.LogHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DBOperation.SQLHelper;
using System.Xml;

namespace ChargeInterface.ShuShan
{
    public class PayAndQueryShuShan
    {
        public const string submitUrl = "http://api.shushanzx.com/Api/Pay";
        public const string queryUrl = "http://api.shushanzx.com/Api/QueryOrder";
        public const string key = "geqewl5u1jae4hp";
        public const string merchantID = "10047";

        public string SubmitOrder(Order order, string str = null)
        {
            try
            {
                CookieContainer coockie = new CookieContainer();

                if (string.IsNullOrEmpty(str))
                    str = setPostDate(order);

                WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " ShuShan 提交参数:" + str.ToString(), LogPathFile.Recharge.ToString());

                string result = PostAndGet.HttpPostString(submitUrl, str.ToString(), ref coockie);

                WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " ShuShan 提交返回:" + result, LogPathFile.Recharge.ToString());

                return result;

            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:SubmitOrder，订单号：" + order.OrderInsideID + " 异常:" + ex.Message, LogPathFile.Exception.ToString());
                return null;
            }
        }

        string setPostDate(Order order)
        {
            string MerchantID = merchantID;	//商家编号
            string MerchantOrderID = order.OrderInsideID;	//商家订单编号（不允许重复）
            string ProductID = SQLProduct.getChargeClassProductCode(order.ProductID, order.MerchantCode); //商品编号
            string BuyAmount = order.BuyAmount.ToString(); ;	//充值数量
            string TargetAccount = string.Empty;  //充值账户
            string TargetAccount_Sign = string.Empty;

            if (order.GameName.Contains("新水浒Q传"))
            {
                if (order.TargetAccount.Contains("手机号码"))
                    TargetAccount = order.TargetAccount.Replace("手机号码", "");
                else if (order.TargetAccount.Contains("其他邮箱【请填写完整账户】"))
                    TargetAccount = order.TargetAccount.Replace("其他邮箱【请填写完整账户】", "");
                else if (order.TargetAccount.Contains("无"))
                    TargetAccount = order.TargetAccount.Replace("无", "");
            }
            else if (order.GameName.Contains("魔兽世界"))
                TargetAccount = order.TargetAccount.Replace("+", ";");

            if (string.IsNullOrEmpty(TargetAccount))
                TargetAccount = order.TargetAccount;

            TargetAccount_Sign = TargetAccount;

            if (isChinese(TargetAccount))
                TargetAccount = System.Web.HttpUtility.UrlEncode(order.TargetAccount, Encoding.UTF8);

            string TargetAccountType = System.Web.HttpUtility.UrlEncode(order.TargetAccountType, Encoding.UTF8);	//充值账户类型
            string TargetAccountTypeName = System.Web.HttpUtility.UrlEncode(order.TargetAccountTypeName, Encoding.UTF8);	//充值账户类型名称

            string RechargeMode = string.Empty;	//充值方式

            if (order.RechargeModeName.Contains("帐号直充"))
                RechargeMode = "1";
            else if (order.RechargeModeName.Contains("点数寄售"))
                RechargeMode = "2";
            else if (order.RechargeModeName.Contains("魔兽世界游戏时间"))
                RechargeMode = "WOW_TIME";

            if (order.GameName.Contains("春秋Q传") || order.GameName.Contains("剑网3")
                || order.GameName.Contains("反恐行动") || order.GameName.Contains("剑侠世界"))
            {
                if (order.RechargeModeName.Contains("金币"))
                    RechargeMode = "5";
                else if (order.RechargeModeName.Contains("通宝"))
                    RechargeMode = "6";
                else if (order.RechargeModeName.Contains("月卡"))
                    RechargeMode = "1";
                else if (order.RechargeModeName.Contains("点卡"))
                    RechargeMode = "2";
            }

            string RechargeModeName = string.Empty; //充值方式名称
            if (order.RechargeModeName.Contains("起凡一卡通"))
                RechargeModeName = string.Empty;
            else
                RechargeModeName = System.Web.HttpUtility.UrlEncode(order.RechargeModeName, Encoding.UTF8);

            string Game = getGameValue(order.GameName);//充值游戏
            string GameName = string.Empty;    //充值游戏名称
            GameName = System.Web.HttpUtility.UrlEncode(order.GameName, Encoding.UTF8);

            string Area = "";//游戏区域
            string AreaName = System.Web.HttpUtility.UrlEncode(order.AreaName, Encoding.UTF8);	//游戏区域名称
            string Server = "";//游戏服务器
            string ServerName = System.Web.HttpUtility.UrlEncode(order.ServerName, Encoding.UTF8);	//游戏服务器名称

            if (order.GameName.Contains("魔域") || order.GameName.Contains("魔域掉钱版") || order.GameName.Contains("魔域口袋版")
                || order.GameName.Contains("机战") || order.GameName.Contains("征服"))
            {
                getmoyuAreaAndServer(order.GameName, order.AreaName, order.ServerName, ref Area, ref Server);

                switch (order.RechargeModeName)
                {
                    case "魔石卡":
                        RechargeMode = "1021101902000000001";
                        break;
                    case "神石卡":
                        RechargeMode = "1021372029000000001";
                        break;
                    case "太阳石卡":
                        RechargeMode = "1021111902000000001";
                        break;
                    default:
                        RechargeMode = "";
                        break;

                }

            }
            else
            {
                Area = new ChargeInterface.MBJ.PayAndQueryMBJ().getAreaValue(order.AreaName, ProductID, AreaValueType.ShuShan);
                Server = System.Web.HttpUtility.UrlEncode(order.Server, Encoding.UTF8);
            }

            string CustomerIP = order.CustomerIp;	//客户IP/客户区域，用来确定消费区域，只需传一个即可。如果都传以IP为准，如果都不传则默认区域为全国。
            string CustomerRegion = "";
            string ResponseUrl = System.Web.HttpUtility.UrlEncode("http://116.62.44.48/NotifyFromShuShan.aspx", Encoding.UTF8); ;	//接收异步通知订单状态的Url
            string Sign = ""; //数字签名数字签名

            if (string.IsNullOrEmpty(TargetAccount_Sign))
                TargetAccount_Sign = order.TargetAccount;

            string md5str = MerchantID + MerchantOrderID + ProductID + BuyAmount + TargetAccount_Sign + key;
            Sign = Md5Helper.MD5Encrypt(md5str);

            StringBuilder str = new StringBuilder();
            str.AppendFormat("MerchantID={0}", MerchantID);
            str.AppendFormat("&MerchantOrderID={0}", MerchantOrderID);
            str.AppendFormat("&ProductID={0}", ProductID);
            str.AppendFormat("&BuyAmount={0}", BuyAmount);
            str.AppendFormat("&TargetAccount={0}", TargetAccount);
            str.AppendFormat("&TargetAccountType={0}", TargetAccountType == null ? "" : TargetAccountType);
            str.AppendFormat("&TargetAccountTypeName={0}", TargetAccountTypeName == null ? "" : TargetAccountTypeName);
            str.AppendFormat("&RechargeMode={0}", RechargeMode == null ? "" : RechargeMode);
            str.AppendFormat("&RechargeModeName={0}", RechargeModeName == null ? "" : RechargeModeName);
            str.AppendFormat("&Game={0}", Game == null ? "" : Game);
            str.AppendFormat("&GameName={0}", GameName == null ? "" : GameName);
            str.AppendFormat("&Area={0}", Area == null ? "" : Area);
            str.AppendFormat("&AreaName={0}", AreaName == null ? "" : AreaName);
            str.AppendFormat("&Server={0}", Server == null ? "" : Server);
            str.AppendFormat("&ServerName={0}", ServerName == null ? "" : ServerName);
            str.AppendFormat("&CustomerIP={0}", CustomerIP == null ? "" : CustomerIP);
            str.AppendFormat("&CustomerRegion={0}", CustomerRegion == null ? "" : CustomerRegion);
            str.AppendFormat("&ResponseUrl={0}", ResponseUrl);
            str.AppendFormat("&Sign={0}", Sign);

            return str.ToString();
        }


        public string getGameValue(string gameName)
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

        public void getmoyuAreaAndServer(string gamename, string areaname, string servername, ref string areaid, ref string serverid)
        {
            try
            {
                string url = "https://xmlserver.99.com/my/mycharge.xml?0.8311741468522764";

                switch (gamename)
                {
                    case "魔域":
                        url = "https://xmlserver.99.com/my/mycharge.xml?0.8311741468522764";
                        break;
                    case "魔域掉钱版":
                        url = "https://xmlserver.99.com/my/Chsjmy.xml?0.5738079243209351";
                        break;
                    case "魔域口袋版":
                        url = "https://xmlserver.99.com/my/mysjcharge.xml?0.3160789587639954";
                        break;
                    case "机战":
                        url = "https://xmlserver.99.com//jz/jzcharge.xml?0.8663311481081863";
                        break;
                    case "征服":
                        url = "https://xmlserver.99.com/zf/zfcharge.xml?0.9778757448608422";
                        break;
                    default:
                        url = "https://xmlserver.99.com/my/mycharge.xml?0.8311741468522764";
                        break;

                }

                Dictionary<string, string> dicArea = new Dictionary<string, string>();

                Dictionary<string, List<Dictionary<string, string>>> dicServer = new Dictionary<string, List<Dictionary<string, string>>>();


                CookieContainer coockie = new CookieContainer();
                string result = PostAndGet.HttpGetString(url, "", ref  coockie);

                XmlDocument docArea = new XmlDocument();
                docArea.LoadXml(result);    //加载Xml文件 
                XmlElement rootArea = docArea.DocumentElement;   //获取根节点 
                XmlNodeList AreaNodes = rootArea.GetElementsByTagName("MainTable"); //获取area
                foreach (XmlNode node in AreaNodes)
                {
                    //string Code = ((XmlElement)node).GetAttribute("Code");   //获取Code属性值 

                    string key = ((XmlElement)node).GetElementsByTagName("key")[0].InnerText;
                    string name = ((XmlElement)node).GetElementsByTagName("name")[0].InnerText;
                    dicArea.Add(key, name);
                }

                XmlDocument docServer = new XmlDocument();
                docServer.LoadXml(result);    //加载Xml文件 
                XmlElement rootServer = docServer.DocumentElement;   //获取根节点 
                XmlNodeList ServerNodes = rootServer.GetElementsByTagName("SubTable"); //获取sever
                foreach (XmlNode node in ServerNodes)
                {
                    string key = ((XmlElement)node).GetElementsByTagName("key")[0].InnerText;
                    string name = ((XmlElement)node).GetElementsByTagName("name")[0].InnerText;
                    string ParentKey = ((XmlElement)node).GetElementsByTagName("ParentKey")[0].InnerText;

                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add(key, name);
                    bool isNew = true;

                    foreach (string serverkey in dicServer.Keys)
                    {
                        if (ParentKey == serverkey)
                        {
                            dicServer[serverkey].Add(dic);
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                    {
                        List<Dictionary<string, string>> lst = new List<Dictionary<string, string>>();
                        lst.Add(dic);
                        dicServer.Add(ParentKey, lst);
                    }
                }

                if (!string.IsNullOrEmpty(areaname))
                {
                    foreach (string item in dicArea.Keys)
                    {
                        if (dicArea[item] == areaname)
                        {
                            areaid = item;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(areaname) && !string.IsNullOrEmpty(servername))
                {
                    foreach (string key in dicArea.Keys)
                    {
                        if (dicArea[key] == areaname)
                        {
                            foreach (string parentKey in dicServer.Keys)
                            {
                                if (parentKey == key)
                                {
                                    foreach (Dictionary<string, string> dic in dicServer[parentKey])
                                    {
                                        foreach (string serverkey in dic.Keys)
                                        {
                                            if (servername == dic[serverkey])
                                            {
                                                serverid = serverkey;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:getmoyuAreaAndServer，异常：" + ex.Message, LogPathFile.Exception.ToString());
                throw;
            }

        }
    }
}
