namespace Demo.Api.Data
{
    public interface IProfileAccessor
    {
        IAuthProfile? AuthProfile { get; set; }
    }
}
