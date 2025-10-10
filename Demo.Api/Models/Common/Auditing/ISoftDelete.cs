namespace Demo.Api.Models.Common.Auditing;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
