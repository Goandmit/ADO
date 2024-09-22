using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ADO
{
    public static class ManagerSql
    {
        private static SqlConnection _sqlConnection;
        private static SqlDataAdapter _sqlDataAdapter;
        private static DataTable _sqlDataTable;

        public static string ConnectionStringMSSQL { get; set; }
        public static string ConnectionStatusMSSQL { get; set; }

        public static void UpdateConnectionStatus()
        {
            ConnectionStatusMSSQL = $"SqlConnection в состоянии: {_sqlConnection.State}";
        }

        public static void ConnectToDB()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "MSSQLLocalDBCustomers",
                IntegratedSecurity = true,
                UserID = "Admin",
                Password = "Admin1"
            };

            _sqlConnection = new SqlConnection(connectionStringBuilder.ConnectionString);            

            ConnectionStringMSSQL = connectionStringBuilder.ConnectionString;
            UpdateConnectionStatus();

            PrepareDB();
        }

        private static void PrepareDB()
        {
            _sqlConnection.Open();

            _sqlDataAdapter = new SqlDataAdapter();
            _sqlDataTable = new DataTable();

            var sql = @"SELECT * FROM Customers Order By Customers.ID";
            _sqlDataAdapter.SelectCommand = new SqlCommand(sql, _sqlConnection);

            sql = @"INSERT INTO Customers (Surname,  Name,  Patronymic, PhoneNumber, Email) 
                                 VALUES (@Surname,  @Name,  @Patronymic, @PhoneNumber, @Email); 
                     SET @ID = @@IDENTITY;";

            _sqlDataAdapter.InsertCommand = new SqlCommand(sql, _sqlConnection);

            _sqlDataAdapter.InsertCommand.Parameters.Add("@ID", SqlDbType.Int, 4, "ID").Direction = ParameterDirection.Output;
            _sqlDataAdapter.InsertCommand.Parameters.Add("@Surname", SqlDbType.NVarChar, 50, "Surname");
            _sqlDataAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");
            _sqlDataAdapter.InsertCommand.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50, "Patronymic");
            _sqlDataAdapter.InsertCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 50, "PhoneNumber");
            _sqlDataAdapter.InsertCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 50, "Email");

            sql = @"UPDATE Customers SET 
                           Surname = @Surname,
                           Name = @Name,
                           Patronymic = @Patronymic,
                           PhoneNumber = @PhoneNumber,
                           Email = @Email                           
                    WHERE ID = @ID";

            _sqlDataAdapter.UpdateCommand = new SqlCommand(sql, _sqlConnection);
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int, 0, "ID").SourceVersion = DataRowVersion.Original;
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@Surname", SqlDbType.NVarChar, 50, "Surname");
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@Patronymic", SqlDbType.NVarChar, 50, "Patronymic");
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 50, "PhoneNumber");
            _sqlDataAdapter.UpdateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 50, "Email");

            sql = "DELETE FROM Customers WHERE ID = @ID";

            _sqlDataAdapter.DeleteCommand = new SqlCommand(sql, _sqlConnection);
            _sqlDataAdapter.DeleteCommand.Parameters.Add("@ID", SqlDbType.Int, 4, "ID");

            _sqlDataAdapter.Fill(_sqlDataTable);

            _sqlConnection.Close();
        }

        public static List<Customer> GetCustomers()
        {
            _sqlConnection.Open();

            List<Customer> items = new List<Customer>();

            using (SqlDataReader reader = _sqlDataAdapter.SelectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Customer item = new Customer();

                    item.ID = reader["ID"].ToString();
                    item.Surname = reader["Surname"].ToString();
                    item.Name = reader["Name"].ToString();
                    item.Patronymic = reader["Patronymic"].ToString();
                    item.PhoneNumber = reader["PhoneNumber"].ToString();
                    item.Email = reader["Email"].ToString();

                    items.Add(item);
                }
            }

            _sqlConnection.Close();

            return items;
        }

        public static Customer GetCustomer(string id)
        {
            _sqlConnection.Open();

            Customer customer = new Customer();

            using (SqlDataReader reader = _sqlDataAdapter.SelectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (id == reader["ID"].ToString())
                    {
                        customer.ID = reader["ID"].ToString();
                        customer.Surname = reader["Surname"].ToString();
                        customer.Name = reader["Name"].ToString();
                        customer.Patronymic = reader["Patronymic"].ToString();
                        customer.PhoneNumber = reader["PhoneNumber"].ToString();
                        customer.Email = reader["Email"].ToString();
                    }
                }
            }

            _sqlConnection.Close();

            return customer;
        }

        public static void EditCustomer(Customer customer)
        {
            _sqlConnection.Open();

            foreach (DataRow row in _sqlDataTable.Rows)
            {
                if (row["ID"].ToString() == customer.ID)
                {
                    if (row["Surname"].ToString() != customer.Surname)
                    {
                        row["Surname"] = customer.Surname;
                    }

                    if (row["Name"].ToString() != customer.Name)
                    {
                        row["Name"] = customer.Name;
                    }

                    if (row["Patronymic"].ToString() != customer.Patronymic)
                    {
                        row["Patronymic"] = customer.Patronymic;
                    }

                    if (row["PhoneNumber"].ToString() != customer.PhoneNumber)
                    {
                        row["PhoneNumber"] = customer.PhoneNumber;
                    }

                    if (row["Email"].ToString() != customer.Email)
                    {
                        row["Email"] = customer.Email;
                    }
                }
            }

            _sqlDataAdapter.Update(_sqlDataTable);

            _sqlConnection.Close();
        }

        public static string AddCustomer(Customer customer)
        {
            _sqlConnection.Open();

            string id;

            DataRow newRow = _sqlDataTable.NewRow();

            newRow["Surname"] = customer.Surname;
            newRow["Name"] = customer.Name;
            newRow["Patronymic"] = customer.Patronymic;
            newRow["PhoneNumber"] = customer.PhoneNumber;
            newRow["Email"] = customer.Email;

            _sqlDataTable.Rows.Add(newRow);
            _sqlDataAdapter.Update(_sqlDataTable);

            int newRowIndex = _sqlDataTable.Rows.Count - 1;
            newRow = _sqlDataTable.Rows[newRowIndex];
            id = newRow["ID"].ToString();

            _sqlConnection.Close();

            return id;
        }

        public static void DeleteCustomer(string id)
        {
            _sqlConnection.Open();

            for (int i = 0; i < _sqlDataTable.Rows.Count; i++)
            {
                DataRow row = _sqlDataTable.Rows[i];

                if (row["ID"].ToString() == id)
                {
                    _sqlDataTable.Rows[i].Delete();
                    _sqlDataAdapter.Update(_sqlDataTable);
                    break;
                }
            }

            _sqlConnection.Close();
        }
    }
}
