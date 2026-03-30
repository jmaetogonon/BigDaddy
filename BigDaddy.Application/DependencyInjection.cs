using BigDaddy.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BigDaddy.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // ── Dispatcher ─────────────────────────────────────────────────────────
        services.AddScoped<Dispatcher>();

        // ── Auto-register all IQueryHandler<,> implementations ─────────────────
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));

        // ── Auto-register all ICommandHandler<,> implementations ───────────────
        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));

        // ── Auto-register all ICommandHandler<> implementations ────────────────
        RegisterHandlers(services, assembly, typeof(ICommandHandler<>));

        // ── FluentValidation ───────────────────────────────────────────────────
        //services.AddFluentValidationAutoValidation();
        //services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type handlerInterface)
    {
        var handlers = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces(), (impl, iface) => (impl, iface))
            .Where(x => x.iface.IsGenericType &&
                        x.iface.GetGenericTypeDefinition() == handlerInterface);

        foreach (var (impl, iface) in handlers)
            services.AddScoped(iface, impl);
    }
}