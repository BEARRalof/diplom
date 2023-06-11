using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseWork.Data;
using CourseWork.Infrastructure.Models;
using CourseWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
            SignInManager<User> signInManager,ApplicationContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users.ToList());
        }

        public async Task<IActionResult> Edit(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                RolesViewModel model = new RolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = userRoles,
                    Roles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                await _signInManager.RefreshSignInAsync(user);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

        private async Task<IActionResult> ChangeUsers(string[] usersIds, Func<User, Task> handler)
        {
            if (usersIds != null)
            {
                var currentUserExist = false;
                foreach (var userId in usersIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        await handler(user);
                        var changedUser = await _userManager.FindByIdAsync(user.Id);
                        if (User.Identity.Name == user.UserName && (user.LockoutEnabled || changedUser == null))
                        {
                            currentUserExist = true;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found");
                    }
                }
                if (currentUserExist)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            return RedirectToAction("UserList", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string[] usersIds)
        {
            return await ChangeUsers(usersIds, async user => {
                user.IsBlocked = true;
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(3);
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string[] usersIds)
        {
            return await ChangeUsers(usersIds, async user =>
            {
                user.IsBlocked = false;
                user.LockoutEnabled = false;
                user.LockoutEnd = DateTimeOffset.UtcNow;
                await _userManager.UpdateAsync(user);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string[] usersIds)
        {
            return await ChangeUsers(usersIds, async user =>
            {
                IQueryable<Recipe> items = null;
                var deleteUser = await _context.User.FirstOrDefaultAsync(u => u.UserName == user.UserName);
                var likeOnItems = _context.LikeOnRecipe.Where(i => i.UserId == deleteUser.Id);       
                var collections = _context.RecipeBook.Where(i => i.UserId == deleteUser.Id);
                _context.LikeOnRecipe.RemoveRange(likeOnItems);
                foreach (var collection in collections)
                {
                    items = _context.Recipe.Where(i => i.RecipeBookId == collection.Id);
                }
                if(items != null)
                    _context.Recipe.RemoveRange(items);
                _context.RecipeBook.RemoveRange(collections);
                _context.User.Remove(deleteUser);
                await _context.SaveChangesAsync();
            });
        }
    }
}
