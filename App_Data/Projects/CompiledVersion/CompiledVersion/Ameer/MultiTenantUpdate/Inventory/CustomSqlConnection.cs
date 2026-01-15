using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MTUInventory
{
    public class CustomSqlConnection : IDisposable
    {
        private SqlConnection _connection;

        public CustomSqlConnection(string connectionString)
        {
            this._connection = new SqlConnection(connectionString);
            this._connection.Open();
        }

        public void Dispose()
        {
            if (this._connection != null)
            {
                this._connection.Close();
                this._connection.Dispose();
            }
        }

        public int GetCompanyID(string companyCD)
        {
            int num;
            SqlCommand command = new SqlCommand("SELECT CompanyID FROM Company WHERE CompanyCD=@companycd", this._connection);
            command.Parameters.AddWithValue("@companycd", companyCD);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new Exception(string.Concat("Can't read company id for companyCD=", companyCD));
                }
                else
                {
                    num = Convert.ToInt32(reader[0]);
                }
            }
            return num;
        }

        public List<string> GetOtherCompanyNames(string callerTenantName)
        {
            SqlCommand command = new SqlCommand("SELECT CompanyKey FROM Company WHERE CompanyCD <> @companycd AND CompanyID>1", this._connection);
            command.Parameters.AddWithValue("@companycd", callerTenantName);
            List<string> otherCompanyNames = new List<string>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    otherCompanyNames.Add(reader.GetString(0));
                }
            }
            return otherCompanyNames;
        }
    }
}
