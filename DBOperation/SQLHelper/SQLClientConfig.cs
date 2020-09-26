using EntityDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
   public class SQLClientConfig
    {
       public static List<ClientConfig> GetClientConfig(Func<ClientConfig, bool> seleWhere)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   return dcl.ClientConfig.Where(seleWhere).ToList();
               }
           }
           catch (Exception)
           {
               return null;
           }
       }
       public static bool  AddClientConfig(ClientConfig clientConfig)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   clientConfig = dcl.ClientConfig.Add(clientConfig);
                   return dcl.SaveChanges() > 0;
               }
           }
           catch (Exception)
           {
               return false ;
           }
       }
       public static bool UpdateClientConfig(ClientConfig clientConfig)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   dcl.Entry(clientConfig).State = System.Data.Entity.EntityState.Modified;
                   return dcl.SaveChanges() > 0;
               }
           }
           catch (Exception)
           {
               return false;
           }
       }
       public static bool DeleteClientConfig(ClientConfig clientConfig)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   dcl.ClientConfig.Attach(clientConfig);
                   dcl.ClientConfig.Remove(clientConfig);
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
