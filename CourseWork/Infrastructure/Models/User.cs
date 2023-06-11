using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Infrastructure.Models
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public bool IsBlocked { get; set; }
        public ICollection<LikeOnRecipe> LikeOnRecipes{ get; set; }
        public ICollection<RecipeBook> RecipeBooks { get; set; }
    }
}
