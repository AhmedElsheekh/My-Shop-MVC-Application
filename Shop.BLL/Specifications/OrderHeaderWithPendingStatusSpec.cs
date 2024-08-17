using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
    public class OrderHeaderWithPendingStatusSpec : BaseSpecification<OrderHeader, int>
    {
        public OrderHeaderWithPendingStatusSpec() : base(o => o.OrderStatus == "Pending")
        {

        }
    }
}
