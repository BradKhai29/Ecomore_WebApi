using System;

namespace DataAccess.Entities.Base;

public abstract class GuidEntity : IGuidEntity
{
    public Guid Id { get; set; }
}
