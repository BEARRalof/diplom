using CourseWork.Infrastructure.Interfaces;
using CourseWork.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CourseWork.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;

        public OrderController(UserManager<User> userManager, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Payment(Guid id, string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);

            _orderRepository.Add(id, user);

            return View("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
