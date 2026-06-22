using HomeTask.Application.Dtos;
using HomeTask.Application.Interfaces;
using HomeTask.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace HomeTask.Tests.Services;

public class PrestadorSuspensaoServiceTests
{
    [Fact]
    public async Task ProcessarSuspensoesExpiradasAsync_DeveDelegarParaPrestadorService()
    {
        var prestadorService = new FakePrestadorHostedService(processados: 1);
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IPrestadorService>(prestadorService)
            .BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var hostedService = new PrestadorSuspensaoService(scopeFactory, NullLogger<PrestadorSuspensaoService>.Instance);

        await hostedService.ProcessarSuspensoesExpiradasAsync(CancellationToken.None);

        Assert.Equal(1, prestadorService.ProcessarChamadas);
    }

    private sealed class FakePrestadorHostedService : IPrestadorService
    {
        private readonly int _processados;

        public FakePrestadorHostedService(int processados)
        {
            _processados = processados;
        }

        public int ProcessarChamadas { get; private set; }

        public Task<int> ProcessarSuspensoesExpiradasAsync(CancellationToken cancellationToken = default)
        {
            ProcessarChamadas++;
            return Task.FromResult(_processados);
        }

        public Task<PrestadorDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorDto?> ObterPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorDto> CriarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorDto> AtualizarAsync(PrestadorDto prestador, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<PrestadorDto>> BuscarAsync(HomeTask.Domain.Enums.CategoriaServico? categoria, string? cidade, DateTime? dataDisponivel, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<IEnumerable<AgendamentoResumoDto>> ObterHistoricoServicosAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorRecebimentosResumoDto> ObterRecebimentosAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AtualizarMediaAvaliacoesAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task AtualizarStatusAsync(Guid prestadorId, HomeTask.Domain.Enums.StatusPrestador status, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<PrestadorPerfilPublicoDto?> ObterPerfilPublicoAsync(Guid prestadorId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
