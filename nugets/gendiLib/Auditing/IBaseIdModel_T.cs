namespace gendiLib.Auditing;

public interface IBaseIdModel<T> : ISoftDelete, ITenantData
{
    T Id { get; set; }
}