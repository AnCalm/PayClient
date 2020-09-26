using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
    public class SQLChargeAccountType
    {
        public static List<ChargeAccountType> GetChargeAccountType(Func<ChargeAccountType, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    return dcl.ChargeAccountType.Where(seleWhere).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool AddChargeAccountType(ChargeAccountType chargeAccountType)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    chargeAccountType = dcl.ChargeAccountType.Add(chargeAccountType);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool UpdateChargeAccountType(ChargeAccountType chargeAccountType)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Entry(chargeAccountType).State = System.Data.Entity.EntityState.Modified;
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool DeleteChargeAccountType(ChargeAccountType chargeAccountType)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.ChargeAccountType.Attach(chargeAccountType);
                    dcl.ChargeAccountType.Remove(chargeAccountType);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
