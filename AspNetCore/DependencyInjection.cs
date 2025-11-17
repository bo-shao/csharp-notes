// https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0
// IServiceProvider vs IServiceScopeFactory
// GetService vs GetRequiredService

public sealed class TransientDisposable : IDisposable
{
    public void Dispose() => Console.WriteLine($"{nameof(TransientDisposable)}.Dispose()");
}

public sealed class ScopedDisposable : IDisposable
{
    public void Dispose() => Console.WriteLine($"{nameof(ScopedDisposable)}.Dispose()");
}

public sealed class SingletonDisposable : IDisposable
{
    public void Dispose() => Console.WriteLine($"{nameof(SingletonDisposable)}.Dispose()");
}

public sealed class UserCreatedSingletonDisposable : IDisposable
{
    private UserCreatedSingletonDisposable() { }

    public static UserCreatedSingletonDisposable Instance => new();

    public void Dispose() => Console.WriteLine($"{nameof(UserCreatedSingletonDisposable)}.Dispose()");
}

public static class DependencyInjectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<TransientDisposable>();
        services.AddScoped<ScopedDisposable>();
        services.AddSingleton<SingletonDisposable>();

        // The ExampleService instance is not created by the service container.
        // The framework does not dispose of the services automatically.
        // The developer is responsible for disposing the services.
        // services.AddSingleton(UserCreatedSingletonDisposable.Instance);
        services.AddSingleton<UserCreatedSingletonDisposable>(_ => UserCreatedSingletonDisposable.Instance);
    }

    public static void Configure(this IServiceProvider services)
    {
        ExemplifyDisposableScoping(services, "Scope 1");
        Console.WriteLine();

        ExemplifyDisposableScoping(services, "Scope 2");
        Console.WriteLine();
    }

    private static void ExemplifyDisposableScoping(IServiceProvider services, string scope)
    {
        Console.WriteLine($"{scope}...");

        using IServiceScope serviceScope = services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        _ = provider.GetRequiredService<TransientDisposable>();
        _ = provider.GetRequiredService<ScopedDisposable>();
        _ = provider.GetRequiredService<SingletonDisposable>();
        _ = provider.GetRequiredService<UserCreatedSingletonDisposable>();
    }
}