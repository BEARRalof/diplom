using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Infrastructure.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название рецепта не может быть пустым")]
        [Display(Name = "Название рецепта")]
        public string Title { get; set; }

        [Display(Name = "Изображение:")]
        public string Img { get; set; }

        [Display(Name = "Тэги")]
        public string Tags { get; set; }
        public string TextField1 { get; set; }       
        public int RecipeBookId { get; set; }
        public RecipeBook RecipeBook { get; set; }
        public ICollection<LikeOnRecipe> Likes { get; set; }
    }
}
