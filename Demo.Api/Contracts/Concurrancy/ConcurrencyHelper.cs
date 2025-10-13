using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Demo.Api.Contracts.Auditing;
using Demo.Api.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace Demo.Api.Contracts.Concurrancy;
    public class ConcurrencyHelper<TContext> : IConcurrencyHelper where TContext : DbContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TContext _dbContext;

        public ConcurrencyHelper(IHttpContextAccessor contextAccessor, TContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        [return: NotNull]
        public async Task<TData> GetObjectForUpdate<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified
        {
            var dbObject = await _dbContext.Set<TData>().FindAsync(key, cancellationToken: cancellationToken);
            if (dbObject == null)
            {
                throw new DatabaseObjectNotFoundException(typeof(TData).Name, "Id", key.ToString());
            }

            CheckAssertions(dbObject, assertions);
            CheckConcurrencyForUpdate(dbObject);

            return dbObject;
        }
        [return: NotNull]
        public async Task<TData> GetObjectForUpdate<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified
        {
            return await this.GetObjectForUpdate<TData>(new object[] { key }, cancellationToken, assertions);
        }

        public void CheckConcurrencyForUpdate([NotNull] ITrackModified? dbObject)
        {
            var concurrencyToken = _contextAccessor.HttpContext.GetIfMatchDateTimeOffset();
            _dbContext.CheckUpdateConcurrency(dbObject, concurrencyToken);
        }

        public async Task<TData?> GetObjectForDelete<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified
        {
            var dbObject = await _dbContext.Set<TData>().FindAsync(key, cancellationToken: cancellationToken);
            if (dbObject == null)
            {
                return null;
            }

            CheckAssertions(dbObject, assertions);
            CheckConcurrencyForDelete(dbObject);

            return dbObject;
        }

        public async Task<TData?> GetObjectForDelete<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified
        {
            return await this.GetObjectForDelete<TData>(new object[] { key }, cancellationToken, assertions);
        }

        public void CheckConcurrencyForDelete(ITrackModified? dbObject)
        {
            var concurrencyToken = _contextAccessor.HttpContext.GetIfMatchDateTimeOffset();
            _dbContext.CheckDeleteConcurrency(dbObject, concurrencyToken);
        }

        // example calls:
        // await _concurrencyHelper.DeleteRecord<DataObject>(new object[] { Guid.NewGuid() }, token);
        // await _concurrencyHelper.DeleteRecord<DataObject>(new object[] { Guid.NewGuid() }, token, x => x.MyProp == "myValue");
        // await _concurrencyHelper.DeleteRecord<DataObject>(new object[] { Guid.NewGuid() }, token, x => "myValue" == x.MyProp);
        public async Task SoftDeleteRecord<TData>(object[] key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified, ISoftDelete
        {
            var record = await this.GetObjectForDelete<TData>(key, cancellationToken: cancellationToken, assertions);
            if (record == null)
            {
                return;
            }

            record.IsDeleted = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // example calls:
        // await _concurrencyHelper.DeleteRecord<DataObject>(guid);
        // await _concurrencyHelper.DeleteRecord<DataObject>(guid, cancelToken);
        // await _concurrencyHelper.DeleteRecord<DataObject>(guid, cancelToken, x => x.MyFK == otherGuid);
        public async Task SoftDeleteRecord<TData>(Guid key, CancellationToken cancellationToken = default, params Expression<Func<TData, bool>>[] assertions) where TData : class, ITrackModified, ISoftDelete
        {
            await this.SoftDeleteRecord<TData>(new object[] { key }, cancellationToken: cancellationToken, assertions);
        }

        internal static void CheckAssertions<TData>(TData record, params Expression<Func<TData, bool>>[] assertions)
        {
            if (assertions == null)
            {
                return;
            }

            foreach (var expression in assertions)
            {
                var func = expression.Compile();

                if (func(record) == false)
                {
                    var body = expression.Body as BinaryExpression;
                    var name = (body?.Left as MemberExpression)?.Member.Name;
                    var value = (body?.Right as ConstantExpression)?.Value?.ToString();
                    if (name == null && value == null) // try switching left & right in case assertion was passed like `x => "myValue" == x.MyProp`
                    {
                        name = (body?.Right as MemberExpression)?.Member.Name;
                        value = (body?.Left as ConstantExpression)?.Value?.ToString();
                    }
                    if (name == null || value == null) // expression is more complex, and we will give a generic messsage i.e. `x => x.P == "p" && x.O == "o")`
                    {
                        // Message ex: `Could not find TestModel1 with matching assertion=((x.P == "p") AndAlso (x.O == "o"))`
                        throw new DatabaseObjectNotFoundException(typeof(TData).Name, "matching assertion", body?.ToString());
                    }
                    // Message ex: `Could not find TestModel1 with P="p"`
                    throw new DatabaseObjectNotFoundException(typeof(TData).Name, name ?? "Id", value);
                }
            }
        }
    }
