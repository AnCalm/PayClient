using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class TypeCast
    {
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public static  byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary> 
        /// 将 Stream 写入文件 
        /// </summary> 
        public static void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件 
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        /// <summary> 
        /// 从文件读取 Stream 
        /// </summary> 
        public static Stream FileToStream(string fileName)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


        /// <summary>
        /// 将 “\\u4fc4\\u7f57\\u65af” 字符串编码
        /// </summary>
        /// <param name="stringIn">传入参数串</param>
        /// <param name="stringOut">返回编码结果</param>
        /// <returns></returns>
        public static  bool GetString(string stringIn, ref string stringOut)
        {
            try
            {
                string sIn = stringIn;
                string sOut = "";
                string[] arr = sIn.Split(new string[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in arr)
                {
                    if (s.Length > 4)
                    {
                        string before = Convert.ToChar(Convert.ToInt32(s.Substring(0, 4), 16)).ToString();
                        string after = s.Substring(4);

                        sOut += before + after;
                    }
                    else
                    {
                        string alone = Convert.ToChar(Convert.ToInt32(s.Substring(0, 4), 16)).ToString();
                        sOut += alone;
                    }
                }
                stringOut = sOut;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
