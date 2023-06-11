using System.Collections.Generic;
using CourseWork.Infrastructure.Models;

namespace CourseWork.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<RecipeBook> RecipeBooks { get; set; }
        public IEnumerable<Recipe> Recipes { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
