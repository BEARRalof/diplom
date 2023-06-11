using CourseWork.Infrastructure.Models;

namespace CourseWork.Data
{
    public class DbObjects
    {
        public static async void CreateItem(ApplicationContext context, Recipe recipe,int id, string img)
        {
            await context.AddAsync(new Recipe
            {
                Title = recipe.Title,
                Tags = recipe.Tags, 
                TextField1 = recipe.TextField1,
                RecipeBookId = id,
                Img = img
            });

            context.SaveChanges();
        }

        public static async void LikeItem(ApplicationContext context, int id, string userId,string userName)
        {
            await context.AddAsync(new LikeOnRecipe()
            {
                IsLiked = true,
                RecipeId = id,
                UserName = userName,
                UserId = userId
            });

            context.SaveChanges();
        }
    }
}
