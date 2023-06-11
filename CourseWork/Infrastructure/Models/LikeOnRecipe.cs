namespace CourseWork.Infrastructure.Models
{
    public class LikeOnRecipe
    {
        public int Id { get; set; }
        public bool IsLiked { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
