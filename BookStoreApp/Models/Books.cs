using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApp.Models
{
    /// <summary>
    /// Represents a book in the library.
    /// </summary>
    public class Books
    {
        /// <summary>Gets or sets the book name.</summary>
        public string BookName { get; set; }

        /// <summary>Gets or sets the total copies available.</summary>
        public int TotalCopies { get; set; }

        /// <summary>Gets or sets the date the book can be borrowed.</summary>
        public string BorrowDate { get; set; }

        /// <summary>Gets or sets the price of the book.</summary>
        public decimal Price { get; set; }
    }
}
