using System;
using System.Collections.Generic;

namespace UnitOfWork.Customer
{
    public class Customer : AggregateRoot
    {
        public Customer()
        {
            RegisterDate = DateTime.Now;
        }
        public string CustomerName { get; set; }
        
        public virtual ShoppingCart.ShoppingCart ShoppingCart { get; set; }

        public virtual List<ContactAddress> ShippingAddresses { get; set; }

        public DateTime RegisterDate { get; }
    }
}