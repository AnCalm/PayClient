using Common;
using Common.LogHelper;
using EntityDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
    public class SQLOrder
    {
        public List<Order> GetOrder(Func<Order, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    List<Order> order = dcl.Order.Where(seleWhere).ToList();
                    return order;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[GetOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return null;
            }
        }


        public List<Order> GetOrder_top1000()
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    List<Order> orderExternalIDlist = dcl.Order.AsNoTracking().OrderByDescending(p => p.OrderID)
                        .Select(p => new { p.OrderExternalID }).Take(1000).ToList().Select(a => new Order { OrderExternalID = a.OrderExternalID }).ToList();

                    return orderExternalIDlist;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[GetOrder_top1000]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return null;
            }
        }

        public bool IsOrders(Func<Order, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    int count = dcl.Order.AsNoTracking().Where(seleWhere).Count();

                    return count > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[IsOrders]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return true;
            }
        }

        public static List<Order> GetBySql()
        {
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;


                List<Order> listOrder = new List<Order>();
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string selectSql = "select top 10 * from [dbo].[Order] where RechargeStatus=0";
                    SqlCommand com = new SqlCommand(selectSql, conn);

                    SqlDataReader dataReader = com.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Order order = new Order();
                        if (!string.IsNullOrEmpty(dataReader["OrderID"].ToString()))
                            order.OrderID = Convert.ToInt32(dataReader["OrderID"]);
                        order.OrderInsideID = dataReader["OrderInsideID"].ToString();
                        order.OrderExternalID = dataReader["OrderExternalID"].ToString();
                        order.ProductID = dataReader["ProductID"].ToString();
                        order.ProductName = dataReader["ProductName"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["ProductParValue"].ToString()))
                            order.ProductParValue = Convert.ToDecimal(dataReader["ProductParValue"]);
                        if (!string.IsNullOrEmpty(dataReader["ProductSalePrice"].ToString()))
                            order.ProductSalePrice = Convert.ToDecimal(dataReader["ProductSalePrice"]);
                        order.TargetAccount = dataReader["TargetAccount"].ToString();
                        order.TargetAccountType = dataReader["TargetAccountType"].ToString();
                        order.TargetAccountTypeName = dataReader["TargetAccountTypeName"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["BuyAmount"].ToString()))
                            order.BuyAmount = Convert.ToInt32(dataReader["BuyAmount"]);
                        if (!string.IsNullOrEmpty(dataReader["TotalSalePrice"].ToString()))
                            order.TotalSalePrice = Convert.ToDecimal(dataReader["TotalSalePrice"]);
                        order.Game = dataReader["Game"].ToString();
                        order.GameName = dataReader["GameName"].ToString();
                        order.Area = dataReader["Area"].ToString();
                        order.AreaName = dataReader["AreaName"].ToString();
                        order.Server = dataReader["Server"].ToString();
                        order.ServerName = dataReader["ServerName"].ToString();
                        order.RechargeMode = dataReader["RechargeMode"].ToString();
                        order.RechargeModeName = dataReader["RechargeModeName"].ToString();
                        order.StockMerchantId = dataReader["StockMerchantId"].ToString();
                        order.StockMerchantName = dataReader["StockMerchantName"].ToString();
                        order.CustomerIp = dataReader["CustomerIp"].ToString();
                        order.CustomerRegion = dataReader["CustomerRegion"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["DealDateTime"].ToString()))
                            order.DealDateTime = Convert.ToDateTime(dataReader["DealDateTime"]);
                        if (!string.IsNullOrEmpty(dataReader["StartDatetime"].ToString()))
                            order.StartDatetime = Convert.ToDateTime(dataReader["StartDatetime"]);
                        var ss = dataReader["EndDatetime"];
                        if (!string.IsNullOrEmpty(dataReader["EndDatetime"].ToString()))
                            order.EndDatetime = Convert.ToDateTime(dataReader["EndDatetime"]);
                        if (!string.IsNullOrEmpty(dataReader["RechargeStatus"].ToString()))
                            order.RechargeStatus = Convert.ToInt32(dataReader["RechargeStatus"]);
                        if (!string.IsNullOrEmpty(dataReader["SuccessfulAmount"].ToString()))
                            order.SuccessfulAmount = Convert.ToDecimal(dataReader["SuccessfulAmount"]);
                        order.ChargeAccountInfo = dataReader["ChargeAccountInfo"].ToString();
                        order.RechargeMsg = dataReader["RechargeMsg"].ToString();
                        order.IsNotify = Convert.ToBoolean(dataReader["IsNotify"]);
                        order.MerchantCode = dataReader["MerchantCode"].ToString();

                        listOrder.Add(order);
                    }
                    conn.Close();
                }




                return listOrder;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[GetBySql]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return null;
            }
        }
        public List<Order> GetNotNotifyOrderBySql(string sort, List<int> chargeClassIDList)
        {
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;


                List<Order> listOrder = new List<Order>();
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    //string selectSql = "select top 20 * from [dbo].[Order] where IsNotify=0 and RechargeStatus >1 and ProductID=" + productID + " order By StartDatetime " + sort;
                
                    StringBuilder strsql=new StringBuilder ();
                    strsql.Append("select top 30 * from [dbo].[Order] ");
                    strsql.Append("left join Product on [dbo].[Order].productID= Product.ProductCode ");
                    strsql.Append("where [dbo].[Order].IsNotify=0 and [dbo].[Order].RechargeStatus >1 and Product.ChargeClassID in (");
                   
                    int num=0;
                    foreach (int chargeClassID in chargeClassIDList)
                    {
                        if (num == 0)
                            strsql.Append(chargeClassID);
                        else
                            strsql.Append("," + chargeClassID);

                        num++;
                    }

                    strsql.Append(")");

                    strsql.Append(" order By StartDatetime " + sort);

                    SqlCommand com = new SqlCommand(strsql.ToString(), conn);

                    SqlDataReader dataReader = com.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Order order = new Order();
                        if (!string.IsNullOrEmpty(dataReader["OrderID"].ToString()))
                            order.OrderID = Convert.ToInt32(dataReader["OrderID"]);
                        order.OrderInsideID = dataReader["OrderInsideID"].ToString();
                        order.OrderExternalID = dataReader["OrderExternalID"].ToString();
                        order.ProductID = dataReader["ProductID"].ToString();
                        order.ProductName = dataReader["ProductName"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["ProductParValue"].ToString()))
                            order.ProductParValue = Convert.ToDecimal(dataReader["ProductParValue"]);
                        if (!string.IsNullOrEmpty(dataReader["ProductSalePrice"].ToString()))
                            order.ProductSalePrice = Convert.ToDecimal(dataReader["ProductSalePrice"]);
                        order.TargetAccount = dataReader["TargetAccount"].ToString();
                        order.TargetAccountType = dataReader["TargetAccountType"].ToString();
                        order.TargetAccountTypeName = dataReader["TargetAccountTypeName"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["BuyAmount"].ToString()))
                            order.BuyAmount = Convert.ToInt32(dataReader["BuyAmount"]);
                        if (!string.IsNullOrEmpty(dataReader["TotalSalePrice"].ToString()))
                            order.TotalSalePrice = Convert.ToDecimal(dataReader["TotalSalePrice"]);
                        order.Game = dataReader["Game"].ToString();
                        order.GameName = dataReader["GameName"].ToString();
                        order.Area = dataReader["Area"].ToString();
                        order.AreaName = dataReader["AreaName"].ToString();
                        order.Server = dataReader["Server"].ToString();
                        order.ServerName = dataReader["ServerName"].ToString();
                        order.RechargeMode = dataReader["RechargeMode"].ToString();
                        order.RechargeModeName = dataReader["RechargeModeName"].ToString();
                        order.StockMerchantId = dataReader["StockMerchantId"].ToString();
                        order.StockMerchantName = dataReader["StockMerchantName"].ToString();
                        order.CustomerIp = dataReader["CustomerIp"].ToString();
                        order.CustomerRegion = dataReader["CustomerRegion"].ToString();
                        if (!string.IsNullOrEmpty(dataReader["DealDateTime"].ToString()))
                            order.DealDateTime = Convert.ToDateTime(dataReader["DealDateTime"]);
                        if (!string.IsNullOrEmpty(dataReader["StartDatetime"].ToString()))
                            order.StartDatetime = Convert.ToDateTime(dataReader["StartDatetime"]);
                        var ss = dataReader["EndDatetime"];
                        if (!string.IsNullOrEmpty(dataReader["EndDatetime"].ToString()))
                            order.EndDatetime = Convert.ToDateTime(dataReader["EndDatetime"]);
                        if (!string.IsNullOrEmpty(dataReader["RechargeStatus"].ToString()))
                            order.RechargeStatus = Convert.ToInt32(dataReader["RechargeStatus"]);
                        if (!string.IsNullOrEmpty(dataReader["SuccessfulAmount"].ToString()))
                            order.SuccessfulAmount = Convert.ToDecimal(dataReader["SuccessfulAmount"]);
                        order.ChargeAccountInfo = dataReader["ChargeAccountInfo"].ToString();
                        order.RechargeMsg = dataReader["RechargeMsg"].ToString();
                        order.IsNotify =Convert.ToBoolean( dataReader["IsNotify"]);
                        order.MerchantCode = dataReader["MerchantCode"].ToString();
                        listOrder.Add(order);
                    }
                    conn.Close();
                }




                return listOrder;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[GetBySql]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return null;
            }
        }
        public static bool UpdateBySql(int orderID, int? Status)
        {
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;


                bool result = true;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string selectSql = "update [dbo].[Order] set RechargeStatus='" + Status + "' where  OrderID=" + orderID;
                    SqlCommand com = new SqlCommand(selectSql, conn);

                    result = com.ExecuteNonQuery() > 0;

                    conn.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[UpdateBySql]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }
        }

        public bool UpdateNotifyOrderBySql(string OrderInsideID, int RechargeStatus, string RechargeMsg)
        {
            try
            {
                bool state = UpdateNotifyOrderBySql1(OrderInsideID, RechargeStatus, RechargeMsg);

                int num = 0;
                while (state == false)
                {
                    state = UpdateNotifyOrderBySql1(OrderInsideID, RechargeStatus, RechargeMsg);

                    System.Threading.Thread.Sleep(500);
                    num++;

                    if (num > 10)
                        break;
                }

                return state;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[UpdateNotifyOrderBySql]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }
        }

        public bool UpdateNotifyOrderBySql1(string OrderInsideID, int RechargeStatus, string RechargeMsg)
        {
            try
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;


                bool result = true;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string selectSql = "update [dbo].[Order] set RechargeStatus='" + RechargeStatus + "', RechargeMsg='" + RechargeMsg + "' where  OrderInsideID=" + OrderInsideID;
                    SqlCommand com = new SqlCommand(selectSql, conn);

                    result = com.ExecuteNonQuery() > 0;

                    conn.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[UpdateNotifyOrderBySql]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }
        }

        public List<Order> GetOrderToCharge(Func<Order, bool> seleWhere)
        {
            try
            {
                List<Order> order = new List<Order>();
                using (DCLEntities dcl = new DCLEntities())
                {
                    foreach (Order item in dcl.Order.Where(seleWhere).ToList())
                    {
                        item.RechargeStatus = (int)OrderRechargeStatus.processing;
                        dcl.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        if (dcl.SaveChanges() > 0)
                            order.Add(item);
                    }
                }
                return order;
            }
            catch (Exception ex)
            {
                WriteLog.Write("异常场信息[GetOrderToCharge]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return null;
            }
        }
        public static List<Order> GetOrder(string orderInsideID, string orderExternalID, string productID, string productName, string targetAccount,string reChargeAccount,DateTime startTime, DateTime endTime, int Status = -1)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    var result = dcl.Order.Where(p => p.OrderInsideID.Contains(orderInsideID) && p.OrderExternalID.Contains(orderExternalID)
                        && p.ProductID.Contains(productID) && p.ProductName.Contains(productName) && p.TargetAccount.Contains(targetAccount)
                        && p.ChargeAccountInfo.Contains(reChargeAccount) && p.StartDatetime > startTime && p.StartDatetime < endTime);
                    if (Status != -1)
                        result = result.Where(p => (p.RechargeStatus != null && p.RechargeStatus == Status));

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("系统订单号:" + orderInsideID + ",商户订单号：" + orderExternalID + ",异常场信息[GetOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);

                return null;
            }
        }
        public static bool AddOrder(Order order)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    order = dcl.Order.Add(order);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("系统订单号:" + order.OrderInsideID + ",商户订单号：" + order.OrderExternalID + ",异常场信息[AddOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }

        }

        public static bool AddOrder(List<Order> orderList)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    foreach (var item in orderList)
                    {
                        dcl.Order.Add(item);
                    }
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("批量插入订单,异常场信息[AddOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }

        }
        public bool UpdateOrder(Order order)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                WriteLog.Write("系统订单号:" + order.OrderInsideID + ",商户订单号：" + order.OrderExternalID + ",异常场信息[UpdateOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);

                return false;
            }
        }

        /// <summary>
        /// 批量修改order
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Columns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool MultiUpdateData(List<Order> orders)
        {
            try
            {
                //BatchUpdate(orders);

                List<int> ids = orders.Select(n => n.OrderID).ToList();

                string idStr = "";

                foreach (var item in ids)
                {
                    if (string.IsNullOrEmpty(idStr))
                    {
                        idStr=item.ToString();
                    }
                    else
                    {
                        idStr = idStr + "," + item.ToString();
                    }
                }

                //查询要更新的数据
                string sql = string.Format("select * from [dbo].[Order] where [OrderID] in ({0})", idStr);

                DataTable dt = DataTableHelper.ToDataTable1<Order>(orders);

                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlDataAdapter myDataAdapter = new SqlDataAdapter();
                    myDataAdapter.SelectCommand = new SqlCommand(sql, conn);
                    SqlCommandBuilder custCB = new SqlCommandBuilder(myDataAdapter);
                    custCB.ConflictOption = ConflictOption.OverwriteChanges;
                    custCB.SetAllValues = true;

                    dt.AcceptChanges();

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState == DataRowState.Unchanged)
                            dr.SetModified();
                    }
                    myDataAdapter.Update(dt);
                    dt.AcceptChanges();
                    myDataAdapter.Dispose();

                    conn.Close();
                }
            
            }
            catch (Exception ex)
            {
                WriteLog.Write("批量修改订单,异常场信息[MultiUpdateData]：" + ex.Message + ex.Source, LogPathFile.Exception);
                return false;
            }

            return true;
        }

        public static bool DeleteOrder(Order order)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Order.Attach(order);
                    dcl.Order.Remove(order);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Write("系统订单号:" + order.OrderInsideID + ",商户订单号：" + order.OrderExternalID + ",异常场信息[DeleteOrder]：" + ex.Message + ex.Source, LogPathFile.Exception);

                return false;
            }
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="columns">columns是要插入的列表</param>
        /// <param name="tableName">tableName是要插入的表名</param>
        /// <returns></returns>
        public static bool MultUpdata(DataTable dt, string columns, string tableName)
        {
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = string.Format("select {0} from {1} where OrderID=0", columns, tableName);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = new SqlCommand(sql, conn);
                        SqlCommandBuilder scb = new SqlCommandBuilder(adapter);
                        scb.ConflictOption = ConflictOption.OverwriteChanges;
                        scb.SetAllValues = true;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState == DataRowState.Unchanged)
                            {
                                dr.SetModified();
                            }
                            adapter.Update(dt);
                            dt.AcceptChanges();
                            adapter.Dispose();
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        conn.Close();
                        return false;
                    }
                }

            }
            return true;
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="list"></param>
        public void BatchUpdate(List<Order> list)
        {
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DCL"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    string SQLStr="select top 100 OrderID,DealDateTime,StartDatetime,EndDatetime,RechargeStatus,SuccessfulAmount,ChargeAccountInfo,RechargeMsg,IsNotify from [dbo].[Order]";
                    da.SelectCommand = new SqlCommand(SQLStr, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("update [dbo].[Order] set ");
                    sb.Append("DealDateTime=@DealDateTime , ");
                    sb.Append("StartDatetime=@StartDatetime , ");
                    sb.Append("EndDatetime=@EndDatetime , ");
                    sb.Append("RechargeStatus=@RechargeStatus , ");
                    sb.Append("SuccessfulAmount=@SuccessfulAmount , ");
                    sb.Append("ChargeAccountInfo=@ChargeAccountInfo , ");
                    sb.Append("RechargeMsg=@RechargeMsg , ");
                    sb.Append("IsNotify=@IsNotify ");
                    sb.Append("where " );
                    sb.Append("OrderID = @OrderID");

                    da.UpdateCommand = new SqlCommand(sb.ToString(), conn);
                    da.UpdateCommand.Parameters.Add("@DealDateTime", SqlDbType.DateTime, 8, "DealDateTime");
                    da.UpdateCommand.Parameters.Add("@StartDatetime", SqlDbType.DateTime, 8, "StartDatetime");
                    da.UpdateCommand.Parameters.Add("@EndDatetime", SqlDbType.DateTime, 8, "EndDatetime");
                    da.UpdateCommand.Parameters.Add("@RechargeStatus", SqlDbType.Int, 4, "RechargeStatus");
                    da.UpdateCommand.Parameters.Add("@SuccessfulAmount", SqlDbType.Decimal, 8, "SuccessfulAmount");
                    da.UpdateCommand.Parameters.Add("@ChargeAccountInfo", SqlDbType.VarChar, 250, "ChargeAccountInfo");
                    da.UpdateCommand.Parameters.Add("@RechargeMsg", SqlDbType.VarChar, 250, "RechargeMsg");
                    da.UpdateCommand.Parameters.Add("@IsNotify", SqlDbType.Bit, 1, "IsNotify");
                    da.UpdateCommand.Parameters.Add("@OrderID", SqlDbType.Int, 4, "OrderID");
                    da.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
                    da.UpdateBatchSize = 0;

                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++, i++)
                        {
                            ds.Tables[0].Rows[j].BeginEdit();
                            ds.Tables[0].Rows[j]["DealDateTime"] = list[i].DealDateTime ?? DateTime.Now;
                            ds.Tables[0].Rows[j]["StartDatetime"] = list[i].StartDatetime??DateTime.Now;
                            ds.Tables[0].Rows[j]["EndDatetime"] = list[i].EndDatetime ?? DateTime.Now;
                            ds.Tables[0].Rows[j]["RechargeStatus"] = list[i].RechargeStatus;
                            ds.Tables[0].Rows[j]["SuccessfulAmount"] = list[i].SuccessfulAmount;
                            ds.Tables[0].Rows[j]["ChargeAccountInfo"] = list[i].ChargeAccountInfo;
                            ds.Tables[0].Rows[j]["RechargeMsg"] = list[i].RechargeMsg;
                            ds.Tables[0].Rows[j]["IsNotify"] = list[i].IsNotify;
                            ds.Tables[0].Rows[j]["OrderID"] = list[i].OrderID;
                            ds.Tables[0].Rows[j].EndEdit();
                            if (i == list.Count - 1)
                                break;
                        }
                        da.Update(ds.Tables[0]);
                    }
                    ds.Clear();
                    ds.Dispose();
                }
            }
        }
    }
}
