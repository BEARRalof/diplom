using CourseWork.Data;
using CourseWork.Infrastructure.Interfaces;
using CourseWork.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private const string ExceptionMessage = "Cart id to edit do not equal entered cart id"; 

        private readonly ApplicationContext _context;

        public CartRepository(ApplicationContext context)
        {
            _context = context;
        }

        public void AddItemInCart(int itemId, string userId)
        {
            Cart cart = _context.Carts
                .Include(c => c.User)
                .FirstOrDefault(x => x.UserId == userId);
            if (cart == null)
            {
                cart = new Cart() 
                    { 
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        User = _context.Users.First(x => x.Id == userId),
                        RecipesInCart = new List<Recipe>()
                    };
            }

            Recipe recipe = _context.Recipe.FirstOrDefault(x => x.Id == itemId);
            if (recipe != null)
            {
                cart.RecipesInCart.Add(recipe);
            }

            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        public void Delete(Guid cartId)
        {
            Cart cart = _context.Carts
                .Include(x => x.RecipesInCart)
                .FirstOrDefault(x =>x.Id == cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }
        }

        public Cart GetCartForUser(string userId)
        {
            return _context.Carts
                .Include(x => x.User)
                .Include(x => x.RecipesInCart)
                .FirstOrDefault(x => x.UserId == userId);
        }

        public void Edit(Guid cartId, Cart cart)
        {
            Cart cartToEdit = _context.Carts
                .Include(x => x.User)
                .Include(x => x.RecipesInCart)
                .FirstOrDefault(x => x.Id == cartId);
            if (cartToEdit.Id != cart.Id)
            {
                throw new ArgumentException(ExceptionMessage);
            }

            if (cartToEdit != null)
            {
                cartToEdit.Id = cart.Id;
                cartToEdit.UserId = cart.UserId;
                cartToEdit.User = cart.User;
                cartToEdit.RecipesInCart = cart.RecipesInCart;
            }
            _context.Carts.Update(cartToEdit);
            _context.SaveChanges();
        }
    }
}
