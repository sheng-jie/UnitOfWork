using System.Linq;

namespace UnitOfWork.Customer
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<ShoppingCart.ShoppingCart> _shoppingCartRepository;

        public CustomerAppService(IRepository<ShoppingCart.ShoppingCart> shoppingCartRepository, IRepository<Customer> customerRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
        }

        public Customer GetCustomerById(int customerId)
        {
            return _customerRepository.FirstOrDefault(c => c.Id == customerId);
        }

        public void CreateCustomer(Customer customer)
        {
            _customerRepository.Insert(customer);
            var cart = new ShoppingCart.ShoppingCart() {CustomerId = customer.Id};
            _shoppingCartRepository.Insert(cart);

        }


        public void AddContactAddress(int customerId, ContactAddress address)
        {
            var customer = GetCustomerById(customerId);
            customer.ShippingAddresses.Add(address);
            _customerRepository.Update(customer);
        }

        public ShoppingCart.ShoppingCart GetShoppingCartByCustomerId(int customerId)
        {
            return _shoppingCartRepository.GetAll().FirstOrDefault(sc => sc.Customer.Id == customerId);
        }
    }
}