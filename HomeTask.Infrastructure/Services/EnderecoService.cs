using Microsoft.EntityFrameworkCore;
using HomeTask.Application.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Infrastructure.Data;

namespace HomeTask.Infrastructure.Services;

public class EnderecoService : IEnderecoService
{
    private readonly HomeTaskDbContext _context;

    public EnderecoService(HomeTaskDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Endereco>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Enderecos
            .Include(e => e.Cidade)
            .Where(e => e.UsuarioId == usuarioId)
            .OrderByDescending(e => e.Principal)
            .ThenBy(e => e.Logradouro)
            .ToListAsync(cancellationToken);
    }

    public async Task<Endereco?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Enderecos
            .Include(e => e.Cidade)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Endereco> CriarAsync(Endereco endereco, CancellationToken cancellationToken = default)
    {
        // se for o primeiro endereço do usuário, define como principal automaticamente
        var temEnderecos = await _context.Enderecos
            .AnyAsync(e => e.UsuarioId == endereco.UsuarioId, cancellationToken);

        if (!temEnderecos)
            endereco.DefinirPrincipal(true);

        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync(cancellationToken);
        return endereco;
    }

    public async Task<Endereco> AtualizarAsync(Endereco endereco, CancellationToken cancellationToken = default)
    {
        _context.Enderecos.Update(endereco);
        await _context.SaveChangesAsync(cancellationToken);
        return endereco;
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endereco = await _context.Enderecos.FindAsync([id], cancellationToken);
        if (endereco != null)
        {
            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync(cancellationToken);

            // se era o principal, define o próximo como principal
            if (endereco.Principal)
            {
                var proximo = await _context.Enderecos
                    .FirstOrDefaultAsync(e => e.UsuarioId == endereco.UsuarioId, cancellationToken);

                if (proximo != null)
                {
                    proximo.DefinirPrincipal(true);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public async Task DefinirPrincipalAsync(Guid enderecoId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        // desmarca todos os endereços do usuário
        var enderecos = await _context.Enderecos
            .Where(e => e.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);

        foreach (var e in enderecos)
            e.DefinirPrincipal(false);

        // marca o selecionado como principal
        var principal = enderecos.FirstOrDefault(e => e.Id == enderecoId);
        if (principal != null)
            principal.DefinirPrincipal(true);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
