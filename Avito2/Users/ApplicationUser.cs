using Microsoft.AspNetCore.Identity;
using System;

namespace Avito2.Users
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => LastName + " " + FirstName;
    }
}
