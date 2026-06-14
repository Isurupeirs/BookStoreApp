using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BookStoreApp.Database
{
    /// <summary>
    /// Manages the database connection for the BookStore application.
    /// </summary>
    public class DatabaseManager
    {
        /// <summary>
        /// The connection string used to connect to the BookStore database.
        /// </summary>
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
            AttachDbFilename=C:\Users\24369705\source\repos\BookStoreApp\BookStoreApp\BookStore\BookStore.mdf;
            Integrated Security=True";

        /// <summary>
        /// Opens and returns a new SQL connection to the database.
        /// </summary>
        public SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Closes the given SQL connection safely.
        /// </summary>
        public void CloseConnection(SqlConnection conn)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}

