using System;

namespace DataAccess.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the update must inherit from.
/// </summary>
public interface IUpdatedEntity
{
    /// <summary>
    ///     The time when the entity is updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     Id of the updater.
    /// </summary>
    Guid UpdatedBy { get; set; }
}
