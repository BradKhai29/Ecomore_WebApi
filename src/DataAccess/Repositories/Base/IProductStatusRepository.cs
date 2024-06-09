﻿using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IProductStatusRepository :
        IGenericRepository<ProductStatusEntity>
    {
    }
}
