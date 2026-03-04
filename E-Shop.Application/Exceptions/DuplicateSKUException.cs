namespace E_Shop.Application.Exceptions
{
    public class DuplicateSKUException : Exception
    {
        public DuplicateSKUException(string msg) : base(msg)
        {
        }
    }
}
