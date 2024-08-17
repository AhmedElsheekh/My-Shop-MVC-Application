using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class ProductByCategoryNameSpec : BaseSpecification<Product, int>
    {
        public ProductByCategoryNameSpec(string categoryName) : base(p => p.Category.Name.ToLower() == categoryName.ToLower())
        {
            Includes.Add(p => p.Category);
        }
    }
}
