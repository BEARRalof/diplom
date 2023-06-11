using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.ViewModels
{
    public class RolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public IList<string> UserRoles { get; set; }

        public RolesViewModel()
        {
            Roles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
