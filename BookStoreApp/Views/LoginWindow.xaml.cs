using System.Data.SqlClient;
using System.Windows;
using BookStoreApp.Database;
using BookStoreApp.Models;

namespace BookStoreApp.Views
{
    /// <summary>
    /// Login window for the BookStore Library application.
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// Initializes the LoginWindow.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the login button click. Validates user credentials from the database.
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (username == "" || password == "")
            {
                txtError.Text = "Please enter username and password.";
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT * FROM Members WHERE UserName=@u AND Password=@p";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string memberType = reader["MemberType"].ToString();

                if (memberType == "SFM")
                {
                    Staff staff = new Staff();
                    staff.MemberID = (int)reader["MemberID"];
                    staff.FirstName = reader["FirstName"].ToString();
                    staff.LastName = reader["LastName"].ToString();
                    staff.MemberType = "SFM";

                    db.CloseConnection(conn);

                    StaffWindow sw = new StaffWindow(staff);
                    sw.Show();
                    this.Close();
                }
                else if (memberType == "STM")
                {
                    Student student = new Student();
                    student.MemberID = (int)reader["MemberID"];
                    student.FirstName = reader["FirstName"].ToString();
                    student.LastName = reader["LastName"].ToString();
                    student.MemberType = "STM";

                    db.CloseConnection(conn);

                    StudentWindow stw = new StudentWindow(student);
                    stw.Show();
                    this.Close();
                }
            }
            else
            {
                db.CloseConnection(conn);
                txtError.Text = "Invalid username or password.";
            }
        }
    }
}