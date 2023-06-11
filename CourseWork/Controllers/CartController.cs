using CourseWork.Data;
using CourseWork.Infrastructure.Interfaces;
using CourseWork.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly ApplicationContext _context;

        public CartController(ICartRepository cartRepository, UserManager<User> userManager, ApplicationContext context)
        {
            _context = context;
            _userManager = userManager;
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);

            Cart cart = _cartRepository.GetCartForUser(user.Id);

            return View(cart);
        }

        [HttpGet]
        public IActionResult AddItemInCart(int id)
        {
            Recipe recipe = _context.Recipe.FirstOrDefault(x => x.Id == id);
            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemInCart(int id, string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            _cartRepository.AddItemInCart(id, user.Id);
            return RedirectToAction("Index", new { UserName = userName });
        }

        public IActionResult Delete(Guid id)
        {
            _cartRepository.Delete(id);

            return RedirectToAction("Index", "Home");
        }
    }
}
