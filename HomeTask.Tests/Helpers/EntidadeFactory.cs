using System.Reflection;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Tests.Helpers;

internal static class EntidadeFactory
{
    public static Usuario CriarUsuario(Guid? id = null, string nome = "Usuario Teste", TipoUsuario tipo = TipoUsuario.Cliente, string documento = "12345678900")
    {
        var usuario = new Usuario();
        usuario.DefinirDados(id ?? Guid.NewGuid(), nome, $"{Guid.NewGuid():N}@teste.com", documento, "47999999999", tipo, DateTime.UtcNow, null, true);
        usuario.DefinirSenhaHash("hash");
        return usuario;
    }

    public static Cliente CriarCliente(Guid? id = null, Guid? usuarioId = null)
    {
        var cliente = new Cliente();
        cliente.DefinirDados(id ?? Guid.NewGuid(), usuarioId ?? Guid.NewGuid());
        return cliente;
    }

    public static Prestador CriarPrestador(Guid? id = null, Guid? usuarioId = null, StatusPrestador status = StatusPrestador.Ativo, decimal media = 0)
    {
        var prestador = new Prestador();
        prestador.DefinirDados(id ?? Guid.NewGuid(), usuarioId ?? Guid.NewGuid(), "Prestador experiente", 10, status, media, 0, 0, null);
        return prestador;
    }

    public static ServicoPrestador CriarServicoPrestador(Guid? id = null, Guid? prestadorId = null, CategoriaServico categoria = CategoriaServico.Faxina, decimal preco = 100, DateTime? dataCriacao = null, bool ativo = true)
    {
        var servico = new ServicoPrestador();
        servico.DefinirDados(id ?? Guid.NewGuid(), prestadorId ?? Guid.NewGuid(), categoria, "Faxina completa", "Servico de faxina", preco, FormatoCobranca.Total, 120, true, 0, 0, ativo, dataCriacao ?? DateTime.UtcNow);
        return servico;
    }

    public static ServicoCliente CriarServicoCliente(Guid? id = null, Guid? clienteId = null, CategoriaServico categoria = CategoriaServico.Jardinagem, decimal preco = 80, DateTime? dataCriacao = null, bool ativo = true)
    {
        var servico = new ServicoCliente();
        servico.DefinirDados(id ?? Guid.NewGuid(), clienteId ?? Guid.NewGuid(), categoria, "Preciso de jardineiro", "Cortar grama", preco, FormatoCobranca.Total, DateTime.UtcNow.AddDays(2), ativo, dataCriacao ?? DateTime.UtcNow);
        return servico;
    }

    public static Agendamento CriarAgendamento(Guid? id = null, Guid? clienteId = null, Guid? prestadorId = null, Guid? enderecoId = null, Guid? principalServicoPrestadorId = null, StatusAgendamento status = StatusAgendamento.Solicitado)
    {
        var agendamento = new Agendamento();
        agendamento.DefinirDados(id ?? Guid.NewGuid(), clienteId ?? Guid.NewGuid(), prestadorId ?? Guid.NewGuid(), principalServicoPrestadorId, DateTime.UtcNow.AddDays(1), 60, status, enderecoId ?? Guid.NewGuid(), "Observacao", 150, DateTime.UtcNow, null, null, null, null);
        return agendamento;
    }

    public static Pagamento CriarPagamento(Guid? id = null, Guid? agendamentoId = null, StatusPagamento status = StatusPagamento.Pendente, decimal valor = 150)
    {
        var pagamento = new Pagamento();
        pagamento.DefinirDados(id ?? Guid.NewGuid(), agendamentoId ?? Guid.NewGuid(), valor, TipoPagamento.Pix, status, null, null, null, null, null, DateTime.UtcNow, null, null, null);
        return pagamento;
    }

    public static AgendamentoServico CriarAgendamentoServico(Guid agendamentoId, Guid servicoId, decimal valor)
    {
        var agendamentoServico = new AgendamentoServico();
        agendamentoServico.DefinirDados(agendamentoId, servicoId, 1, valor);
        return agendamentoServico;
    }

    public static Cidade CriarCidade(Guid? id = null, string nome = "Blumenau", string estado = "SC")
    {
        var cidade = new Cidade();
        Set(cidade, nameof(Cidade.Id), id ?? Guid.NewGuid());
        Set(cidade, nameof(Cidade.Nome), nome);
        Set(cidade, nameof(Cidade.Estado), estado);
        return cidade;
    }

    public static Endereco CriarEndereco(Guid? id = null, Guid? usuarioId = null, Guid? cidadeId = null, string logradouro = "Rua XV", string bairro = "Centro")
    {
        var endereco = new Endereco();
        endereco.DefinirDados(cidadeId ?? Guid.NewGuid(), logradouro, "100", null, bairro, "89010000");
        endereco.DefinirUsuarioId(usuarioId ?? Guid.NewGuid());
        Set(endereco, nameof(Endereco.Id), id ?? Guid.NewGuid());
        return endereco;
    }

    public static void DefinirNavegacao<T>(T entidade, string propriedade, object? valor)
    {
        Set(entidade!, propriedade, valor);
    }

    private static void Set(object alvo, string propriedade, object? valor)
    {
        alvo.GetType().GetProperty(propriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(alvo, valor);
    }
}
