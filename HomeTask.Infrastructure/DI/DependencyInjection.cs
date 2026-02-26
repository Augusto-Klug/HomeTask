using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HomeTask.Application.Interfaces;
using HomeTask.Infrastructure.Data;
using HomeTask.Infrastructure.Services;

namespace HomeTask.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");

        services.AddDbContext<HomeTaskDbContext>(options =>
            options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
            )
        )
    );

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPrestadorService, PrestadorService>();
        services.AddScoped<IServicoService, ServicoService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();
        services.AddScoped<IAvaliacaoService, AvaliacaoService>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IMensagemService, MensagemService>();

        return services;
    }
}
