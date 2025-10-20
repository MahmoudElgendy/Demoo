namespace Demo.Api.Middlewares.Exceptions
{
    public class UniqueKeyException : Exception
    {
        public UniqueKeyException() : base("Key shold be unique")
        {
        }
    }
}
