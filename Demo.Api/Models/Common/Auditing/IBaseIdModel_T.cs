namespace Demo.Api.Models.Common.Auditing;

public interface IBaseIdModel<T> : ISoftDelete, ITenantData
{
    T Id { get; set; }
}