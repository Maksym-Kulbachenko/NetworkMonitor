using NetworkMonitor.Persistence;

namespace NetworkMonitor.Services;

public class UpdateTrafficDataService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public UpdateTrafficDataService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateTrafficData, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        return Task.CompletedTask;
    }

    private void UpdateTrafficData(object state)
    {
        using var scope = _serviceProvider.CreateScope();
        try
        {
            var overallTrafficData = scope.ServiceProvider.GetRequiredService<SshService>().GetOverallTrafficData();
            overallTrafficData.Timestamp = DateTime.UtcNow;

            var context = scope.ServiceProvider.GetRequiredService<TrafficDbContext>();
            context.NetworkStatistics.Add(overallTrafficData);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
