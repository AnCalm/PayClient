using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.LogHelper
{
    public class Log:IDisposable
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string file=LogPathFile.Other;
        LogType type = LogType.Hourly;
        public Log() { }
        public Log(string fileStr)
        {
            file = fileStr;
        }
        public Log(string fileStr, LogType logType)
        {
            file = fileStr;
            type = logType;
        }

        public void Write(string msg)
        {
            string fullPath = path + "\\Log\\" + file;
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            StreamWriter writer = new StreamWriter(fullPath + "\\" + GetFilename(), true, Encoding.UTF8);

            writer.Write(DateTime.Now.ToString());
            writer.Write('\t');
            writer.WriteLine(msg);
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        private string GetFilename()
        {
            DateTime now = DateTime.Now;
            string format = "";
            switch (type)
            {
                case LogType.Hourly:
                    format = "yyyyMMddHH'.log'";
                    break;
                case LogType.Daily:
                    format = "yyyyMMdd'.log'";
                    break;
                case LogType.Weekly:
                    format = "yyyyMMdd'.log'";
                    break;
                case LogType.Monthly:
                    format = "yyyyMM'.log'";
                    break;
                case LogType.Annually:
                    format = "yyyy'.log'";
                    break;
            }
            return now.ToString(format);
        }

        public void Dispose()
        {
            
        }
    }

    public class WriteLog
    {
        public static void Write(string msg, string pathFile)
        {
            try
            {
                using (Log log = new Log(pathFile, LogType.Hourly))
                {
                    log.Write(msg);
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("WriteLog: 异常信息：" + ex.Message, LogPathFile.FileEx.ToString(),LogType.Monthly);
            }
        }

        public static void Write(string msg, string pathFile, LogType logType)
        {
            try
            {
                using (Log log = new Log(pathFile, logType))
                {
                    log.Write(msg);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
 




        
                    
               
