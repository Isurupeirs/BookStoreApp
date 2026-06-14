using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApp.Models
{
    /// <summary>
    /// Represents a student member of the library.
    /// </summary>
    public class Student : Member
    {
        /// <summary>Gets the full display name of the student.</summary>
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}