using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
    public static class SQLProduct
    {
        public static List<Product> GetProducts(Func<Product, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    return dcl.Product.Where(seleWhere).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string getChargeClassProductCode(string supProductID, string MerchantCode)
        {

            Product product = GetProducts(p => p.ProductCode == supProductID && p.MerchantCode == MerchantCode).FirstOrDefault();
            if (product != null)
                return product.ChargeClassProductCode;
            else
                return null;

            //switch (supProductID)
            //{
            //    case "14883":
            //        kamenProductID = "1258442";
            //        break;
            //    case "14884":
            //        kamenProductID = "1258444";
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
