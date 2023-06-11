using CourseWork.Data;
using CourseWork.Infrastructure.Interfaces;
using CourseWork.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CourseWork.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationContext _context;

        public OrderRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void Add(Guid cartId, User user)
        {
            Order order = _context.Orders
                .Include(x => x.Cart)
                .Include(x => x.User)
                .FirstOrDefault(x => x.CartId == cartId);

            if (order == null)
            {
                order = new Order()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    User = user
                };
            }

            Cart cart = _context.Carts.FirstOrDefault(x => x.Id == cartId);
            if (cart != null)
            {
                order.CartId = cartId;
                order.Cart = cart;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Delete(Guid orderId)
        {
            Order order = GetOrderById(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        public Order GetOrderById(Guid orderId)
        {
            return _context.Orders
                .Include(x => x.Cart)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == orderId);
        }
    }
}
