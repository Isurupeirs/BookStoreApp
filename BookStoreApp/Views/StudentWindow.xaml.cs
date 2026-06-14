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
    /// Student window for borrowing books from the library.
    /// </summary>
    public partial class StudentWindow : Window
    {
        private Student loggedInStudent;
        private string selectedBookName = "";
        private decimal bookPrice = 0;
        private DateTime returnDate;

        /// <summary>
        /// Initializes the StudentWindow with the logged in student.
        /// </summary>
        public StudentWindow(Student student)
        {
            InitializeComponent();
            loggedInStudent = student;
            lblStudentName.Text = "Welcome, " + student.FullName;
            LoadStaffContact();
        }

        /// <summary>
        /// Loads the staff contact name from the Books table.
        /// </summary>
        private void LoadStaffContact()
        {
            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT TOP 1 StaffName FROM Books";
            SqlCommand cmd = new SqlCommand(query, conn);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                lblStaffContact.Text = "Library Staff Contact: " + result.ToString();
            }

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Searches for a book by name in the database.
        /// </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string bookName = txtSearch.Text.Trim();

            if (bookName == "")
            {
                MessageBox.Show("Please enter a book name to search.");
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            string query = "SELECT BookId, Name, Quantity, BorrowDate, Price FROM Books WHERE Name LIKE @name";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", "%" + bookName + "%");

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("Book not found in the library database.");
            }
            else
            {
                dgBooks.ItemsSource = table.DefaultView;
            }

            db.CloseConnection(conn);
        }

        /// <summary>
        /// Handles selection of a book from the search results.
        /// </summary>
        private void dgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBooks.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgBooks.SelectedItem;
                selectedBookName = row["Name"].ToString();
                bookPrice = Convert.ToDecimal(row["Price"]);
                lblMessage.Text = "Selected: " + selectedBookName;
            }
        }
        /// <summary>
        /// Calculates the return date based on borrow days entered.
        /// </summary>
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBookName == "")
            {
                MessageBox.Show("Please select a book first.");
                return;
            }

            int days = 0;
            bool isValid = int.TryParse(txtDays.Text, out days);

            if (!isValid || days <= 0)
            {
                MessageBox.Show("Please enter a valid number of days.");
                return;
            }

            if (days > 14)
            {
                lblMessage.Text = "Not allowed. Maximum borrow period is 14 days.";
                lblMessage.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                returnDate = DateTime.Today.AddDays(days);
                lblReturnDate.Text = "Return by: " + returnDate.ToString("dd/MM/yyyy");
                lblMessage.Text = "Borrow period allowed.";
                lblMessage.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        /// <summary>
        /// Finalises the book borrow and updates the database.
        /// </summary>
        private void btnFinalise_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBookName == "")
            {
                MessageBox.Show("Please search and select a book first.");
                return;
            }

            if (returnDate == DateTime.MinValue)
            {
                MessageBox.Show("Please calculate the return date first.");
                return;
            }

            DatabaseManager db = new DatabaseManager();
            SqlConnection conn = db.GetConnection();

            // Reduce book count in Books table
            string updateQuery = "UPDATE Books SET Quantity = Quantity - 1 WHERE Name = @book";
            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@book", selectedBookName);
            updateCmd.ExecuteNonQuery();

            // Insert record into Student table
            string insertQuery = @"INSERT INTO Student 
        (StudentID, BookName, BorrowedDate, ReturnDate, Price, TotalPrice)
        VALUES (@sid, @book, @bdate, @rdate, @price, @total)";
            SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@sid", loggedInStudent.MemberID);
            insertCmd.Parameters.AddWithValue("@book", selectedBookName);
            insertCmd.Parameters.AddWithValue("@bdate", DateTime.Today);
            insertCmd.Parameters.AddWithValue("@rdate", returnDate);
            insertCmd.Parameters.AddWithValue("@price", bookPrice);
            insertCmd.Parameters.AddWithValue("@total", bookPrice);
            insertCmd.ExecuteNonQuery();

            db.CloseConnection(conn);
            MessageBox.Show("Book borrowed successfully!");
            btnCancel_Click(sender, e);
        }

        /// <summary>
        /// Clears all fields and resets the form.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            txtDays.Text = "";
            dgBooks.ItemsSource = null;
            lblReturnDate.Text = "";
            lblMessage.Text = "";
            selectedBookName = "";
            bookPrice = 0;
            returnDate = DateTime.MinValue;
        }
    }
}