using CourseWork.Infrastructure.Models;
using System;

namespace CourseWork.Infrastructure.Interfaces
{
    public interface IOrderRepository
    {
        void Add(Guid cartId, User user);
        void Delete(Guid orderId);
        Order GetOrderById(Guid orderId);
    }
}
