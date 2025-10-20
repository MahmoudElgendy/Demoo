namespace Demo.Api.Middlewares.Exceptions
{
    public class PasswordRestrictionException : Exception
    {
        public PasswordRestrictionException()
        : base("Password isn't strong enough")
        {
        }
    }
}
