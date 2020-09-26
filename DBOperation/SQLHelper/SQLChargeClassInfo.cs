using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
   public class SQLChargeClassInfo
    {
       public static List<ChargeClassInfo> ChargeClassInfos = new List<ChargeClassInfo>();
       public static List<ChargeClassInfo> GetProductInfos(Func<ChargeClassInfo, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    return dcl.ChargeClassInfo.Where(seleWhere).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
