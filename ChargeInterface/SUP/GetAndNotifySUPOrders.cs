using Common;
using Common.LogHelper;
using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ChargeInterface.SUP
{
    public class GetAndNotifySUPOrders
    {
        string merchantID = string.Empty;
        string key = string.Empty;
        string getOrderurl = string.Empty;
        string notifyOrderurl = string.Empty;
        int count = 0;
        int time = 0;

        public GetAndNotifySUPOrders()
        {
            ClientConfig clientConfig = SQLClientConfig.GetClientConfig(p => p.MerchantCode == MerchantCodeType.SUP).FirstOrDefault();
            if (clientConfig == null)
                return;

            merchantID = clientConfig.MerchantID;
            key = clientConfig.MerchantKey;
            getOrderurl = clientConfig.GetOrdersURL;
            notifyOrderurl = clientConfig.NotifyOrderURL;
            count = clientConfig.GetOrderCount == null ? 0 : Convert.ToInt32(clientConfig.GetOrderCount);
            time = clientConfig.GetOrderTime == null ? 0 : Convert.ToInt32(clientConfig.GetOrderTime);
        }

        public List<Order> getOrderFromSUP(ref int getOrderTime)
        {
            getOrderTime = time;

            List<Order> orderSet = new List<Order>();

            try
            {
                string sign = Md5Helper.GetMD5String_utf8(merchantID + key);
                string postData = string.Format("MerchantID={0}&Sign={1}&ProductNo={2}&Count={3}&Status={4}", merchantID,sign,"", count, "0");

                System.Net.CookieContainer cookie = new System.Net.CookieContainer();

                WriteLog.Write("方法:getOrderFromSUP 获取订单参数：" + postData, LogPathFile.Other);

                string result = PostAndGet.HttpGetString(getOrderurl, postData, ref cookie);

                WriteLog.Write("方法:getOrderFromSUP 获取订单信息：" + result, LogPathFile.Other);



                string State = Regex.Match(result, @"<State>(.*?)</State>").Groups[0].Value;

                string State_Info = Regex.Match(result, @"<State_Info>(.*?)</State_Info>").Groups[0].Value;

                if (State.Contains("0") && State_Info.Contains("成功"))
                    assignmentOrder(orderSet, result);

            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:getOrderFromSUP异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }

            return orderSet;
        }

        void assignmentOrder(List<Order> orderSet, string item)
        {
            try
            {
                List<Order> dbOrderList = new SQLOrder().GetOrder_top1000();

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(item);//加载xml
                XmlNodeList xmlList = xml.GetElementsByTagName("Order"); //取得节点名为order的XmlNode集合
                foreach (XmlNode xmlNode in xmlList)
                {
                    XmlNodeList childList = xmlNode.ChildNodes; //取得orders下的子节点集合
                    Order order = new Order();
                    foreach (XmlNode childNode in childList)
                    {
                        #region order
                        if ("OrderID".Equals(childNode.Name))
                            order.OrderExternalID = childNode.InnerText.Trim();
                        else if ("ProductNo".Equals(childNode.Name))
                            order.ProductID = childNode.InnerText.Trim();
                        else if ("ProductName".Equals(childNode.Name))
                            order.ProductName = childNode.InnerText.Trim();
                        else if ("ParValue".Equals(childNode.Name))
                            order.ProductParValue = Convert.ToDecimal(childNode.InnerText.Trim());
                        else if ("Price".Equals(childNode.Name))
                            order.ProductSalePrice = Convert.ToDecimal(childNode.InnerText.Trim());
                        else if ("ChargeAccount".Equals(childNode.Name))
                            order.TargetAccount = childNode.InnerText.Trim();
                        //else if ("RechargeType".Equals(childNode.Name))
                        //    order.TargetAccountType = childNode.InnerText.Trim();
                        //else if ("RechargeType".Equals(childNode.Name))
                        //    order.TargetAccountTypeName = childNode.InnerText.Trim();
                        else if ("BuyNumber".Equals(childNode.Name))
                            order.BuyAmount = Convert.ToInt32(childNode.InnerText.Trim());
                        else if ("TradeAmount".Equals(childNode.Name))
                            order.TotalSalePrice = Convert.ToDecimal(childNode.InnerText.Trim());
                        //else if ("GameName".Equals(childNode.Name))
                        //    order.Game = childNode.InnerText.Trim();
                        else if ("GameName".Equals(childNode.Name))
                            order.GameName = childNode.InnerText.Trim();
                          
                            //如果有角色，
                        //else if ("RoleName".Equals(childNode.Name))
                        //  

                        else if ("AreaID".Equals(childNode.Name))
                            order.Area = childNode.InnerText.Trim();
                        else if ("Area".Equals(childNode.Name))
                            order.AreaName = childNode.InnerText.Trim();
                        else if ("SrvID".Equals(childNode.Name))
                            order.Server = childNode.InnerText.Trim();
                        else if ("Srv".Equals(childNode.Name))
                            order.ServerName = childNode.InnerText.Trim();
                        //else if ("RechargeType".Equals(childNode.Name))
                        //    order.RechargeMode = childNode.InnerText.Trim();
                        else if ("RechargeType".Equals(childNode.Name))
                            order.RechargeModeName = childNode.InnerText.Trim();
                        //else if ("stock-merchant-id".Equals(childNode.Name))
                        //    order.StockMerchantId = childNode.InnerText.Trim();
                        //else if ("stock-merchant-name".Equals(childNode.Name))
                        //    order.StockMerchantName = childNode.InnerText.Trim();
                        else if ("BuyerIP".Equals(childNode.Name))
                            order.CustomerIp = childNode.InnerText.Trim();
                        //else if ("customer-region".Equals(childNode.Name))
                        //    order.CustomerRegion = childNode.InnerText.Trim();
                        //else if ("deal-date-time".Equals(childNode.Name))
                        //    order.DealDateTime = Convert.ToDateTime(childNode.InnerText.Trim());



                        #endregion
                    }
                    order.StartDatetime = DateTime.Now;
                    order.OrderInsideID = OrderHelper.GenerateId(order.StartDatetime, "01");
                    order.SuccessfulAmount = 0;
                    order.RechargeStatus = (int)OrderRechargeStatus.untreated;
                    order.IsNotify = false;
                    order.MerchantCode = MerchantCodeType.SUP;
                    if (BelongPoProduct(order))
                    {
                        if (!CheckRepeatOrder(order,dbOrderList))
                            orderSet.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:assignmentOrder_Sup异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }
        }

        public bool notigyOrderToSUP(Order order, bool isprocessing = false)
        {
            WriteLog.Write("方法:订单开始通知易约，订单号：" + order.OrderInsideID + ",订单状态："
                + order.RechargeStatus+"_"+EnumService.GetDescription((int)order.RechargeStatus), LogPathFile.Other.ToString());

            
            //充值成功=0,充值失败=1, 处理中=2,可疑订单=3
            int State = 0;
            string StateInfo = "充值成功";
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
                        State = 0;
                        if (string.IsNullOrEmpty(order.RechargeMsg))
                            StateInfo = EnumService.GetDescription(OrderRechargeStatus.successful);
                        else
                            StateInfo = order.RechargeMsg;
                        break;
                    case (int)OrderRechargeStatus.failure:
                        State = 1;
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

            string sign = Md5Helper.MD5Encrypt(merchantID + order.OrderExternalID + State + key);

            string postData = string.Format("MerchantID={0}&OrderID={1}&State={2}&StateInfo={3}&ChargeAccount={4}&ChargeMac={5}&Sign={6}", 
                merchantID, order.OrderExternalID, State, System.Web.HttpUtility.UrlEncode(StateInfo, Encoding.UTF8),"","", sign);

            System.Net.CookieContainer cookie = new System.Net.CookieContainer();

            string result = PostAndGet.HttpPostString(notifyOrderurl, postData, ref cookie);


            WriteLog.Write("方法:订单通知易约结果，订单号：" + order.OrderInsideID + ", 易约通知返回：" + result, LogPathFile.Other.ToString());

            string notifyStatus = Regex.Match(result, @"<State>(.*?)</State>").Groups[0].Value;

            if (notifyStatus.Contains("0"))
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
                List<Product> productLst = SQLProduct.GetProducts(p => p.MerchantCode == MerchantCodeType.SUP);
                if (productLst == null || productLst.Count == 0)
                    return false;

                foreach (Product item in productLst)
                {
                    if (order != null && order.ProductID != null)
                    {
                        var result = productLst.Count(p => p.ProductCode == order.ProductID && p.IsActive);
                        if (result != null && result >0)
                            return true;
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:BelongPoProduct_Sup异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
                throw;
            }
        }

       public  static bool CheckRepeatOrder(Order order, List<Order> dbOrderList)
        {
            try
            {
                if (order != null && order.OrderExternalID != null)
                {
                    bool result = true;  //重复存在;

                    if (dbOrderList != null && dbOrderList.Count > 0)
                    {
                        int count = dbOrderList.Where(p => p.OrderExternalID == order.OrderExternalID).Count();

                        if (count > 0)
                            result = true;
                        else
                            result = false;  //不存在
                    }
                    else
                    {
                        result = new SQLOrder().IsOrders(p => p.OrderExternalID == order.OrderExternalID);
                    }

                    WriteLog.Write("SUP重复订单,订单号： " + order.OrderExternalID + " ,是否有重复: " + result, LogPathFile.Other);

                    return result;

                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("方法:CheckRepeatOrder_Sup异常，信息：" + ex.Message + ex.StackTrace, LogPathFile.Exception);
            }
            return false;
        }

    }
}
