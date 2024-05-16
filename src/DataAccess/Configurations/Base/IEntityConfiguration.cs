using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Configurations.Base
{
    internal interface IEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity { }
}
