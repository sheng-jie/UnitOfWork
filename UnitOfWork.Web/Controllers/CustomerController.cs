using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UnitOfWork.Customer;

namespace UnitOfWork.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerAppService _customerAppService;

        public CustomerController(ICustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        public ContentResult Create()
        {
            var customer = new Customer.Customer() { CustomerName = "shengjie" };
            _customerAppService.CreateCustomer(customer);

            return Content($"The shopping cart number of customer is {customer.ShoppingCart.Id}");
        }
    }
}