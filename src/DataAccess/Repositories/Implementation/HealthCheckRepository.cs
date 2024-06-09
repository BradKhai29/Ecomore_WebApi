using DataAccess.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class HealthCheckRepository :
        IHealthCheckRepository
    {
        private readonly DbContext _dbContext;

        public HealthCheckRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task CheckHealthAsync()
        {
            return _dbContext.Database.ExecuteSqlRawAsync("SELECT 1;");
        }
    }
}
