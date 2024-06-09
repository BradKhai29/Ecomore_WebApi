using System;

namespace DataAccess.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the creation must inherit from.
/// </summary>
public interface ICreatedEntity
{
    /// <summary>
    ///     The time when the entity is created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Id of the creator.
    /// </summary>
    Guid CreatedBy { get; set; }
}
