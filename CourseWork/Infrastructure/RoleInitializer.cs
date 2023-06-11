using System.Threading.Tasks;
using CourseWork.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Infrastructure
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@mail.ru";
            string login = "admin";
            string password = "admin";

            if (await roleManager.FindByNameAsync("admin") == null)
                await roleManager.CreateAsync(new IdentityRole("admin"));
            if (await userManager.FindByNameAsync(login) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = login
                };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
