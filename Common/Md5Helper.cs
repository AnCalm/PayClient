using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Md5Helper
    {
        /// <summary>
        /// MD5 加密字符串
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encrypt(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetMD5String_utf8(string str)
        {

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(str));

            StringBuilder sb = new StringBuilder(32);

            for (int i = 0; i < t.Length; i++)
            {

                sb.Append(t[i].ToString("x").PadLeft(2, '0'));

            }

            return sb.ToString();

        }

        public static string GetMD5String_Default(string str)
        {

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] t = md5.ComputeHash(Encoding.Default.GetBytes(str));

            StringBuilder sb = new StringBuilder(32);

            for (int i = 0; i < t.Length; i++)
            {

                sb.Append(t[i].ToString("x").PadLeft(2, '0'));

            }

            return sb.ToString();

        }

        public static string EncryptMd5_Kamen(string encryptStr)
        {
            byte[] result = Encoding.Default.GetBytes(encryptStr.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToUpper();
        }

        public static string EncryptMd5_Kamen1(string encryptStr)
        {
            byte[] result = Encoding.GetEncoding("utf-8").GetBytes(encryptStr.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }


        public static string Md5_Fulu(string str)
        {
            Encoding encode = Encoding.UTF8;
            var cl = str;
            var md5 = MD5.Create();
            var s = md5.ComputeHash(encode.GetBytes(cl));
            return s.Aggregate("", (current, t) => current + t.ToString("x2"));
        }
    }
}
