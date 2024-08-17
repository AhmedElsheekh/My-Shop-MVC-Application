using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class ProductWithCategorySpec : BaseSpecification<Product, int>
    {
        public ProductWithCategorySpec() : base()
        {
            Includes.Add(p => p.Category);
        }

        public ProductWithCategorySpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Category);
        }

        public ProductWithCategorySpec(string productName) : base(p => p.Name.ToLower().Contains(productName.ToLower()))
        {
            Includes.Add(p => p.Category);
        }
    }
}
