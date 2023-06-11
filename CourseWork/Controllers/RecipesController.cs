using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseWork.Data;
using CourseWork.Infrastructure.Models;
using CourseWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace CourseWork.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        private static Cloudinary cloudinary;
        private static string cloud_name = "dpmauufdt";
        private static string api_key = "378443462439775";
        private static string api_secret = "2-bzZf2fR5Mmm9mCcgRFOnuSaps";

        public RecipesController(ApplicationContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(int id)
        {
            var userId = await GetUserId();
            var applicationContext = _context.Recipe
                .Include(i => i.RecipeBook)
                .Where(i => i.RecipeBookId == id);
            if (!User.IsInRole("admin"))
            {
                foreach (var item in applicationContext)
                {
                    if (item.RecipeBook.UserId != userId)
                        return NotFound();
                }
            }

            var collection = await GetCustomCollection(id);
            var itemViewModel = new RecipeViewModel
            {
                Recipes = applicationContext,
                RecipeBook = collection
            };
            return View(itemViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var itemViewModel = await GetItemViewModel(id);

            if (itemViewModel.Recipe == null)
                return NotFound();

            return View(itemViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var customCollection = await GetCustomCollection(id);
            var userId = await GetUserId();

            if (!User.IsInRole("admin"))
            {
                if (customCollection.UserId != userId)
                    return NotFound();
            }
            
            var itemViewModel = new RecipeViewModel
            {
                RecipeBook = customCollection
            };
            return View(itemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(int id, [Bind("Id,Title,Tags,TextField1,Img")] Recipe recipe, RecipeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileName = await UploadedImage(viewModel);
                DbObjects.CreateItem(_context,recipe,id, fileName);
                return RedirectToAction("Index", new { id = id });
            }
            var customCollection = await GetCustomCollection(id);
            var itemViewModel = new RecipeViewModel
            {
                Recipe = recipe,
                RecipeBook = customCollection
            };
            return View(itemViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeItem(int id)
        {
            var userId = await GetUserId();
            var userName = await GetUserName();
            var like = await _context.LikeOnRecipe.FirstOrDefaultAsync(i => i.RecipeId == id && i.UserId == userId);

            if (like == null)
                DbObjects.LikeItem(_context, id, userId,userName);
            else
                ChangeLikeState(like);

            return RedirectToAction("Details", new {id = id});
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null)
                return NotFound();

            var recipeBook = await GetCustomCollection(recipe.RecipeBookId);

            if (!User.IsInRole("admin"))
            {
                var userId = await GetUserId();
                if (recipeBook.UserId != userId)
                    return NotFound();
            }

            var itemViewModel = new RecipeViewModel
            {
                RecipeBook = recipeBook,
                Recipe = recipe
            };

            return View(itemViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id,[Bind("Id,Title,Tags,TextField1,Img")] Recipe recipe, string imageUrl, RecipeViewModel recipeViewModel)
        {
            if (id != recipe.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var fileName = await UploadedImage(recipeViewModel);
                    recipe.Img = fileName ?? imageUrl;
                    recipe.RecipeBookId = recipeViewModel.Recipe.RecipeBookId;
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(recipe.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index", new { id = recipe.RecipeBookId });
            }
            var recipeBook = await GetCustomCollection(recipe.Id);
            var itemViewModel = new RecipeViewModel
            {
                Recipe = recipe,
                RecipeBook = recipeBook
            };
            
            return View(itemViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.Recipe
                .Include(i => i.RecipeBook)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (!User.IsInRole("admin"))
            {
                var userId = await GetUserId();

                if (item == null || item.RecipeBook.UserId != userId)
                    return NotFound();
            }

            if (item == null)
                return NotFound();

            return View(item);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Recipe.FindAsync(id);
            _context.Recipe.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new {id = item.RecipeBookId});
        }

        private bool ItemExists(int id)
        {
            return _context.Recipe.Any(e => e.Id == id);
        }

        private async Task<RecipeViewModel> GetItemViewModel(int? id)
        {
            var item = await _context.Recipe
                .Include(i => i.RecipeBook)
                .FirstOrDefaultAsync(m => m.Id == id);

            var likes = _context.LikeOnRecipe.Where(m => m.RecipeId == id);

            var itemViewModel = new RecipeViewModel
            {
                Recipe = item,                
                LikesOnRecipe = likes
            };

            return itemViewModel;
        }

        private async Task<RecipeBook> GetCustomCollection(int id)
        {
            var customCollection = await _context.RecipeBook
                .FirstOrDefaultAsync(i => i.Id == id);
            return customCollection;
        }

        private async Task<string> GetUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userId = user.Id;
            return userId;
        }

        private async Task<string> GetUserName()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userName = user.UserName;
            return userName;
        }

        private void ChangeLikeState(LikeOnRecipe like)
        {
            like.IsLiked = like.IsLiked ? like.IsLiked = false : like.IsLiked = true;

            _context.Update(like);
            _context.SaveChanges();
        }

        private async Task<string> UploadedImage(RecipeViewModel model)
        {
            string url = null;

            if (model.RecipeImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.RecipeImage.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    Account account = new Account(cloud_name, api_key, api_secret);
                    cloudinary = new Cloudinary(account);

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.RecipeImage.FileName, memoryStream),
                    };

                    ImageUploadResult result = await cloudinary.UploadAsync(uploadParams);
                    url = result.SecureUrl.ToString();
                }
            }

            return url;
        }
    }
}
