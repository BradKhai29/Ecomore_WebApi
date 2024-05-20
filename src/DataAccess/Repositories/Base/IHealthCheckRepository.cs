namespace DataAccess.Repositories.Base
{
    public interface IHealthCheckRepository
    {
        Task CheckHealthAsync();
    }
}
