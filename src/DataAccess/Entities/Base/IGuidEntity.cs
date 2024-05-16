using System;

namespace DataAccess.Entities.Base
{
    public interface IGuidEntity : IEntity
    {
        Guid Id { get; }
    }
}
