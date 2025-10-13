using System.Runtime.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Update;

namespace Demo.Api.Contracts.Exceptions;

public class ObjectNotFoundException : DbUpdateException
{
    public ObjectNotFoundException()
    {
    }

    public ObjectNotFoundException(string message) : base(message)
    {
    }

    public ObjectNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public ObjectNotFoundException(string message, IReadOnlyList<IUpdateEntry> entries) : base(message, entries)
    {
    }

    public ObjectNotFoundException(string message, IReadOnlyList<EntityEntry> entries) : base(message, entries)
    {
    }

    public ObjectNotFoundException(string message, Exception? innerException, IReadOnlyList<IUpdateEntry> entries) : base(message, innerException, entries)
    {
    }

    public ObjectNotFoundException(string message, Exception? innerException, IReadOnlyList<EntityEntry> entries) : base(message, innerException, entries)
    {
    }

    public static ObjectNotFoundException ForEntry<TEntity>(EntityEntry<TEntity>? entry, Exception? innerException = null) where TEntity : class
    {
        var list = new List<EntityEntry>();
        if (entry != null) { list.Add(entry); }
        if (innerException is null)
            return new ObjectNotFoundException(GenerateEntryMessage(entry, typeof(TEntity)), list);
        else
            return new ObjectNotFoundException(GenerateEntryMessage(entry, typeof(TEntity)), innerException, list);

    }

    private static string GenerateEntryMessage(EntityEntry? entry, Type type)
    {
        //we sent an update and couldnt find the record. Assume delete? 
        var typename = type is not null ?
            type.Name :
            entry?.Entity is not null ?
                entry.Entity.GetType().Name :
                "Unknown";
        var message = $"Entity ({typename}) was not found.";
        if (entry != null)
        {
            message += $" Key: {GetPrimaryKeyValues(entry)}";
        }
        return message;
    }

    private static string GetPrimaryKeyValues(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var pk = entry.Metadata.FindPrimaryKey();
        if (pk != null && pk.Properties.Any())
        {
            return string.Join(',', pk.Properties
                .Select(p => entry.Property(p)!.CurrentValue?.ToString())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray());
        }
        return string.Empty;
    }
}
