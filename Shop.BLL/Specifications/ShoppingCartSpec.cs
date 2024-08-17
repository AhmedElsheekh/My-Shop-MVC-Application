using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class ShoppingCartSpec : BaseSpecification<ShoppingCart, int>
    {

        public ShoppingCartSpec(string userId) : base(s => s.ApplicationUserId == userId)
        {
            Includes.Add(s => s.Product);
        }

        public ShoppingCartSpec(string userId, int productId) : base(s => s.ApplicationUserId == userId && s.ProductId == productId)
        {
            Includes.Add(s => s.Product);
        }
    }
}
