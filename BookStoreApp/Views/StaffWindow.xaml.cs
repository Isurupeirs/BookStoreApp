using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BookStoreApp.Database;
using BookStoreApp.Models;

namespace BookStoreApp.Views
{
    /// <summary>
    /// Staff window for managing library books and viewing student records.
    /// </summary>
    public partial class StaffWindow : Window
    {
        private Staff loggedInStaff;
        private string selectedBookName = "";

        /// <summary>
        /// Initializes the StaffWindow with the logged in staff member.
        /// </summary>
        public StaffWindow(Staff staff)
        {
            InitializeComponent();
            loggedInStaff = staff;
            lblStaffName.Text = "Welcome, " + staff.FirstName + " " + staff.LastName;
            LoadStaffLocation();
            LoadBooks();
        }

        /// <summary>
        /// Loads the staff member's location from the database.
        /// </summary>
        private void LoadStaffLocation()
        {
            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT Location FROM Members WHERE MemberID = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", loggedInStaff.MemberID);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                lblLocation.Text = "Location: " + result.ToString();
                loggedInStaff.Location = result.ToString();
            }

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Loads all books from the Books table into the DataGrid.
        /// </summary>
        private void LoadBooks()
        {
            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT BookId, Name, Quantity, Price, BorrowDate, StaffName FROM Books";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dgBooks.ItemsSource = table.DefaultView;

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Handles book selection from the DataGrid.
        /// </summary>
        private void dgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBooks.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgBooks.SelectedItem;
                selectedBookName = row["Name"].ToString();
                txtBookName.Text = row["Name"].ToString();
                txtCopies.Text = row["Quantity"].ToString();
                txtPrice.Text = row["Price"].ToString();
            }
        }

        /// <summary>
        /// Adds a new book to the Books table.
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtBookName.Text == "" || txtCopies.Text == "" || txtPrice.Text == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = @"INSERT INTO Books (BookName, TotalCopies, Price, BorrowDate)
                             VALUES (@name, @copies, @price, @date)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", txtBookName.Text);
            cmd.Parameters.AddWithValue("@copies", int.Parse(txtCopies.Text));
            cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
            cmd.Parameters.AddWithValue("@date", DateTime.Today);
            cmd.ExecuteNonQuery();

            db.CloseConnection(conn);
            MessageBox.Show("Book added successfully!");
            LoadBooks();
        }

        /// <summary>
        /// Updates the selected book in the Books table.
        /// </summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBookName == "")
            {
                MessageBox.Show("Please select a book to update.");
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = @"UPDATE Books SET TotalCopies=@copies, Price=@price
                             WHERE BookName=@name";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@copies", int.Parse(txtCopies.Text));
            cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
            cmd.Parameters.AddWithValue("@name", selectedBookName);
            cmd.ExecuteNonQuery();

            db.CloseConnection(conn);
            MessageBox.Show("Book updated successfully!");
            LoadBooks();
        }

        /// <summary>
        /// Deletes the selected book from the Books table.
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBookName == "")
            {
                MessageBox.Show("Please select a book to delete.");
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Are you sure you want to delete " + selectedBookName + "?",
                "Confirm Delete",
                MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                DatabaseManager db = new DatabaseManager();
                SqlConnection conn = db.GetConnection();

                string query = "DELETE FROM Books WHERE BookName=@name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", selectedBookName);
                cmd.ExecuteNonQuery();

                db.CloseConnection(conn);
                MessageBox.Show("Book deleted successfully!");
                LoadBooks();
            }
        }

        /// <summary>
        /// Sorts the books DataGrid by borrow date.
        /// </summary>
        private void btnSortDate_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT * FROM Books ORDER BY BorrowDate";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dgBooks.ItemsSource = table.DefaultView;

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Sorts the books DataGrid by price.
        /// </summary>
        private void btnSortPrice_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT * FROM Books ORDER BY Price";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dgBooks.ItemsSource = table.DefaultView;

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Searches student records by book name.
        /// </summary>
        private void btnSearchStudent_Click(object sender, RoutedEventArgs e)
        {
            string bookName = txtSearchStudent.Text.Trim();

            if (bookName == "")
            {
                MessageBox.Show("Please enter a book name to search.");
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT * FROM Student WHERE BookName LIKE @name";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", "%" + bookName + "%");

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("No records found for that book.");
            }
            else
            {
                dgStudentRecords.ItemsSource = table.DefaultView;
            }

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Searches student records between two dates.
        /// </summary>
        private void btnSearchDates_Click(object sender, RoutedEventArgs e)
        {
            if (dpFrom.SelectedDate == null || dpTo.SelectedDate == null)
            {
                MessageBox.Show("Please select both From and To dates.");
                return;
            }

            DateTime fromDate = dpFrom.SelectedDate.Value;
            DateTime toDate = dpTo.SelectedDate.Value;

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT * FROM Student WHERE BorrowedDate BETWEEN @from AND @to";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@from", fromDate);
            cmd.Parameters.AddWithValue("@to", toDate);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("No records found between those dates.");
            }
            else
            {
                dgStudentRecords.ItemsSource = table.DefaultView;
            }

            db.CloseConnection(conn);
        }
    }
}