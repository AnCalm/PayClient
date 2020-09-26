using ChargeInterface.AntoInterface;
using ChargeInterface.Fulu;
using ChargeInterface.SUP;
using Common;
using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.LogHelper;

namespace ChargeInterface.Charge
{
    public class PayFulu2 : ICharge
    {
        public Order Charge(Order order)
        {
            try
            {

            PayAndQueryFulu2 pay1 = new PayAndQueryFulu2();

            string result = pay1.SubmitOrder(order);

            //{
            //    "code": 0,
            //    "message": "接口调用成功",
            //    "result": "{\"area\":\"电信一区\",\"buy_num\":1,\"charge_account\":\"888888\",\"create_time\":null,\"customer_order_no\":\"201906281030191013526\",\"finish_time\":null,\"operator_serial_number\":\"\",\"order_id\":\"19062837751058701652\",\"order_price\":1.1,\"order_state\":\"untreated\",\"order_type\":4,\"product_id\":10000001,\"product_name\":\"腾讯Q币直充一元\",\"server\":\"逐鹿中原\",\"type\":\"Q币\"}",
            //    "sign": "1d158f0089b7a091fba0b5df23cd80d5"
            //}

            if (!result.Contains("{"))
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = result;
                return order;
            }

            JavaScriptObject jsonObj = (JavaScriptObject)JavaScriptConvert.DeserializeObject(result);

            string code = jsonObj["code"].ToString();
            string message = jsonObj["message"].ToString();


            if (code == "0")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.Submit;
            }
            else if (code == "1000" || code == "1001" || code == "1002" || code == "1003" || code == "1004"
            || code == "1006" || code == "1007" || code == "1008" || code == "1009" || code == "1010"
                || code == "1012" || code == "1013" || code == "1014" || code == "1015" || code == "1016"
                || code == "1017" || code == "1018" || code == "1019" || code == "2002" || code == "2003"
                || code == "3000" || code == "3001" || code == "3002" || code == "3003" || code == "3004"
                || code == "3005" || code == "3008" || code == "4000" || code == "4001" || code == "4002"|| code == "4004")
            {
                order.RechargeStatus = (int)OrderRechargeStatus.failure;
                order.RechargeMsg = message;
            }
            else
            {
                order.RechargeStatus = (int)OrderRechargeStatus.suspicious;
                order.RechargeMsg = message;

                new GetAndNotifySUPOrders().notigyOrderToSUP(order);
            }


            #region code
            //0	接口调用成功	接口调用成功，按正常流程处理；下单接口中，接口调用成功表示下单成功，但是下单成功不表示订单充值成功，要想获得订单的充值结果，需要调用查单接口来获得订单充值状态。
            //1000	必须传入API接口名称	method参数不能为空，必须传入API接口名称参数。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1001	无效的API接口名称	method接口方法名称错误，请检查。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1002	必须传入时间戳	timestamp参数不能为空，必须传入时间戳参数。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1003	时间戳格式错误	时间戳格式为：yyyy-MM-dd HH:mm:ss，请按要求传入参数。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1004	时间戳已超过有效期	接口请求服务器时间如果小于当前时间10分钟，则时间戳会超过接口请求有效期，请同步接口请求服务器时间。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1005	必须传入app_key	app_key参数不能为空，必须传入app_key参数。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1006	无效的app_key	app_key参数错误，请在福禄商户控制台->充值API2->应用配置->秘钥管理中的app_key项仔细核对。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1007	必须传入版本号	version参数不能为空，必须传入版本号，目前的版本号参数值为：2.0。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1008	版本号错误	传入的版本号参数值错误，目前的版本号参数值为：2.0。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1009	必须传入format格式	format参数不能为空，必须传入format格式参数；目前的format格式仅支持json。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1010	format格式错误	format格式参数值错误，目前的format格式仅支持json。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1011	必须传入编码格式	charset参数为空，必须传入编码格式参数。目前的charset编码格式仅支持utf-8。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1012	编码格式错误	charset编码格式参数值错误，目前的charset编码格式仅支持utf-8。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1013	必须传入签名加密类型	sign_type参数为空，必须传入签名加密类型。目前的sign_type签名加密类型仅支持md5。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1014	签名加密类型错误	sign_type签名加密类型参数值错误，目前的sign_type签名加密类型仅支持md5。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1015	必须传入签名	sign参数为空，必须传入签名。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1016	签名错误	传入的签名sign值与签名规则计算出的值比对不上。对于下单接口，调整参数后，可重新下单，也可失败订单。入参签名规则为： 1、将所有公共参数包装成对象object； 2、将object序列化为json字符串objectJson； 3、将objectJson转化为字符数组charObjectArray，然后将charObjectArray进行Array.Sort()排序； 4、将排序后的charObjectArray转化为字符串objectStr，然后在objectStr后直接拼接应用秘钥; 5、将第4步拼接了应用秘钥的字符串进行md5，最后将得到的值转化为小写字符串即得到sign签名值。
            //1017	必须传入请求参数集合	biz_content参数为空，必须传入请求参数集合。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1018	缺少必要参数	接口请求参数缺少必要参数，请仔细检查。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //1019	访问IP不在IP白名单内	接口请求服务器的外网IPv4格式IP地址没有添加到福禄商户控制台->应用配置->IP白名单中，请将接口响应信息中的IP地址添加进去（目前各接口没有校验IP白名单，可不必配置IP白名单）。对于下单接口，调整参数后，可重新下单，也可失败订单。
            //2002	无效的商户或应用	用户接口中，根据传入参数，找到了商户信息，但该商户的审核状态或认证状态不正常；下单接口中，可能存在该商户不存在、商户未认证、商户未审核、商户应用被禁用这几种情况。请联系运营处理。对于下单接口，可失败订单。
            //2003	商户或应用配置异常	福禄商户控制台->应用配置->秘钥配置中的配置信息异常，导致在各接口中没有找到商户编号信息，或下单和查单接口中没有获取到充值API2应用配置的应用编号信息。请联系运营处理。对于下单接口，可重试下单，也可失败订单。
            //3000	必须传入商品编号	获得商品信息接口中，缺少请求参数商品编号product_id。
            //3001	商品不存在或无法购买	获得商品信息接口中，根据传入参数，内部请求流程Code非0或没有找到商品信息或商品信息被标记为了删除；直充下单和卡密下单接口中，请求参数商品编号product_id值小于0；下单接口和商品接口中商品没有设置密价组或被删除；请联系运营处理。对于下单接口，可失败订单。
            //3002	商品已下架	下单接口中，商品已下架。请联系运营处理。订单可以失败处理。
            //3003	商品维护中	下单接口中，商品维护中。请联系运营处理。订单可以失败处理。
            //3004	商品在维护期内	下单接口中，商品在维护期内。请联系运营处理。订单可以失败处理。
            //3005	商品库存不足	下单接口中，商品库存不足。请联系运营处理。订单可以失败处理。
            //3008	商品类型错误	直充下单接口或卡密下单接口中，用直充下单接口下了卡密商品，或用卡密下单接口下了直充商品，就会返回商品类型错误，请使用正确类型的商品下单。订单可以失败处理。
            //4000	必须传入外部订单号	下单或查单接口中，外部订单号customer_order_no参数为空，必须传入外部订单号；对于下单接口，可重试下单，也可失败订单。
            //4001	购买数量必须大于0	直充下单接口或卡密下单接口中，购买数量buy_num参数小于0，购买数量必须大于0。对于这两个下单接口，参数调整后，可重试下单，也可失败订单。
            //4002	必须传入充值账号	非卡密下单接口中，充值账号charge_account参数为空，必须传入充值账号；对于这些接口，可重试下单，也可失败订单。
            //4004	充值账号在黑名单中	非卡密下单接口中，充值账号在黑名单中。请联系运营处理。订单可失败处理。
            //4008	添加订单失败	下单接口中，下单发生异常。请调用查单接口持续查询订单状态。如果查单返回4011(订单不存在)，并且10分钟后，还是返回4011状态，则订单可以失败。注意：不要轻易失败订单，请一定要查单来确认订单状态。
            //4009	执行下单超时，请查单确认下单结果	下单接口中，下单发生异常。请调用查单接口持续查询订单状态。如果查单返回4011(订单不存在)，并且10分钟后，还是返回4011状态，则订单可以失败。注意：不要轻易失败订单，请一定要查单来确认订单状态。
            //4010	外部订单号已存在	下单接口中，已正常下单成功，请不要重复提交订单下单。请调用查单接口持续查询订单状态，直到订单有最终充值结果。
            //4012	查询异常，请重试	用户接口、获得商品信息接口、获得商品模板接口、查单接口中，查询出现异常。请联系运营处理。查单接口中，请持续查询订单状态。如果查单返回4011(订单不存在)或4012(查询异常，请重试)，并且10分钟后，还是返回这两个状态，则订单可以失败。注意：不要轻易失败订单，请一定要查单来确认订单状态。
            //5000	系统异常	系统异常，联系运营处理。对于下单接口，请调用查单接口持续查询订单状态。如果查单返回4011(订单不存在)，并且10分钟后，还是返回4011状态，则订单可以失败。注意：不要轻易失败订单，请一定要查单来确认订单状态。
            #endregion

            }
            catch (Exception ex)
            {

                WriteLog.Write("方法:Charge，订单号：" + order.OrderInsideID + " Fulu 提交异常:" + ex.Message, LogPathFile.Exception.ToString());
            }
            return order;
        }
    }
}
