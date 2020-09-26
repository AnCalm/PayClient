using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common.CookieOperation
{
    public static class CookieHelper
    {
        static string cookiePath = AppDomain.CurrentDomain.BaseDirectory + "\\cookie\\";

        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            if (!Directory.Exists(cookiePath))
            {
                Directory.CreateDirectory(cookiePath);
            }

            using (Stream stream = File.Create(cookiePath + "\\" + file))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                }
                catch (Exception)
                {
                }
            }
        }

        public static CookieContainer ReadCookiesFromDisk(string file)
        {
            try
            {
                using (Stream stream = File.Open(cookiePath + "\\" + file, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return new CookieContainer();
            }
        }
    }
}
