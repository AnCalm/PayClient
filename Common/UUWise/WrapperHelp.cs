using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace Common.UUWise
{
    public static class WrapperHelp
    {
        public static string strSoftID = "111315";
        public static string softKey = "e10edfe1b8ab4034b4421a05321f18a9";
        public static string strCheckKey = "D290CA20-E33F-4C6A-BDB6-64F1E6214813";

        /// <summary>
        /// 检验dll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void checkDll()
        {
            //int softId = int.Parse(strSoftID);
            //Guid guid = Guid.NewGuid();
            //string strGuid = guid.ToString().Replace("-", "").Substring(0, 32).ToUpper();
            //string DLLPath = System.Environment.CurrentDirectory + "\\UUWiseHelper.dll";
            ////string DLLPath = "E:\\work\\UUWiseHelper 新版http协议\\输出目录\\UUWiseHelper.dll";
            //string strDllMd5 = GetFileMD5(DLLPath);
            //CRC32 objCrc32 = new CRC32();
            //string strDllCrc = String.Format("{0:X}", objCrc32.FileCRC(DLLPath));
            ////CRC不足8位，则前面补0，补足8位
            //int crcLen = strDllCrc.Length;
            //if (crcLen < 8)
            //{
            //    int miss = 8 - crcLen;
            //    for (int i = 0; i < miss; ++i)
            //    {
            //        strDllCrc = "0" + strDllCrc;
            //    }
            //}
            ////下面是软件id对应的dll校验key。在开发者后台-我的软件里面可以查的到。

            //string yuanshiInfo = strSoftID + strCheckKey + strGuid + strDllMd5.ToUpper() + strDllCrc.ToUpper();
            //richTextBox1.Text += yuanshiInfo + "\n";
            //string localInfo = MD5Encoding(yuanshiInfo);
            //StringBuilder checkResult = new StringBuilder();
            //Wrapper.uu_CheckApiSign(softId, softKey, strGuid, strDllMd5, strDllCrc, checkResult);
            //string strCheckResult = checkResult.ToString();
            //if (localInfo.Equals(strCheckResult))
            //    richTextBox1.Text += "Dll校验成功！\n";
            //else
            //    richTextBox1.Text += "Dll校验失败！服务器返回信息为" + strCheckResult + "本地校验信息为" + localInfo + "\n";
        }

        /// <summary>
        /// 登陆账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Login_UU()
        {
            try
            {
                /*	优优云DLL 文件MD5值校验
                 *  用处：近期有不法份子采用替换优优云官方dll文件的方式，极大的破坏了开发者的利益
                 *  用户使用替换过的DLL打码，导致开发者分成变成别人的，利益受损，
                 *  所以建议所有开发者在软件里面增加校验官方MD5值的函数
                 *  如何获取文件的MD5值，通过下面的GetFileMD5(文件)函数即返回文件MD5
                 */

                string DLLPath = System.Environment.CurrentDirectory + "\\UUWiseHelper.dll";
                //string Md5 = GetFileMD5(DLLPath);
                //string AuthMD5 = "79dd7e248b7ec70e2ececa19b51c39c6";//作者在编写软件时内置的比对用DLLMD5值，不一致时将禁止登录,具体需要各位自己先获取使用的DLL的MD5验证字符串
                // if (Md5 != AuthMD5)
                //{
                //    MessageBox.Show("此软件使用的是UU云1.1.0.9动态链接库版DLL，与您目前软件内DLL版本不符，请前往http://www.uuwise.com下载更换此版本DLL");
                //     return;
                // }

                string u = "qq80662883";
                string p = "5350a6fd";
                int res = Wrapper.uu_login(u, p);
                if (res > 0)
                {
                    //登陆成功
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 设置软件信息
        /// </summary>
        public static void SetSoftInfo()
        {
            try
            {
                int softId = int.Parse(strSoftID);
                Wrapper.uu_setSoftInfo(softId, softKey);
            }
            catch (Exception ex)
            {
            }
        }

     
        /// <summary>
        /// 验证码识别
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <param name="codeId"></param>
        /// <returns></returns>
        public static bool GetCodeByByte_UU(string url,ref  CookieContainer cookie, int codeType, ref string code, ref int codeId)
        {
            try
            {
                #region 获取验证码

                Image image = ByteHelper.byteArrayToImage(PostAndGet.HttpGetByte(url, "", ref cookie));

                #endregion

                #region 保存图片
                //判断保存图片的文件夹是否存在，若不存在则创建
                //string filepath = System.Windows.Forms.Application.StartupPath + @"\37";

                //if (!Directory.Exists(filepath))
                //{
                //    Directory.CreateDirectory(filepath);
                //}

                //Bitmap bmp1 = new Bitmap(image);
                //MemoryStream bmpStream1 = new MemoryStream();
                //bmp1.Save(bmpStream1, System.Drawing.Imaging.ImageFormat.Jpeg);
                //string BmpPath = filepath + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
                //FileStream fs = new FileStream(BmpPath, FileMode.Create);
                //bmpStream1.WriteTo(fs);
                //bmpStream1.Close();
                //fs.Close();
                //bmpStream1.Dispose();
                //fs.Dispose();
                #endregion

                #region 把图片转换为字节流
                //判断保存图片的文件夹是否存在，若不存在则创建

                Bitmap bmp = new Bitmap(image);
                MemoryStream bmpStream = new MemoryStream();

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Flush();

                #endregion

                #region 调用优优云打码
                StringBuilder strResult = new StringBuilder(50);
                codeId = Wrapper.uu_recognizeByCodeTypeAndBytes(buffer, buffer.Length, codeType, strResult);
                code = CheckResult(strResult.ToString(), Convert.ToInt32(strSoftID), codeId, strCheckKey);

                #endregion

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="picContent">验证码字节数组</param>
        /// <param name="codeLength">验证码字符长度</param>
        /// <param name="codeType">验证码类型</param>
        /// <param name="codeId">验证码识别ID</param>
        public static bool GetCodebyUU(byte[] picContent, int codeLength, int codeType, ref string code, ref int codeId)
        {
            try
            {
                StringBuilder result = new StringBuilder(50);
                codeId = Wrapper.uu_recognizeByCodeTypeAndBytes(picContent, codeLength, codeType, result);
                code = CheckResult(result.ToString(), Convert.ToInt32(strSoftID), codeId, strCheckKey);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 报告错误
        /// </summary>
        /// <param name="CodeID"></param>
        /// <returns></returns>
        public static int reportError(int CodeID)
        {
            try
            {
                int report = Wrapper.uu_reportError(CodeID);
                return report;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string CheckResult(string result, int softId, int codeId, string checkKey)
        {
            //对验证码结果进行校验，防止dll被替换
            if (string.IsNullOrEmpty(result))
                return result;
            else
            {
                if (result[0] == '-')
                    //服务器返回的是错误代码
                    return result;

                string[] modelReult = result.Split('_');
                //解析出服务器返回的校验结果
                string strServerKey = modelReult[0];
                string strCodeResult = modelReult[1];
                //本地计算校验结果
                string localInfo = softId.ToString() + checkKey + codeId.ToString() + strCodeResult.ToUpper();
                string strLocalKey = MD5Encoding(localInfo).ToUpper();
                //相等则校验通过
                if (strServerKey.Equals(strLocalKey))
                    return strCodeResult;
                return "结果校验不正确";
            }
        }

        /// <summary>
        /// MD5 加密字符串
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encoding(string rawPass)
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

        #region 根据路径获取文件MD5
        /// <summary>
        /// 获取文件MD5校验值
        /// </summary>
        /// <param name="filePath">校验文件路径</param>
        /// <returns>MD5校验字符串</returns>
        private static string GetFileMD5(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5byte = md5.ComputeHash(fs);
            int i, j;
            StringBuilder sb = new StringBuilder(16);
            foreach (byte b in md5byte)
            {
                i = Convert.ToInt32(b);
                j = i >> 4;
                sb.Append(Convert.ToString(j, 16));
                j = ((i << 4) & 0x00ff) >> 4;
                sb.Append(Convert.ToString(j, 16));
            }
            return sb.ToString();
        }
        #endregion
    }
}
