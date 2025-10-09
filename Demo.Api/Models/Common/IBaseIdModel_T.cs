namespace Demo.Api.Models.Common;

public interface IBaseIdModel<T> : ISoftDelete, ITenantData
{
    T Id { get; set; }
}