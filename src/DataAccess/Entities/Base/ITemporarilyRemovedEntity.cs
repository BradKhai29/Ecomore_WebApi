using System;

namespace DataAccess.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the temporarily removed must inherit from.
/// </summary>
public interface ITemporarilyRemovedEntity
{
    /// <summary>
    ///     The time when the entity is temporarily removed.
    /// </summary>
    DateTime RemovedAt { get; set; }

    /// <summary>
    ///     Id of the entity remover.
    /// </summary>
    Guid RemovedBy { get; set; }
}
