using System.Collections.Generic;
using CourseWork.Infrastructure.Models;
using Microsoft.AspNetCore.Http;

namespace CourseWork.ViewModels
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<Recipe> Recipes { get; set; }
        public IFormFile RecipeImage { get; set; }

        public RecipeBook RecipeBook { get; set; }
        public IEnumerable<LikeOnRecipe> LikesOnRecipe { get; set; }
    }
}
