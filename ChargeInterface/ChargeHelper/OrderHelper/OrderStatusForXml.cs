using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChargeInterface.ChargeHelper.OrderHelper
{
    public class OrderStatusForXml
    {
        public static Dictionary<string, List<OrderStatus>> DicOrderStatusXML;
        public static string GetOrderStatus(string productCode, string orderMsg)
        {
            if (DicOrderStatusXML == null || DicOrderStatusXML.Count == 0)
                getXml();

            try
            {
                foreach (var codeKey in DicOrderStatusXML.Keys)
                {
                    if (productCode.Equals(codeKey))
                    {
                        foreach (var item in DicOrderStatusXML[codeKey])
                        {
                            if (orderMsg.Equals(item.Msg))
                            {
                                return item.Status;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return "可疑";
        }

        static void getXml()
        {
            try
            {
                DicOrderStatusXML = new Dictionary<string, List<OrderStatus>>();

                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "OrderStatus.xml");    //加载Xml文件 
                XmlElement root = doc.DocumentElement;   //获取根节点 
                XmlNodeList personNodes = root.GetElementsByTagName("ProductCode"); //获取ProductCode子节点集合 
                foreach (XmlNode node in personNodes)
                {
                    string Code = ((XmlElement)node).GetAttribute("Code");   //获取Code属性值 
                    List<OrderStatus> lstStatus = new List<OrderStatus>();
                    foreach (var item in node.ChildNodes)
                    {
                        Dictionary<string, string> DicStatus = new Dictionary<string, string>();
                        string msg = ((XmlElement)item).GetAttribute("Msg");
                        string status = ((XmlElement)item).GetAttribute("OrderStatus");
                        OrderStatus orderstatus = new Model.OrderStatus()
                        {
                            Msg = msg,
                            Status = status
                        };
                        lstStatus.Add(orderstatus);
                    }
                    DicOrderStatusXML.Add(Code, lstStatus);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
