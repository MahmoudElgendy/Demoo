namespace Demo.Api.Contracts.Auditing;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
