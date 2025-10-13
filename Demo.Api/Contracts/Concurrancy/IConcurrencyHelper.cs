using Demo.Api.Contracts.Auditing;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Demo.Api.Contracts.Concurrancy
{
    public interface IConcurrencyHelper
    {
        /// <summary>
        /// Retrieves the object from the DbContext by key, checking for valid concurrency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data object that is valid to be updated</returns>
        [return: NotNull]
        Task<TData> GetObjectForUpdate<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified;
        /// <summary>
        /// Retrieves the object from the DbContext by key, checking for valid concurrency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data object that is valid to be updated</returns>
        [return: NotNull]
        Task<TData> GetObjectForUpdate<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified;
        /// <summary>
        /// Checks the request's headers and throws an exception if the If-Match value does not match the ModifiedOn value on the object
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dbObject"></param>
        void CheckConcurrencyForUpdate([NotNull] ITrackModified? dbObject);
        /// <summary>
        /// Retrieves the object from the DbContext by key, checking for valid concurrency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data object that is valid to be deleted</returns>
        Task<TData?> GetObjectForDelete<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified;
        /// <summary>
        /// Retrieves the object from the DbContext by key, checking for valid concurrency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Data object that is valid to be deleted</returns>
        Task<TData?> GetObjectForDelete<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified;
        /// <summary>
        /// Checks the request's headers and throws an exception if the If-Match value does not match the ModifiedOn value on the object
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="dbObject"></param>
        void CheckConcurrencyForDelete(ITrackModified? dbObject);
        /// <summary>
        /// Soft Deletes record after checking concurrency token
        /// </summary>
        /// <typeparam name="TData">The type of the entity to delete (ITrackModified & ISoftDelete)</typeparam>
        /// <param name="key">The single or compound key</param>
        /// <param name="cancellationToken"></param>
        /// <param name="assertions"></param>
        /// <returns></returns>
        Task SoftDeleteRecord<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified, ISoftDelete;
        /// <summary>
        /// Soft Deletes record after checking concurrency token
        /// </summary>
        /// <typeparam name="TData">The type of the entity to delete (ITrackModified & ISoftDelete)</typeparam>
        /// <param name="key">The Guid primary key</param>
        /// <param name="cancellationToken"></param>
        /// <param name="assertions"></param>
        Task SoftDeleteRecord<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified, ISoftDelete;
    }
}
