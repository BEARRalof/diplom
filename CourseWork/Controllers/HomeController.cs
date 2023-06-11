using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CourseWork.Data;
using CourseWork.Infrastructure.Models;
using CourseWork.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var collections = GetTopCollections();
            var items = GetLastAddedItems();
            var tags = GetUniqueTags();

            var homeViewModel = new HomeViewModel
            {
                RecipeBooks = collections,
                Recipes = items,
                Tags = tags
            };
            return View(homeViewModel);
        }

        public IActionResult Search(string query)
        {
            var items = _context.Recipe
                .Include(c => c.RecipeBook)
                .Where(x => x.Title.Contains(query) || x.Tags.Contains(query) 
                            || x.TextField1.Contains(query)
                            || x.RecipeBook.Description.Contains(query));

            var search = new SearchViewModel
            {
                Recipes = items,
                Query = query
            };

            return View(search);
        }

        public IActionResult SearchByTags(string tags)
        {
            var items = _context.Recipe
                .Where(x => x.Tags.Contains(tags));

            var search = new SearchViewModel
            {
                Recipes = items,
                Query = tags
            };

            return View(search);
        }

        private IEnumerable<RecipeBook> GetTopCollections()
        {
            var collections = _context.RecipeBook
                .Include(i => i.Recipes)
                .OrderByDescending(i => i.Recipes.Count).Take(3);

            return collections;
        }

        private IEnumerable<Recipe> GetLastAddedItems()
        {
            var items = _context.Recipe
                .OrderByDescending(i => i.Id).Take(4);

            return items;
        }

        private IEnumerable<string> GetUniqueTags()
        {
            IEnumerable<string> tags = null;

            var tempTags = _context.Recipe
                .Select(i => i.Tags)
                .Where(t => t != null)
                .Distinct().Take(10);
            
            if(tempTags != null)
            {
                tags = SplitBy(tempTags).Distinct();
            }
            return tags;
        }

        private static IEnumerable<string> SplitBy(IEnumerable<string> tags)
        {
            return tags.SelectMany(s => s.Split(",")).OrderBy(p => p);
        }
    }
}
