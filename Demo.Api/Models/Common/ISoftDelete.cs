namespace Demo.Api.Models.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
