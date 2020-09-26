using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
   public class SQLProductInfo
    {
      static List<ProductInfo> ProductInfos = new List<ProductInfo>();
       public static List<ProductInfo> GetProductInfos(Func<ProductInfo, bool> seleWhere)
       {
           try
           {
               using (DCLEntities dcl = new DCLEntities())
               {
                   return dcl.ProductInfo.Where(seleWhere).ToList();
               }
           }
           catch (Exception)
           {
               return null;
           }
       }

       public static List<ProductInfo> GetProductInfosByProductID(int productID)
       {
           try
           {
               List<ProductInfo> lst = new List<ProductInfo>();

               if (ProductInfos==null || ProductInfos.Count<=0)
               {
                   ProductInfos=GetProductInfos(n => n.ProductInfoID > 0);
               }

               if (ProductInfos!=null)
               {
                   lst=ProductInfos.Where(n => n.ProductID == productID).ToList();
               }

               return lst;
           }
           catch (Exception)
           {
               return null;
           }
       }
    }
}
