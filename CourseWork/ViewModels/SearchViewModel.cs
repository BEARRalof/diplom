using System.Collections.Generic;
using CourseWork.Infrastructure.Models;

namespace CourseWork.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<Recipe> Recipes { get; set; }
        public string Query { get; set; }
    }
}
