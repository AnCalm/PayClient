using ChargeInterface.AntoInterface;
using ChargeInterface.Fulu;
using Common;
using Common.LogHelper;
using EntityDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChargeInterface.Query
{
    public class QueryFulu : IQuery
    {
        public Order Query(Order order)
        {
            PayAndQueryFulu pay1 = new PayAndQueryFulu();

            string result = pay1.QueryOrder(order);

            // 直充商品：
            //{
            //    "code": 0,
            //    "message": "接口调用成功",
            //    "result": "{\"area\":\"电信一区\",\"buy_num\":1,\"cards\":null,\"charge_account\":\"888888\",\"create_time\":\"2019-07-01 17:53:32\",\"customer_order_no\":\"201906281030191013526\",\"finish_time\":\"2019-07-01 17:53:41\",\"operator_serial_number\":\"--\",\"order_id\":\"19070134869845421753\",\"order_price\":40.0,\"order_state\":\"success\",\"order_type\":4,\"product_id\":10000373,\"product_name\":\"qb5测试代充账号功能\",\"server\":\"逐鹿中原\",\"type\":\"Q币\"}",
            //    "sign": "06f351b34d9b02bc13bc62e66bdab2c8"
            //}
            //卡密商品：
            //{
            //    "code": 0,
            //    "message": "接口调用成功",
            //    "result": "{\"area\":null,\"buy_num\":2,\"cards\":[{\"CardNumber\":\"12nCp6X/nALmrvr1erxK+D4L8n/kqz/RItKWUfvZrCU=\",\"CardPwd\":\"9HeOgdv+NpLihh2+5Gm0Mj4L8n/kqz/RItKWUfvZrCU=\",\"CardDeadline\":\"2019-06-30 11:15:32\"},{\"CardNumber\":\"12nCp6X/nALmrvr1erxK+BzfvN8D1qbXOYunJrydEWA\",\"CardPwd\":\"9HeOgdv+NpLihh2+5Gm0MhzfvN8D1qbXOYunJrydEWA=\",\"CardDeadline\":\"2019-06-30 11:15:32\"}],\"charge_account\":null,\"create_time\":\"2019-07-01 17:55:38\",\"customer_order_no\":\"201906281230191013521\",\"finish_time\":\"2019-07-01 17:55:41\",\"operator_serial_number\":\"\",\"order_id\":\"19070116656844081755\",\"order_price\":8.9999,\"order_state\":\"success\",\"order_type\":3,\"product_id\":10000481,\"product_name\":\"入库出库专用卡密库存不要随便使用-jfs\",\"server\":null,\"type\":null}",
            //    "sign": "d440344a46479d3fa61883bcc2f1d983"
            //}
            JavaScriptObject jsonObj = (JavaScriptObject)JavaScriptConvert.DeserializeObject(result);

            string code = jsonObj["code"].ToString();
            string message = jsonObj["message"].ToString();

            string resultJson = jsonObj["result"].ToString();

            JavaScriptObject jsonObj2 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(resultJson);
            string order_id = jsonObj2["order_id"].ToString();
            string order_state = jsonObj2["order_state"].ToString();

            //订单状态： （success：成功，processing：处理中，failed：失败，untreated：未处理）

            switch (order_state)
            {
                case "success":
                    order.RechargeStatus = (int)OrderRechargeStatus.successful;
                    order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.successful) + "-" + order_state;
                    return order;
                case "failed":

                    order.RechargeStatus = (int)OrderRechargeStatus.failure;

                    order.RechargeMsg = EnumService.GetDescription(OrderRechargeStatus.failure) + "-" + order_state;
                    return order;

                case "processing":
                case "untreated":
                default:
                    break;
            }

            return order;
        }
    }
}
