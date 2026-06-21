using HomeTask.Application.Interfaces;

namespace HomeTask.WebApi.Services;

public class MensagemRetentionService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromDays(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MensagemRetentionService> _logger;

    public MensagemRetentionService(IServiceScopeFactory scopeFactory, ILogger<MensagemRetentionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RemoverMensagensExpiradasAsync(stoppingToken);
            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task RemoverMensagensExpiradasAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mensagemService = scope.ServiceProvider.GetRequiredService<IMensagemService>();
            var removidas = await mensagemService.RemoverExpiradasAsync(cancellationToken);
            if (removidas > 0)
                _logger.LogInformation("Removidas {Quantidade} mensagens expiradas do chat.", removidas);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover mensagens expiradas do chat.");
        }
    }
}
