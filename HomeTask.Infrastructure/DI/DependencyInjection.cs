using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HomeTask.Application.Interfaces;
using HomeTask.Application.Services;
using HomeTask.Domain.Repositories;
using HomeTask.Infrastructure.Data;
using HomeTask.Infrastructure.Repositories;
using LocalArquivoService = HomeTask.Infrastructure.Services.LocalArquivoService;

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
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                )
            )
        );

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPrestadorRepository, PrestadorRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IMensagemRepository, MensagemRepository>();
        services.AddScoped<IConversaRepository, ConversaRepository>();
        services.AddScoped<IServicoPrestadorRepository, ServicoPrestadorRepository>();
        services.AddScoped<IServicoClienteRepository, ServicoClienteRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<ICertificacaoRepository, CertificacaoRepository>();
        services.AddScoped<ICidadeRepository, CidadeRepository>();

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IPrestadorService, PrestadorService>();
        services.AddScoped<IServicoPrestadorService, ServicoPrestadorService>();
        services.AddScoped<IServicoClienteService, ServicoClienteService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();
        services.AddScoped<IAvaliacaoService, AvaliacaoService>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IMensagemService, MensagemService>();
        services.AddScoped<IArquivoService, LocalArquivoService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<ICertificacaoService, CertificacaoService>();
        services.AddScoped<ICidadeService, CidadeService>();
        services.AddScoped<IConversaService, ConversaService>();

        return services;
    }
}
