using CourseWork.Infrastructure.Models;
using System;

namespace CourseWork.Infrastructure.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCartForUser(string userId);
        void AddItemInCart(int itemId, string userId);
        void Edit(Guid cartId, Cart cart);
        void Delete(Guid cartId);
    }
}
