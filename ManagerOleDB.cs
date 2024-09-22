using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace ADO
{
    public static class ManagerOleDB
    {
        private static OleDbConnection _oleDbConnection;
        private static OleDbDataAdapter _oleDbDataAdapter;        
        private static DataTable _oleDbDataTable;

        public static string ConnectionStringMSAccess { get; set; }
        public static string ConnectionStatusMSAccess { get; set; }

        public static void UpdateConnectionStatus()
        {           
            ConnectionStatusMSAccess = $"OleDbConnection в состоянии: {_oleDbConnection.State}";        
        }

        public static void ConnectToDB()
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=MSAccessPurchases.accdb;" +
                "Persist Security Info=True;";                

            _oleDbConnection = new OleDbConnection(connectionString);            

            ConnectionStringMSAccess = connectionString;
            UpdateConnectionStatus();

            PrepareDB();
        }

        public static void PrepareDB()
        {
            _oleDbConnection.Open();

            _oleDbDataAdapter = new OleDbDataAdapter();
            _oleDbDataTable = new DataTable();

            var sql = @"SELECT * FROM Purchases Order By Purchases.ID";
            _oleDbDataAdapter.SelectCommand = new OleDbCommand(sql, _oleDbConnection);

            sql = @"DELETE FROM Purchases WHERE ID = @ID";
            _oleDbDataAdapter.DeleteCommand = new OleDbCommand(sql, _oleDbConnection);
            _oleDbDataAdapter.DeleteCommand.Parameters.Add("@ID", OleDbType.Integer, 4, "ID");

            _oleDbDataAdapter.Fill(_oleDbDataTable);

            _oleDbConnection.Close();
        }

        public static List<Purchase> GetPurchases()
        {
            _oleDbConnection.Open();

            List<Purchase> items = new List<Purchase>();            

            using (OleDbDataReader reader = _oleDbDataAdapter.SelectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Purchase item = new Purchase();

                    item.ID = reader["ID"].ToString();
                    item.Email = reader["Email"].ToString();
                    item.ProductCode = reader["ProductCode"].ToString();
                    item.ProductName = reader["ProductName"].ToString();

                    items.Add(item);
                }
            }

            _oleDbConnection.Close();

            return items;
        }

        public static List<Purchase> GetPurchasesByEmail(string email)
        {
            _oleDbConnection.Open();                

            string sql = "SELECT * FROM Purchases " +
                         "WHERE Purchases.Email = @Email " +
                         "Order By Purchases.ID";

            OleDbCommand command = new OleDbCommand(sql, _oleDbConnection);

            OleDbParameter param = new OleDbParameter("@Email", OleDbType.LongVarChar, 50);
            param.Value = email;

            command.Parameters.Add(param);

            List<Purchase> items = new List<Purchase>();

            using (OleDbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Purchase item = new Purchase();

                    item.ID = reader["ID"].ToString();
                    item.Email = reader["Email"].ToString();
                    item.ProductCode = reader["ProductCode"].ToString();
                    item.ProductName = reader["ProductName"].ToString();

                    items.Add(item);
                }
            }

            _oleDbConnection.Close();

            return items;
        }

        public static Purchase GetPurchase(string id)
        {
            _oleDbConnection.Open();

            Purchase purchase = new Purchase();

            using (OleDbDataReader reader = _oleDbDataAdapter.SelectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (id == reader["ID"].ToString())
                    {
                        purchase.ID = reader["ID"].ToString();
                        purchase.Email = reader["Email"].ToString();
                        purchase.ProductCode = reader["ProductCode"].ToString();
                        purchase.ProductName = reader["ProductName"].ToString();
                    }
                }
            }

            _oleDbConnection.Close();

            return purchase;
        }

        public static void EditPurchase(Purchase purchase)
        {
            _oleDbConnection.Open();

            string sql = "UPDATE Purchases " +
                "SET Email = @Email, " +
                "ProductCode = @ProductCode, " +
                "ProductName = @ProductName " +
                "WHERE ID = @ID";

            OleDbCommand command = new OleDbCommand(sql, _oleDbConnection);

            OleDbParameter param1 = new OleDbParameter("@Email", OleDbType.LongVarChar, 50);
            param1.Value = purchase.Email;

            OleDbParameter param2 = new OleDbParameter("@ProductCode", OleDbType.LongVarChar, 50);
            param2.Value = purchase.ProductCode;

            OleDbParameter param3 = new OleDbParameter("@ProductName", OleDbType.LongVarChar, 50);
            param3.Value = purchase.ProductName;

            OleDbParameter param4 = new OleDbParameter("@ID", OleDbType.Integer, 0);
            param4.Value = purchase.ID;

            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);
            command.Parameters.Add(param4).SourceVersion = DataRowVersion.Original;

            command.ExecuteNonQuery();            
            _oleDbDataAdapter.Fill(_oleDbDataTable);

            _oleDbConnection.Close();
        }

        public static string AddPurchase(Purchase purchase)
        {
            _oleDbConnection.Open();

            string id;

            string sql = "INSERT INTO Purchases (Email,  ProductCode,  ProductName) " +
                         "VALUES (@Email,  @ProductCode,  @ProductName)";

            OleDbCommand command = new OleDbCommand(sql, _oleDbConnection);

            OleDbParameter param1 = new OleDbParameter("@Email", OleDbType.LongVarChar, 50);            
            param1.Value = purchase.Email;

            OleDbParameter param2 = new OleDbParameter("@ProductCode", OleDbType.LongVarChar, 50);
            param2.Value = purchase.ProductCode;

            OleDbParameter param3 = new OleDbParameter("@ProductName", OleDbType.LongVarChar, 50);
            param3.Value = purchase.ProductName;

            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);

            command.ExecuteNonQuery();
            _oleDbDataAdapter.Fill(_oleDbDataTable);

            int newRowIndex = _oleDbDataTable.Rows.Count - 1;
            DataRow newRow = _oleDbDataTable.Rows[newRowIndex];
            id = newRow["ID"].ToString();

            _oleDbConnection.Close();

            return id;
        }
        public static void DeletePurchase(string id)
        {
            _oleDbConnection.Open();

            for (int i = 0; i < _oleDbDataTable.Rows.Count; i++)
            {
                DataRow row = _oleDbDataTable.Rows[i];

                if (row["ID"].ToString() == id)
                {
                    _oleDbDataTable.Rows[i].Delete();
                    _oleDbDataAdapter.Update(_oleDbDataTable);
                    break;
                }
            }

            _oleDbConnection.Close();
        }
    }
}
