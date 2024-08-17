using Shop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.BLL.Specifications
{
	public class OrderHeaderWithAppUserSpec : BaseSpecification<OrderHeader, int>
	{
        public OrderHeaderWithAppUserSpec() : base()
        {
            Includes.Add(o => o.ApplicationUser);
        }
        public OrderHeaderWithAppUserSpec(int orderId) : base(o => o.Id == orderId)
		{
			Includes.Add(o => o.ApplicationUser);
		}

		public OrderHeaderWithAppUserSpec(string applicationUserId) : base(o => o.ApplicationUserId == applicationUserId)
		{

		}

    }
}
