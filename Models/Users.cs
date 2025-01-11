using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.Reflection;

namespace DASS.Models
{
    public class Users : IdentityUser
    {
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }
        
        public string LastName { get; set; }
        
	}
}
