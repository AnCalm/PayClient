using EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
   public  class SQLOrderChargeAccount
    {
       public static OrderChargeAccount GetChargeAccount(string typeCode, bool isReadonly = true)
       {
           try
           {
               int count = 0;
               do
               {
                   if (count > 10) break;
                   OrderChargeAccount account = GetChargeAccount_i(typeCode, isReadonly);
                   if (account != null)
                       return account;
                   count++;
                   System.Threading.Thread.Sleep(5 * 1000);
               } while (true);
           }
           catch (Exception)
           {
               throw;
           }

           return null;
       }
       public static OrderChargeAccount GetChargeAccount_i(string typeCode,bool isReadonly )
       {
           try
           {
               ChargeAccountType AccountType = SQLChargeAccountType.GetChargeAccountType(p => p.Code == typeCode).FirstOrDefault();

               OrderChargeAccount orderChargeAccount = GetOrderChargeAccount(p => p.ChargeAccountTypeID == AccountType.ChargeAccountTypeID
                   && p.IsAvailable == true && p.IsUsing == false).OrderBy(p=>p.UseTimes).FirstOrDefault();
               if (orderChargeAccount == null)
                   return null;
               if (isReadonly)
               {
                   orderChargeAccount.IsUsing = true;
                   if (UpdateOrderChargeAccount(orderChargeAccount))
                       return orderChargeAccount;
               }
               else
                   return orderChargeAccount;
           }
           catch (Exception)
           {

           }
           return null;
       }

       public static bool UpdateChargeAccount(OrderChargeAccount orderChargeAccount, bool isUsing, bool IsAvailable = true)
       {
           try
           {
               orderChargeAccount.LastUseTime = DateTime.Now;
               if (orderChargeAccount.UseTimes == null)
                   orderChargeAccount.UseTimes = 0;
               orderChargeAccount.UseTimes++;
               orderChargeAccount.IsUsing = isUsing;
               orderChargeAccount.IsAvailable = IsAvailable;
               return UpdateOrderChargeAccount(orderChargeAccount);
           }
           catch (Exception)
           {
               return false;
           }
       }


       public static List<OrderChargeAccount> GetOrderChargeAccount(Func<OrderChargeAccount, bool> seleWhere)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   return dcl.OrderChargeAccount.Where(seleWhere).ToList();
               }
           }
           catch (Exception)
           {
               return null;
           }
       }
       public static bool AddOrderChargeAccount(OrderChargeAccount orderChargeAccount)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   orderChargeAccount = dcl.OrderChargeAccount.Add(orderChargeAccount);
                   return dcl.SaveChanges() > 0;
               }
           }
           catch (Exception)
           {
               return false;
           }
       }
       public static bool UpdateOrderChargeAccount(OrderChargeAccount orderChargeAccount)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   dcl.Entry(orderChargeAccount).State = System.Data.Entity.EntityState.Modified;
                   return dcl.SaveChanges() > 0;
               }
           }
           catch (Exception)
           {
               return false;
           }
       }
       public static bool DeleteOrderChargeAccount(OrderChargeAccount orderChargeAccount)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   dcl.OrderChargeAccount.Attach(orderChargeAccount);
                   dcl.OrderChargeAccount.Remove(orderChargeAccount);
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
