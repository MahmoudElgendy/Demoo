using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Demo.Api.Contracts.Auditing;
using Demo.Api.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace Demo.Api.Contracts.Concurrancy;

public static class ConcurrencyExt
{
    #region DbContext extensions
    public static void CheckConcurrencyValue<TEntity>(this DbContext context, [NotNull] TEntity? entity, string checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);

        if (entity == null || (entity is ISoftDelete sd && sd.IsDeleted))
        {
            var entry = entity is null ? null : context.Entry(entity);
            throw ObjectNotFoundException.ForEntry(entry);
        }

        CheckConcurrencyValue(context, entity, FromString(checkValue));
    }

    public static void CheckConcurrencyValue<TEntity>(this DbContext context, TEntity entity, DateTimeOffset? checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(entity);

        if (checkValue.HasValue && entity.ModifiedOn != checkValue)
        {
            throw CreateDbUpdateConcurrencyException(context, entity);
        }
    }

    public static void CheckUpdateConcurrency<TEntity, TDto>(this DbContext context, [NotNull] TEntity? entity, TDto dto)
        where TEntity : class, ITrackModified
        where TDto : ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(dto);

        context.CheckUpdateConcurrency(entity, dto.ModifiedOn);
    }

    public static void CheckUpdateConcurrency<TEntity>(this DbContext context, [NotNull] TEntity? entity, string checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        context.CheckUpdateConcurrency(entity, FromString(checkValue));
    }

    public static void CheckUpdateConcurrency<TEntity>(this DbContext context, [NotNull] TEntity? entity, DateTimeOffset? checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);

        if (entity == null || (entity is ISoftDelete sd && sd.IsDeleted))
        {
            var entry = entity is null ? null : context.Entry(entity);
            throw ObjectNotFoundException.ForEntry(entry);
        }

        context.CheckConcurrencyValue(entity, checkValue);
    }

    public static void CheckDeleteConcurrency<TEntity, TDto>(this DbContext context, TEntity? entity, TDto dto)
        where TEntity : class, ITrackModified
        where TDto : ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(dto);
        context.CheckDeleteConcurrency(entity, dto.ModifiedOn);
    }

    public static void CheckDeleteConcurrency<TEntity>(this DbContext context, TEntity? entity, string checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        context.CheckDeleteConcurrency(entity, FromString(checkValue));
    }

    public static void CheckDeleteConcurrency<TEntity>(this DbContext context, TEntity? entity, DateTimeOffset? checkValue)
        where TEntity : class, ITrackModified
    {
        ArgumentNullException.ThrowIfNull(context);
        if (entity != null)
        {
            context.CheckConcurrencyValue(entity, checkValue);
        }
    }

    public static void CheckConcurrencyToken<TEntity>(this DbContext context, TEntity entity, object checkValue)
        where TEntity : class
    {
        var entry = context.Entry(entity);
        IEntityType entityType = entry.Metadata;

        var prop = entityType.GetProperties()
            .Where(p => p.IsConcurrencyToken)
            .FirstOrDefault();

        if (prop == null)
        {
            throw new InvalidOperationException("Entity does not have ConncurrencyToken");
        }

        var currentValue = entry.Property(prop.Name).CurrentValue;
        if (currentValue is null && checkValue is null)
        {
            return;
        }
        if ((currentValue is null && checkValue is not null) || !currentValue!.Equals(checkValue))
        {
            throw CreateDbUpdateConcurrencyException(context, entity);
        }
    }

    private static DateTimeOffset? FromString(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default;
        else if (DateTimeOffset.TryParse(value, out var dtoffset))
            return dtoffset;
        else
            throw new ArgumentException($"'{nameof(value)}' is not parsable to DateTimeOffset.", nameof(value));
    }

    private static DbUpdateConcurrencyException CreateDbUpdateConcurrencyException<T>(DbContext context, T entity)  where T : class
    {
#pragma warning disable EF1001 // Internal EF Core API usage.
        var entry = context.GetDependencies().StateManager.GetOrCreateEntry(entity);
        entry.SetEntityState(EntityState.Modified);
        var entries = new ReadOnlyCollection<IUpdateEntry>(new List<IUpdateEntry>() { entry });
#pragma warning restore EF1001 // Internal EF Core API usage.
        return new DbUpdateConcurrencyException($"Your changes cannot be saved. Another user modified the {entity.GetType().Name}. Refresh the screen to see the latest data.", entries);
    }


    public static bool IsEntryModified<T, TProperty1>(this DbContext context, T obj, Expression<Func<T, TProperty1>> prop1) where T : class
    {
        var prop = context.Entry<T>(obj).Property(prop1);
        return !EqualityComparer<TProperty1>.Default.Equals(prop.CurrentValue, prop.OriginalValue);
    }
    #endregion DbContext extensions



    #region HttpContext / HttpRequest extensions
    public static DateTimeOffset? GetIfMatchDateTimeOffset(this HttpContext context)
    {
        return context?.Request?.GetIfMatchDateTimeOffset();
    }

    public static DateTimeOffset? GetIfMatchDateTimeOffset(this HttpRequest request)
    {
        string? headerValue = request?.GetIfMatch();
        return DateTimeOffset.TryParse(headerValue, out var result) ? result : null;
    }

    public static string? GetIfMatch(this HttpContext context)
    {
        return context?.Request?.GetIfMatch();
    }

    public static string? GetIfMatch(this HttpRequest request)
    {
        if (request.Headers.ContainsKey("If-Match"))
            return request.Headers["If-Match"];

        return null;
    }
    #endregion HttpContext / HttpRequest extensions
}
