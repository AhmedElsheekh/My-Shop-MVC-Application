using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class CategoryByNameSpec : BaseSpecification<Category, int>
    {
        public CategoryByNameSpec(string categoryName) : base(c => c.Name.ToLower().Contains(categoryName.ToLower()))
        {

        }
    }
}
