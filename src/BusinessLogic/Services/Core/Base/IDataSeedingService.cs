namespace BusinessLogic.Services.Core.Base
{
    public interface IDataSeedingService
    {
        Task<bool> SeedAsync(CancellationToken cancellationToken);
    }
}
