namespace Demo.Api.Contracts.Exceptions
{
    public class UpdateSystemDataException : Exception
    {
        public UpdateSystemDataException()
            : base("System data is not allowed to be updated.")
        {
        }

        public UpdateSystemDataException(string message)
            : base(message)
        {
        }
    }
}
