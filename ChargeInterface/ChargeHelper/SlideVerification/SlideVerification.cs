using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeInterface.ChargeHelper.SlideVerification
{
    public static class SlideVerification
    {
        public static void VerificationFor91y(System.Net.CookieContainer cookie)
        {
            string url = "http://123.206.33.187:9000/crack/aLiYun";
            string key = "FFFF00000000016860BF";
            string scene = "login";
            //FFFF00000000016860BF:1511432977354:0.8646665122863599
            string t = key +":"+ GetTimeLikeJS() + ":0.8646665122863599";
            string userid = "469FB388E2844B90A55E44D99852DE67";

            string data=key+"&"+scene+"&"+t+"&"+userid;
            string result = PostAndGet.HttpGetString(url, data, ref cookie);
        }

        public static long GetTimeLikeJS()
        {
            long lLeft = 621355968000000000;
            DateTime dt = DateTime.Now;
            long Sticks = (dt.Ticks - lLeft) / 10000;
            return Sticks;
        }
    }
}
