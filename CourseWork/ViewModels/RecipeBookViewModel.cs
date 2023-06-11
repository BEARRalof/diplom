using CourseWork.Infrastructure.Models;
using Microsoft.AspNetCore.Http;

namespace CourseWork.ViewModels
{
    public class RecipeBookViewModel
    {
        public RecipeBook RecipeBook { get; set; }
        public string UserName { get; set; }
        public IFormFile CollectionImage { get; set; }
    }
}
