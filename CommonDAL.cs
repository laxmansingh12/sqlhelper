using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using SQLDataUtility;
using SBMS.Utilities;

namespace SBMS.DAL.Common
{
    public class CommonDAL
    {
        public DataSet GetGSTData(GSTTYpe gstType, DateTime from, DateTime to)
        {
            SqlParameter[] param =
                {
                new SqlParameter("@DateFrom", from),
                new SqlParameter("@DateTo", to)
            };

            return SqlHelper.ExecuteDataset(CommandType.StoredProcedure, gstType == GSTTYpe.GST1 ? "usp_GetGSTR1Data" : "usp_GetGSTR2Data", param);
        }

        public DataTable GetSupplierList(int supplierID = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select 0 AS Id, 'Select' AS Name 
            union all
            select Id, Name from supplier where Active = 1");
            DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, sb.ToString()).Tables[0];
            return dt;
        }

        public DataTable GetProductList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select 0 AS Id, 'Select' AS Name 
            union all
            select id, ProductName as Name from Product where Active = 1");
            DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, sb.ToString()).Tables[0];
            return dt;
        }

        public DataTable GetFileData(int linkID, string PageIdentification)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"  SELECT  Name,Extension,FileData from documents
                                WHERE linkID = {0} and PageIdentification = '{1}'", linkID, PageIdentification);
            return SqlHelper.ExecuteDataset(System.Data.CommandType.Text, sb.ToString()).Tables[0];
        }

        public DataTable GetTrasportData(int id, string tableName = "PURCHASE")
        {
            tableName = tableName.ToLower() == "purchasereturn" ? "purchase" : tableName;
            tableName = tableName.ToLower() == "salereturn" ? "sale" : tableName;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"  SELECT ISNULL(TD.DeliveryMode, '') AS DeliveryMode, TD.SupplyTime AS SupplyTime, 
            ISNULL(TD.PlaceOfDelivery,'') AS PlaceOfDelivery, ISNULL(TD.GSTIN, 0) AS GSTIN, ISNULL(TD.MobileNo,'') AS MobileNo, ISNULL(TD.VehicleNo,'') AS VehicleNo
            FROM {0} PUR 
            INNER JOIN TransportDetail TD ON TD.SNo = PUR.TransportSNo 
            WHERE PUR.id = {1}", tableName, id);
            return SqlHelper.ExecuteDataset(System.Data.CommandType.Text, sb.ToString()).Tables[0];
        }

        public DataTable GetChargesData(int id, string tableName = "PURCHASE")
        {
            StringBuilder sb = new StringBuilder();
            if (tableName.ToLower() == "purchasereturn")
            {
                sb.AppendFormat(@" SELECT 0 SNO, 0 Freight, 0 Packing, 0 Insurence, 0 Other");
            }
            else
            {
                sb.AppendFormat(@"  SELECT C.* FROM " + tableName + " P INNER JOIN CHARGES C ON P.CHARGESSNO = C.SNO WHERE P.ID = {0}", id);
            }

            return SqlHelper.ExecuteDataset(System.Data.CommandType.Text, sb.ToString()).Tables[0];
        }

        public string GetInvoiceNumber(string invoiceType, string orderNumber = "")
        {
            SqlParameter[] param =
                 {
                new SqlParameter("@InvoiceType", invoiceType),
                new SqlParameter("@OrderNumber", orderNumber)
            };

            return Convert.ToString(SqlHelper.ExecuteScalar(CommandType.StoredProcedure, "usp_GetInvoiceNumber", param));
        }

        public DataTable GetIncomeAndExpenses()
        {
            string query = string.Format(@"Select Id,Category, Title, Amount from IncomeAndExpenses where IsDeleted = 0 AND FY = dbo.GetFY()");
            DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, query).Tables[0];
            return dt;
        }

        public DataTable GetProductGroups()
        {
            string query = string.Format(@"Select Id, GroupName from ProductGroup");
            DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, query).Tables[0];
            return dt;
        }

        public int SaveIncomeAndExpense(string category, string title, decimal amount)
        {
            string query = string.Format(@"INSERT INTO IncomeAndExpenses(Title, Amount, Category,FY, CreatedOn,CreatedBy,ModifiedOn, ModifiedBy, IsDeleted) VALUES ('{0}', {1}, '{2}', dbo.GetFY(), GETDATE(),{3}, GETDATE(),{3},0)", title, amount, category, DataStorageUtility.ADMIN_MODEL.Id);
            return SqlHelper.ExecuteNonQuery(CommandType.Text, query);
        }

        public int SaveProductGroup(int id, string name)
        {
            string query = string.Empty;
            if (id > 0)
            {
                query = string.Format(@"UPDATE ProductGroup SET GroupName = '{0}' WHERE Id = {1}",id, name);

            }
            else
            {
                query = string.Format(@"INSERT INTO ProductGroup(GroupName) VALUES ('{0}')", name);
            }
            return SqlHelper.ExecuteNonQuery(CommandType.Text, query);
        }

        public int DeleteIncomeAndExpense(int id)
        {
            string query = string.Format(@"DELETE FROM IncomeAndExpenses WHERE Id = {0}", id);
            return SqlHelper.ExecuteNonQuery(CommandType.Text, query);
        }

        public int DeleteProductGroup(int id)
        {
            string query = string.Format(@"DELETE FROM ProductGroup WHERE Id = {0}", id);
            return SqlHelper.ExecuteNonQuery(CommandType.Text, query);
        }

    }
}
