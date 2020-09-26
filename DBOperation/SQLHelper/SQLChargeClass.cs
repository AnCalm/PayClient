using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityDB;

namespace DBOperation.SQLHelper
{
    public static class SQLChargeClass
    {
        public static List<ChargeClass> GetChargeClasss(Func<ChargeClass, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    return dcl.ChargeClass.Where(seleWhere).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
