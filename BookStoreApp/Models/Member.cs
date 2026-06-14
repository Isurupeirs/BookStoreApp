using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApp.Models
{
    /// <summary>
    /// Base class for all library members.
    /// </summary>
    public class Member
    {
        /// <summary>Gets or sets the member ID.</summary>
        public int MemberID { get; set; }

        /// <summary>Gets or sets the first name.</summary>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the last name.</summary>
        public string LastName { get; set; }

        /// <summary>Gets or sets the member type (SFM or STM).</summary>
        public string MemberType { get; set; }

        /// <summary>Gets or sets the username.</summary>
        public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; }
    }
}
