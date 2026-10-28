namespace Logistics.Persistence.Seeds
{
    public interface IDataSeeder
    {
        int Order { get; }
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
