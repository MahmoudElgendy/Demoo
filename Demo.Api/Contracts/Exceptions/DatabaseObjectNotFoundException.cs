namespace Demo.Api.Contracts.Exceptions
{
    public class DatabaseObjectNotFoundException : Exception
    {
        public DatabaseObjectNotFoundException()
        {
        }

        public DatabaseObjectNotFoundException(string objectName, string keyName, string keyValue)
            : base("Could not find " + objectName + " with " + keyName + "=" + keyValue)
        {
        }
    }
}
