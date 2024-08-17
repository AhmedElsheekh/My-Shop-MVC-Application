using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class OrderDetailsByOrderHeaderIdWithProductSpec : BaseSpecification<OrderDetail, int>
    {
        public OrderDetailsByOrderHeaderIdWithProductSpec(int orderHeaderId) : base(o => o.OrderHeaderId == orderHeaderId)
        {
            Includes.Add(o => o.Product);
            Includes.Add(o => o.Product.Category);
        }
    }
}
