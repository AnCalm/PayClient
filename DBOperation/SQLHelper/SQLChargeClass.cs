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


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        public static void UpdateChargeClasssList(List<ChargeClass> list)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    foreach (var item in list)
                    {
                        dcl.ChargeClass.Attach(item);
                        var entry = dcl.Entry(item);
                        entry.Property("IsUsed").IsModified = true;//指明用户名这个字段是被修改的
                    }
                    dcl.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
