using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class CheckStrHelper
    {
        public static bool IsChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
            //if (Regex.IsMatch(str, @"[\u4e00-\u9fbb]+$"))
            //    return true;
            //else
            //    return false;
        }
    }
}
