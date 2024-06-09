using Microsoft.Extensions.Options;

namespace NetworkMonitor.Extensions;

public static class IocExtensions
{
    public static void AddConfiguration<TInterface, T>(this IServiceCollection services, string? configurationName = null)
        where T : class, TInterface
    {
        AddConfiguration<T>(services, configurationName);
        RegisterByType<TInterface, T>(services);
    }

    public static void AddConfiguration<T>(this IServiceCollection services, string? configurationName = null)
        where T : class
    {
        services.AddOptions<T>()
                .BindConfiguration(configurationName ?? typeof(T).Name)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        RegisterByType<T, T>(services);
    }

    private static void RegisterByType<TInterface, T>(IServiceCollection services)
        where T : class, TInterface
    {
        services.AddSingleton(typeof(TInterface), provider => provider.GetRequiredService<IOptions<T>>().Value);
    }
}
