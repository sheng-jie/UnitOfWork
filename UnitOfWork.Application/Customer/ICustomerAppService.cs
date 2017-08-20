namespace UnitOfWork.Customer
{
    public interface ICustomerAppService : IApplicationService
    {
        Customer GetCustomerById(int customerId);
        void CreateCustomer(Customer customer);
        void AddContactAddress(int customerId, ContactAddress address);
        ShoppingCart.ShoppingCart GetShoppingCartByCustomerId(int customerId);
    }
}