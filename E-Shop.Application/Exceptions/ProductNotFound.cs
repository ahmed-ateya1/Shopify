namespace E_Shop.Application.Exceptions
{
    public class ProductNotFound : Exception
    {
        public ProductNotFound(string message) : base(message)
        {
        }
    }
}
