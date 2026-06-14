using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApp.Models
{
    /// <summary>
    /// Represents a staff member of the library.
    /// </summary>
    public class Staff : Member
    {
        /// <summary>Gets or sets the office location of the staff member.</summary>
        public string Location { get; set; }
    }
}