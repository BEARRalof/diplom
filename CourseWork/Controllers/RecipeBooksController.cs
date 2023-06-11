using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWork.Data;
using CourseWork.Infrastructure.Models;
using CourseWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Controllers
{
    public class RecipeBooksController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private static Cloudinary cloudinary;
        private static string cloud_name = "dpmauufdt";
        private static string api_key = "378443462439775";
        private static string api_secret = "2-bzZf2fR5Mmm9mCcgRFOnuSaps";

        public RecipeBooksController(ApplicationContext context, UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customCollection = await _context.RecipeBook
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!User.IsInRole("admin"))
            {
                if (customCollection == null || customCollection.UserId != user.Id)
                    return NotFound();
            }
            if (customCollection == null)
                return NotFound();


            return View(customCollection);
        }

        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customCollection = await _context.RecipeBook
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customCollection == null)
            {
                return NotFound();
            }

            return View(customCollection);
        }

        [Authorize]
        public IActionResult Create(string userName)
        {
            if (userName == null)
                return NotFound();
            var model = new RecipeBookViewModel
            {
                UserName = userName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Img")] RecipeBook recipeBook,string userName,RecipeBookViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("admin"))
                {
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user == null)
                        return NotFound();
                    recipeBook.UserId = user.Id;
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user == null)
                        return NotFound();
                    recipeBook.UserId = user.Id;
                }
                var fileName = await UploadedImage(viewModel);
                recipeBook.Img = fileName;

                _context.Add(recipeBook);
                await _context.SaveChangesAsync();
                return RedirectToAction("Personal","Account", new {UserName = userName});
            }

            var model = new RecipeBookViewModel
            {
                RecipeBook = recipeBook,
                UserName = userName
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id,string userName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(userName);

            var customCollection = await _context.RecipeBook
                .Include(u => u.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (!User.IsInRole("admin"))
            {
                var tempUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null || user.UserName != tempUser.UserName)
                    return NotFound();

                if (customCollection == null || customCollection.UserId != user.Id)
                    return NotFound();
            }

            if (customCollection == null)
                return NotFound();

            var model = new RecipeBookViewModel
            {
                RecipeBook = customCollection,
                UserName = userName
            };

            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Img")] RecipeBook recipeBook, string userName, string imageUrl, RecipeBookViewModel viewModel)
        {
            if (id != recipeBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var fileName = await UploadedImage(viewModel);
                    recipeBook.Img = fileName ?? imageUrl;
                    var user = await _userManager.FindByNameAsync(userName);
                    recipeBook.UserId = user.Id;
                    _context.Update(recipeBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomCollectionExists(recipeBook.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details",new {id = id});
            }

            var model = new RecipeBookViewModel
            {
                RecipeBook = recipeBook,
                UserName = userName
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customCollection = await _context.RecipeBook
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!User.IsInRole("admin"))
            {
                if (customCollection == null || customCollection.UserId != user.Id)
                    return NotFound();
            }
            if (customCollection == null)
                return NotFound();

            return View(customCollection);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customCollection = await _context.RecipeBook
                .Include(c => c.User)
                .FirstOrDefaultAsync(i => i.Id == id);
            var userName = customCollection.User.UserName;
            _context.RecipeBook.Remove(customCollection);
            await _context.SaveChangesAsync();
            return RedirectToAction("Personal","Account",new {UserName = userName });
        }

        private bool CustomCollectionExists(int id)
        {
            return _context.RecipeBook.Any(e => e.Id == id);
        }

        private async Task<string> UploadedImage(RecipeBookViewModel model)
        {
            string url = null;

            if (model.CollectionImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.CollectionImage.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    Account account = new Account(cloud_name, api_key, api_secret);
                    cloudinary = new Cloudinary(account);

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.CollectionImage.FileName, memoryStream),
                    };

                    ImageUploadResult result = await cloudinary.UploadAsync(uploadParams);
                    url = result.SecureUrl.ToString();
                }
            }

            return url;
        }
    }
}
