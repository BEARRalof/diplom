using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Infrastructure.Models
{
    public class RecipeBook
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Данное поле не может быть пустым.")]
        [Display(Name = "Название рецепта:")]
        public string Title { get; set; }

        [Display(Name = "Описание:")]
        public string Description { get; set; }

        [Display(Name = "Изображение:")]
        public string Img { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
