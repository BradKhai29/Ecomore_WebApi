using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface ICategoryRepository :
        IGenericRepository<CategoryEntity>
    {
    }
}
