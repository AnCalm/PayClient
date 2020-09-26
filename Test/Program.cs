using ChargeInterface.BaiYou;
using ChargeInterface.Fulu;
using ChargeInterface.MBJ;
using ChargeInterface.Query;
using ChargeInterface.SUP;
using ChargeInterface.SW;
using ChargeInterface.XunTong;
using Common;
using DBOperation.SQLHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> ses = new List<string>();
            ses.Add("444");
            ses.Add("4434");
            ses.Add("44e4");
            ses.Add("44e4");

            List<Order> orders = new List<Order>();
            Order order12 = new Order()
            {
                OrderID = 1653631,
                 IsNotify=true 
            };
            Order order1 = new Order()
            {
                OrderID = 1653630,
                IsNotify=true 
            
            };

            orders.Add(order12);
            orders.Add(order1);

            new SQLOrder().MultiUpdateData(orders);

            CookieContainer coockie = new CookieContainer();
            string result = PostAndGet.HttpPostString("http://121.42.166.214:8099/select.aspx", "username=15071226434&type=qb&sporderid=01201904152304350000011081&buyhaoma=3258222489&sign=0a81867f2a42f8990cd6e47e0cc45a69", ref coockie);



            string AreaName = "第四十九大区(王者/天使电信)";
            string Servername = "王者一区(电信)";
            string Area = "";
            string Server = "";
            new PayAndQuerySW().getmoyuAreaAndServer("魔域", AreaName, Servername, ref Area, ref Server);

            List<Order> dbOrderList = new SQLOrder().GetOrder_top1000();

            Order order123=new Order ()
            {
                OrderExternalID = "2695468386217401"
            };

            GetAndNotifySUPOrders.CheckRepeatOrder(order123, dbOrderList);

            bool bo =new PayAndQuerySW().isChinese("ksdflhg少哦好东西 41561");

            int ssssssss = new QuerySW().getOrderStatus("upay_oidb_0x4fd verify failed for uin[166355766] userip[223.4.205.37] valid[0] reason[4]", "601");

            Order order133 = new Order()
            {
                OrderInsideID = "11",
                BuyAmount = 1,
                TargetAccount = "鄂尔多斯飞",
                //CustomerIp = "223.104.63.183"


            };

            new PayAndQueryBaiYou().SubmitOrder(order133);

            new ChargeInterface.Charge.PayFulu().Charge(order133);

            new PayAndQueryXunTong().SubmitOrder(order133);

//2018/6/24 21:27:58	方法:Kamen回调，ChargeTime：2018-06-24 21:27:55 CustomerOrderNo:01201806242127490000000008OrderNo:1375870705 ReMark:交易失败 Status:False
//2018/6/24 21:27:58	方法:Kamen回调，Kamen订单号：1375870705 本地系统订单号:01201806242127490000000008签名验证错误，Kamen数字签名69962afcc9ff56fdd4fadfbb04ea5841本地签名:D70A7255A9D91F4E867340B4288C55AF


            string md5str = "chargetime=2018-06-24 21:27:55&customerorderno=01201806242127490000000008&orderno=1375870705&remark=" + System.Web.HttpUtility.UrlEncode("交易失败", Encoding.UTF8) + "&Status=False918BB8A545017EB8B5E26D9F62AD33E1";
          string   ChargeTime="2016-02-18 11:44:06";
            string CustomerOrderNo="t0001";
            string OrderNo="653063195";
            string ReMark="交易成功";
            string Status="True";
            string Secret="F636297CB44B2F02BA4651282266EC2F";
            string sData = "chargetime=" + ChargeTime.Replace("/", "-") +"&customerorderno=" +CustomerOrderNo +"&orderno="+ OrderNo+ "&remark=" 
                + ReMark + "&status=" + Status;
            //string sign = EncryptMd5UTF8(sData+Secret);

            md5str = "chargetime=2016-02-18 11:44:06&customerorderno=t0001&orderno=653063195&remark=交易成功&status=TrueF636297CB44B2F02BA4651282266EC2F";
            //md5str = "chargetime=2014-11-12 10:31:03&customerorderno=493171&orderno=338833405&remark=交易成功&status=True+商户密钥";

            string dfdd = System.Web.HttpUtility.UrlEncode(md5str, Encoding.UTF8);
            //ec01cb60874420ab1c11083b07dd98ad

            string checkSign = Md5Helper.EncryptMd5_Kamen1(sData + Secret);


           // 01201903312106160000107708 RuiLian 提交参数:oid=01201903312106160000107708&cid=100183&pid=1078&pn=%e7%b4%ab%e5%85%89%e9%98%81-&nb=1&fm=15.0000&ru=http%3a%2f%2f116.62.44.48%2fNotifyFromRuiLian.aspx&at=&ct=%e6%96%b0%e6%b5%aa%e5%be%ae%e5%8d%9a%e4%bc%9a%e5%91%98&fr=&fs=&rin=&pip=219.140.132.224&info1=&info2=&sign=7839fe0efd29c9775f86d392f1182918

     //Kamen数字签名69962afcc9ff56fdd4fadfbb04ea5841本地签名:D70A7255A9D91F4E867340B4288C55AF



           // "oid=101447975&cid=test&pid=10097&pn=123456&nb=1&fm=1&ru=backcallurl&at=xxx&ct=xxx&fr=xxx&fs=xxx&rin=xxx&pip=118.249.190.100&info1=xxx&info2=xxx
            //&sign=c608f410759bf59d925ceb9d9c02dad0"


            string md5str1 = "101447975-test-10097-123456-1-1-backcallurl-test";
            string sign1 = Md5Helper.GetMD5String_utf8(md5str1);
            string sign2 = Md5Helper.EncryptMd5_Kamen(md5str1);
            string sign3 = Md5Helper.EncryptMd5_Kamen1(md5str1);

    Order order = new Order();
            order.BuyAmount = 10;
            order.TargetAccount = "15072412234";
            ChargeInterface.Charge.Pay_91_com pt = new ChargeInterface.Charge.Pay_91_com();
            order = pt.Charge(order);

            new QuerySW().Query(order);

            CookieContainer cookie = new CookieContainer();
            Cookie ck = new Cookie()
            {
                Name = "test",
                Value = "123",
                Domain="www"
            };
            cookie.Add(ck);

            Common.CookieOperation.CookieHelper.WriteCookiesToDisk("357440019", cookie);
          
            CookieContainer cc=  Common.CookieOperation.CookieHelper.ReadCookiesFromDisk("E:\\testcookie\\357440019");

           string ss= ChargeInterface.ChargeHelper.OrderHelper.OrderStatusForXml.GetOrderStatus("91y", "充值失败");
           Console.WriteLine(ss);
            ss = ChargeInterface.ChargeHelper.OrderHelper.OrderStatusForXml.GetOrderStatus("91y", "充值成功");

            Console.WriteLine(ss);
        }
    }
}
