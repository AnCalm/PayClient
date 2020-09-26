using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityDB;
using System.Text.RegularExpressions;
using System.Xml;
using Common.LogHelper;
using Common;
using DBOperation.SQLHelper;

namespace ChargeInterface.SW
{
    /// <summary>
    /// 数网接口
    /// </summary>
    public class ManageSW
    {
        string merchantID = string.Empty;
        string key = string.Empty;
        string getOrderurl = string.Empty;
        string notifyOrderurl = string.Empty;
        int count = 0;
        int time = 0;

        public ManageSW()
        {
            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SW).FirstOrDefault();
            if (clientConfig == null)
                return;

             merchantID = clientConfig.MerchantID;
             key = clientConfig.MerchantKey;
             getOrderurl = clientConfig.GetOrdersURL;
             notifyOrderurl = clientConfig.NotifyOrderURL;
             count = clientConfig.GetOrderCount == null ? 0 : Convert.ToInt32(clientConfig.GetOrderCount);
             time = clientConfig.GetOrderTime == null ? 0 : Convert.ToInt32(clientConfig.GetOrderTime);
        }

        /// <summary>
        /// 从数网获取订单
        /// </summary>
        /// <param name="merchantID">商户编号</param>
        /// <param name="key">商户密码</param>
        /// <param name="url">接口地址</param>
        /// <param name="count">订单数量</param>
        /// <returns></returns>
        public List<Order> getOrderFromSW(ref int getOrderTime)
        {
            getOrderTime = time;

            List<Order> orderSet = new List<Order>();

            try
            {
                string sign = Md5Helper.MD5Encrypt(merchantID + key);
                string postData = string.Format("MerchantID={0}&Count={1}&Sign={2}", merchantID, count, sign);


                System.Net.CookieContainer cookie = new System.Net.CookieContainer();

                string result = PostAndGet.HttpGetString(getOrderurl, postData, ref cookie);

                WriteLog.Write("方法:getOrderFromSW 获取订单信息：" + result, LogPathFile.Other);

                if (result.Contains("获取成功"))
                    assignmentOrder(orderSet, result);

            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:getOrderFromSW异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }
            
            return orderSet;
        }

        static void assignmentOrder(List<Order> orderSet, string item)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(item);//加载xml
                XmlNodeList xmlList = xml.GetElementsByTagName("order"); //取得节点名为order的XmlNode集合
                foreach (XmlNode xmlNode in xmlList)
                {
                    XmlNodeList childList = xmlNode.ChildNodes; //取得orders下的子节点集合
                    Order order = new Order();
                    foreach (XmlNode childNode in childList)
                    {
                        #region order
                        if ("order-id".Equals(childNode.Name))
                            order.OrderExternalID = childNode.InnerText.Trim();
                        else if ("product-id".Equals(childNode.Name))
                            order.ProductID = childNode.InnerText.Trim();
                        else if ("product-name".Equals(childNode.Name))
                            order.ProductName = childNode.InnerText.Trim();
                        else if ("product-par-value".Equals(childNode.Name))
                            order.ProductParValue = Convert.ToDecimal(childNode.InnerText.Trim());
                        else if ("product-sale-price".Equals(childNode.Name))
                            order.ProductSalePrice = Convert.ToDecimal(childNode.InnerText.Trim());
                        else if ("target-account".Equals(childNode.Name))
                            order.TargetAccount = childNode.InnerText.Trim();
                        else if ("target-account-type".Equals(childNode.Name))
                            order.TargetAccountType = childNode.InnerText.Trim();
                        else if ("target-account-type-name".Equals(childNode.Name))
                            order.TargetAccountTypeName = childNode.InnerText.Trim();
                        else if ("buy-amount".Equals(childNode.Name))
                            order.BuyAmount = Convert.ToInt32(childNode.InnerText.Trim());
                        else if ("total-sale-price".Equals(childNode.Name))
                            order.TotalSalePrice = Convert.ToDecimal(childNode.InnerText.Trim());
                        else if ("game".Equals(childNode.Name))
                            order.Game = childNode.InnerText.Trim();
                        else if ("game-name".Equals(childNode.Name))
                            order.GameName = childNode.InnerText.Trim();
                        else if ("area".Equals(childNode.Name))
                            order.Area = childNode.InnerText.Trim();
                        else if ("area-name".Equals(childNode.Name))
                            order.AreaName = childNode.InnerText.Trim();
                        else if ("server".Equals(childNode.Name))
                            order.Server = childNode.InnerText.Trim();
                        else if ("server-name".Equals(childNode.Name))
                            order.ServerName = childNode.InnerText.Trim();
                        else if ("recharge-mode".Equals(childNode.Name))
                            order.RechargeMode = childNode.InnerText.Trim();
                        else if ("recharge-mode-name".Equals(childNode.Name))
                            order.RechargeModeName = childNode.InnerText.Trim();
                        else if ("stock-merchant-id".Equals(childNode.Name))
                            order.StockMerchantId = childNode.InnerText.Trim();
                        else if ("stock-merchant-name".Equals(childNode.Name))
                            order.StockMerchantName = childNode.InnerText.Trim();
                        else if ("customer-ip".Equals(childNode.Name))
                            order.CustomerIp = childNode.InnerText.Trim();
                        else if ("customer-region".Equals(childNode.Name))
                            order.CustomerRegion = childNode.InnerText.Trim();
                        else if ("deal-date-time".Equals(childNode.Name))
                            order.DealDateTime = Convert.ToDateTime(childNode.InnerText.Trim());
                        #endregion
                    }
                    order.StartDatetime = DateTime.Now;
                    order.OrderInsideID = OrderHelper.GenerateId(order.StartDatetime, "01");
                    order.SuccessfulAmount = 0;
                    order.RechargeStatus = (int)OrderRechargeStatus.untreated;
                    order.IsNotify = false;
                    order.MerchantCode = MerchantCodeType.SW;
                    if (BelongPoProduct(order))
                    {
                        if (CheckRepeatOrder(order))
                            orderSet.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:assignmentOrder异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }
        }


        /// <summary>
        /// 通知数网订单结果
        /// </summary>
        /// <param name="merchantID">商户编号</param>
        /// <param name="key">商户密码</param>
        /// <param name="url">接口地址</param>
        /// <param name="order">订单</param>
        /// <returns></returns>
        public bool notigyOrderToSW( Order order, bool isprocessing=false)
        {
            WriteLog.Write("方法:订单开始通知数网，订单号：" + order.OrderInsideID, LogPathFile.Other.ToString());

            string sign = Md5Helper.MD5Encrypt(merchantID +order.OrderExternalID+ key);

            //订单状态（充值成功：1，充值失败：0，处理中：2， 可疑订单：3）
            int State = 1;
            string StateInfo ="充值成功";
            if (isprocessing)
            {
                State = 2;
                StateInfo = EnumService.GetDescription(OrderRechargeStatus.processing);
            }
            else
            {
                switch (order.RechargeStatus)
                {
                    case (int)OrderRechargeStatus.successful:
                        State = 1;
                        if (string.IsNullOrEmpty(order.RechargeMsg))
                            StateInfo = EnumService.GetDescription(OrderRechargeStatus.successful);
                        else
                            StateInfo = order.RechargeMsg;
                        break;
                    case (int)OrderRechargeStatus.failure:
                        State = 0;
                        if (string.IsNullOrEmpty(order.RechargeMsg))
                            StateInfo = EnumService.GetDescription(OrderRechargeStatus.failure);
                        else
                            StateInfo = order.RechargeMsg;
                        break;
                    case (int)OrderRechargeStatus.processing:
                        State = 2;
                        StateInfo = EnumService.GetDescription(OrderRechargeStatus.processing);
                        break;
                    case (int)OrderRechargeStatus.untreated:
                    case (int)OrderRechargeStatus.suspicious:
                        State = 3;
                        if (string.IsNullOrEmpty(order.RechargeMsg))
                            StateInfo = EnumService.GetDescription(OrderRechargeStatus.suspicious);
                        else
                            StateInfo = order.RechargeMsg;
                        
                        break;
                }
            }
            string postData = string.Format("MerchantID={0}&OrderID={1}&State={2}&StateInfo={3}&Sign={4}", merchantID, order.OrderExternalID,State,System.Web.HttpUtility.UrlEncode( StateInfo,Encoding.UTF8),sign);

            //item = new HttpItem()
            //{
            //    URL = url+"?"+postData,
            //    Encoding = Encoding.UTF8,//可选项 默认类会自动识别
            //    Method = "get"    //可选项 默认为Get
            //};

            //得到HTML代码
            //result = http.GetHtml(item);

            System.Net.CookieContainer cookie = new System.Net.CookieContainer();

            //string result = PostAndGet.HttpGetString(url, postData, ref cookie);

            string result = PostAndGet.HttpPostString(notifyOrderurl, postData, ref cookie);


            WriteLog.Write("方法:订单通知数网结果，订单号：" + order.OrderInsideID+",数网通知返回："+result, LogPathFile.Other.ToString());

            string notifyStatus = Regex.Match(result, @"<state>(.*?)</state>").Groups[0].Value;

            if (notifyStatus.Contains("1"))
                return true;
            else
                return false;

        }

        /// <summary>
        /// 是否属于等待充值的商品
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        static bool BelongPoProduct(Order order)
        {
            try
            {

                List<Product> productLst = SQLProduct.GetProducts(p => p.MerchantCode == MerchantCodeType.SW);
                if (productLst == null || productLst.Count == 0)
                    return false;

                foreach (Product item in productLst)
                {
                    if (order != null && order.ProductID != null)
                    {
                        var result = productLst.Count(p => p.ProductCode == order.ProductID && p.IsActive);
                        if (result != null && result > 0)
                            return true;
                    }

                }

                return false;


                //if (order != null && order.ProductID != null)
                //{
                //    if (order.ProductID.Equals("70623") || order.ProductID.Equals("70893") || order.ProductID.Equals("70927") || order.ProductID.Equals("71014")
                //        || order.ProductID.Equals("71015") || order.ProductID.Equals("71016") || order.ProductID.Equals("71017") || order.ProductID.Equals("71018"))
                //        return true;
                //    if (order.ProductID.Equals("70948"))
                //        return true;
                //    if (order.ProductID.Equals("70949") || order.ProductID.Equals("70950") || order.ProductID.Equals("70951") || order.ProductID.Equals("70952")
                //        || order.ProductID.Equals("70953") || order.ProductID.Equals("70954") || order.ProductID.Equals("71324") || order.ProductID.Equals("72161"))
                //        return true;
                //    if (order.ProductID.Equals("71195") || order.ProductID.Equals("70921"))
                //        return true;
                //}
                //return false;
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:BelongPoProduct异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
                throw;
            }
        }

        static bool CheckRepeatOrder(Order order)
        {
            try
            {
                if (order != null && order.OrderExternalID != null)
                {
                    bool result = new SQLOrder().IsOrders(p => p.OrderExternalID == order.OrderExternalID);

                    WriteLog.Write("SW重复订单,订单号： " + order.OrderExternalID + " ,是否有重复: " + result, LogPathFile.Other);

                    if (!result)
                        return true;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:CheckRepeatOrder异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }
            return false;
        }
    }
}
