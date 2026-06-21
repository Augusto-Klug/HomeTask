using HomeTask.Application.Dtos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.WebApi.Conversores;

public static class DtoConversores
{
    public static Usuario ParaEntidade(this UsuarioDto dto)
    {
        var usuario = new Usuario();
        usuario.DefinirDados(dto.Id, dto.Nome, dto.Email, dto.Documento, dto.Telefone, dto.TipoUsuario, dto.DataCadastro, dto.UltimoAcesso, dto.Ativo);

        var endereco = new Endereco();
        endereco.DefinirDados(dto.CidadeId, dto.Logradouro, dto.Numero, dto.Complemento, dto.Bairro, dto.Cep);
        usuario.DefinirEndereco(endereco);
        return usuario;
    }

    public static UsuarioDto ParaDto(this Usuario usuario) =>
        new()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Documento = usuario.Documento,
            Telefone = usuario.Telefone,
            TipoUsuario = usuario.TipoUsuario,
            DataCadastro = usuario.DataCadastro,
            UltimoAcesso = usuario.UltimoAcesso,
            Ativo = usuario.Ativo,
            Logradouro = usuario.Endereco?.Logradouro ?? string.Empty,
            Numero = usuario.Endereco?.Numero,
            Complemento = usuario.Endereco?.Complemento,
            Bairro = usuario.Endereco?.Bairro ?? string.Empty,
            Cep = usuario.Endereco?.Cep ?? string.Empty,
            CidadeId = usuario.Endereco?.CidadeId ?? Guid.Empty
        };

    public static Agendamento ParaEntidade(this AgendamentoDto dto)
    {
        var agendamento = new Agendamento();
        agendamento.DefinirDados(dto.Id, dto.ClienteId, dto.PrestadorId, dto.PrincipalServicoPrestadorId, dto.DataHoraAgendada, dto.DuracaoMinutos, dto.Status, dto.EnderecoId, dto.Observacoes, dto.ValorTotal, dto.DataSolicitacao, dto.DataResposta, dto.DataInicio, dto.DataConclusao, dto.MotivoRecusa);
        agendamento.DefinirEnderecoDescricao(dto.EnderecoDescricao);
        return agendamento;
    }

    public static AgendamentoDto ParaDto(this Agendamento agendamento) =>
        new()
        {
            Id = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            PrestadorId = agendamento.PrestadorId,
            PrincipalServicoPrestadorId = agendamento.PrincipalServicoPrestadorId,
            DataHoraAgendada = agendamento.DataHoraAgendada,
            DuracaoMinutos = agendamento.DuracaoMinutos,
            Status = agendamento.Status,
            EnderecoId = agendamento.EnderecoId,
            EnderecoDescricao = agendamento.EnderecoDescricao,
            Observacoes = agendamento.Observacoes,
            ValorTotal = agendamento.ValorTotal,
            DataSolicitacao = agendamento.DataSolicitacao,
            DataResposta = agendamento.DataResposta,
            DataInicio = agendamento.DataInicio,
            DataConclusao = agendamento.DataConclusao,
            MotivoRecusa = agendamento.MotivoRecusa,
            ServicosOferecidosIds = agendamento.AgendamentoServicos.Select(s => s.ServicoBaseId).ToList()
        };

    public static ServicoPrestador ParaEntidade(this ServicoPrestadorDto dto)
    {
        var servico = new ServicoPrestador();
        servico.DefinirDados(
            dto.Id,
            dto.PrestadorId ?? Guid.Empty,
            dto.Categoria,
            dto.Titulo,
            dto.Descricao,
            dto.PrecoBase,
            dto.UnidadeCobranca,
            dto.DuracaoEstimadaMinutos,
            dto.MediaAvaliacoes ?? 0,
            dto.TotalAvaliacoes,
            dto.Ativo,
            dto.DataCriacao == default ? DateTime.UtcNow : dto.DataCriacao);
        return servico;
    }

    public static ServicoPrestadorDto ParaDto(this ServicoPrestador servico) =>
        new()
        {
            Id = servico.Id,
            PrestadorId = servico.PrestadorId,
            Categoria = servico.Categoria,
            Titulo = servico.Titulo,
            Descricao = servico.Descricao,
            UnidadeCobranca = servico.UnidadeCobranca,
            PrecoBase = servico.PrecoBase,
            DataCriacao = servico.DataCriacao,
            DuracaoEstimadaMinutos = servico.DuracaoEstimadaMinutos,
            Ativo = servico.Ativo,
            TipoAnuncio = TipoAnuncio.Oferta,
            MediaAvaliacoes = servico.MediaAvaliacoes,
            TotalAvaliacoes = servico.TotalAvaliacoes,
            MediaAvaliacoesPrestador = servico.Prestador?.MediaAvaliacoes,
            TotalAvaliacoesPrestador = servico.Prestador?.TotalAvaliacoes
        };

    public static ServicoCliente ParaEntidade(this ServicoClienteDto dto)
    {
        var servico = new ServicoCliente();
        servico.DefinirDados(dto.Id, dto.ClienteId ?? Guid.Empty, dto.Categoria, dto.Titulo, dto.Descricao, dto.PrecoBase, dto.UnidadeCobranca, dto.DataDesejada, dto.Ativo, dto.DataCriacao == default ? DateTime.UtcNow : dto.DataCriacao);
        return servico;
    }

    public static ServicoClienteDto ParaDto(this ServicoCliente servico) =>
        new()
        {
            Id = servico.Id,
            ClienteId = servico.ClienteId,
            Categoria = servico.Categoria,
            Titulo = servico.Titulo,
            Descricao = servico.Descricao,
            UnidadeCobranca = servico.UnidadeCobranca,
            PrecoBase = servico.PrecoBase,
            DataCriacao = servico.DataCriacao,
            DataDesejada = servico.DataDesejada,
            Ativo = servico.Ativo,
            TipoAnuncio = TipoAnuncio.Pedido
        };
}
