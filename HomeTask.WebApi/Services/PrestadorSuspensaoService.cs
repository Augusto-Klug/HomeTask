using HomeTask.Application.Interfaces;

namespace HomeTask.WebApi.Services;

public class PrestadorSuspensaoService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromDays(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PrestadorSuspensaoService> _logger;

    public PrestadorSuspensaoService(IServiceScopeFactory scopeFactory, ILogger<PrestadorSuspensaoService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessarSuspensoesExpiradasAsync(stoppingToken);
            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    public async Task ProcessarSuspensoesExpiradasAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var prestadorService = scope.ServiceProvider.GetRequiredService<IPrestadorService>();
            var reativados = await prestadorService.ProcessarSuspensoesExpiradasAsync(cancellationToken);
            if (reativados > 0)
                _logger.LogInformation("Reativados {Quantidade} prestadores com suspensao expirada.", reativados);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reativar prestadores com suspensao expirada.");
        }
    }
}
