using HomeTask.Application.Dtos;
using HomeTask.Domain.Common;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;

namespace HomeTask.Application.Mappings;

internal static class EntityDtoMapper
{
    public static UsuarioDto ParaDto(this Usuario usuario)
    {
        var endereco = usuario.Endereco;

        return new UsuarioDto
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
            Logradouro = endereco?.Logradouro ?? string.Empty,
            Numero = endereco?.Numero,
            Complemento = endereco?.Complemento,
            Bairro = endereco?.Bairro ?? string.Empty,
            Cep = endereco?.Cep ?? string.Empty,
            CidadeId = endereco?.CidadeId ?? Guid.Empty
        };
    }

    public static Usuario ParaEntidade(this UsuarioDto dto)
    {
        var usuario = new Usuario();
        usuario.DefinirDados(
            dto.Id,
            dto.Nome,
            dto.Email,
            dto.Documento,
            dto.Telefone,
            dto.TipoUsuario,
            dto.DataCadastro,
            dto.UltimoAcesso,
            dto.Ativo);

        var endereco = new Endereco();
        endereco.DefinirDados(dto.CidadeId, dto.Logradouro, dto.Numero, dto.Complemento, dto.Bairro, dto.Cep);
        usuario.DefinirEndereco(endereco);

        return usuario;
    }

    public static PerfilDto ParaPerfilDto(this Usuario usuario, Prestador? prestador = null)
    {
        var perfil = new PerfilDto
        {
            Nome = usuario.Nome,
            Tipo = (int)usuario.TipoUsuario,
            Email = usuario.Email,
            Documento = usuario.Documento,
            Telefone = usuario.Telefone
        };

        var endereco = usuario.Endereco;
        if (endereco != null)
        {
            perfil.Cep = endereco.Cep;
            perfil.Logradouro = endereco.Logradouro;
            perfil.Bairro = endereco.Bairro;
            perfil.Cidade = endereco.Cidade?.Nome;
            perfil.Estado = endereco.Cidade?.Estado;
        }

        var dadosPrestador = prestador ?? usuario.Prestador;
        if (dadosPrestador != null)
        {
            perfil.Descricao = dadosPrestador.Descricao;
            perfil.RaioAtendimentoKm = dadosPrestador.RaioAtendimentoKm;
            perfil.Status = dadosPrestador.Status;
            perfil.MediaAvaliacoes = dadosPrestador.MediaAvaliacoes;
            perfil.TotalAvaliacoes = dadosPrestador.TotalAvaliacoes;
            perfil.TotalServicosConcluidos = dadosPrestador.TotalServicosConcluidos;
            perfil.Certificacoes = dadosPrestador.Certificacoes.Select(c => c.ParaDto()).ToList();
            perfil.Portfolios = dadosPrestador.Portfolios.Select(p => p.ParaDto()).ToList();
        }

        return perfil;
    }

    public static ClienteDto ParaDto(this Cliente cliente) =>
        new()
        {
            Id = cliente.Id,
            UsuarioId = cliente.UsuarioId
        };

    public static Cliente ParaEntidade(this ClienteDto dto)
    {
        var cliente = new Cliente();
        cliente.DefinirDados(dto.Id, dto.UsuarioId);
        return cliente;
    }

    public static PrestadorDto ParaDto(this Prestador prestador) =>
        new()
        {
            Id = prestador.Id,
            UsuarioId = prestador.UsuarioId,
            Descricao = prestador.Descricao,
            RaioAtendimentoKm = prestador.RaioAtendimentoKm,
            Status = prestador.Status,
            MediaAvaliacoes = prestador.MediaAvaliacoes,
            TotalAvaliacoes = prestador.TotalAvaliacoes,
            TotalServicosConcluidos = prestador.TotalServicosConcluidos,
            DataVerificacao = prestador.DataVerificacao
        };

    public static Prestador ParaEntidade(this PrestadorDto dto)
    {
        var prestador = new Prestador();
        prestador.DefinirDados(
            dto.Id,
            dto.UsuarioId,
            dto.Descricao,
            dto.RaioAtendimentoKm,
            dto.Status,
            dto.MediaAvaliacoes,
            dto.TotalAvaliacoes,
            dto.TotalServicosConcluidos,
            dto.DataVerificacao);
        return prestador;
    }

    public static CidadeDto ParaDto(this Cidade cidade) =>
        new()
        {
            Id = cidade.Id,
            Nome = cidade.Nome,
            Estado = cidade.Estado,
            CodIBGE = cidade.CodIBGE
        };

    public static AgendamentoDto ParaDto(this Agendamento agendamento) =>
        new()
        {
            Id = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            PrestadorId = agendamento.PrestadorId,
            ServicosOferecidosIds = agendamento.AgendamentoServicos.Select(s => s.ServicoBaseId).ToList(),
            DataHoraAgendada = agendamento.DataHoraAgendada,
            DuracaoMinutos = agendamento.DuracaoMinutos,
            Status = agendamento.Status,
            EnderecoId = agendamento.EnderecoId,
            Observacoes = agendamento.Observacoes,
            ValorTotal = agendamento.ValorTotal,
            DataSolicitacao = agendamento.DataSolicitacao,
            DataResposta = agendamento.DataResposta,
            DataConclusao = agendamento.DataConclusao,
            MotivoRecusa = agendamento.MotivoRecusa
        };

    public static Agendamento ParaEntidade(this AgendamentoDto dto)
    {
        var agendamento = new Agendamento();
        agendamento.DefinirDados(
            dto.Id,
            dto.ClienteId,
            dto.PrestadorId,
            dto.DataHoraAgendada,
            dto.DuracaoMinutos,
            dto.Status,
            dto.EnderecoId,
            dto.Observacoes,
            dto.ValorTotal,
            dto.DataSolicitacao,
            dto.DataResposta,
            dto.DataConclusao,
            dto.MotivoRecusa);
        return agendamento;
    }

    public static AgendamentoResumoDto ParaResumoDto(this Agendamento agendamento) =>
        new()
        {
            Id = agendamento.Id,
            ClienteId = agendamento.ClienteId,
            ClienteNome = agendamento.Cliente?.Usuario?.Nome ?? string.Empty,
            PrestadorId = agendamento.PrestadorId,
            PrestadorNome = agendamento.Prestador?.Usuario?.Nome ?? string.Empty,
            DataHoraAgendada = agendamento.DataHoraAgendada,
            DuracaoMinutos = agendamento.DuracaoMinutos,
            Status = agendamento.Status,
            Observacoes = agendamento.Observacoes,
            ValorTotal = agendamento.ValorTotal,
            DataSolicitacao = agendamento.DataSolicitacao,
            DataResposta = agendamento.DataResposta,
            DataConclusao = agendamento.DataConclusao,
            MotivoRecusa = agendamento.MotivoRecusa,
            AguardandoRespostaDe = Agendamento.ObterResponsavelPelaResposta(agendamento),
            Endereco = new EnderecoResumoDto
            {
                Logradouro = agendamento.Endereco?.Logradouro ?? string.Empty,
                Bairro = agendamento.Endereco?.Bairro ?? string.Empty,
                Cidade = agendamento.Endereco?.Cidade?.Nome ?? string.Empty,
                Estado = agendamento.Endereco?.Cidade?.Estado ?? string.Empty
            },
            Servicos = agendamento.AgendamentoServicos.Select(s => new ServicoResumoDto
            {
                Id = s.ServicoBaseId,
                Titulo = s.ServicoBase?.Titulo ?? string.Empty,
                PrecoBase = s.ValorUnitario,
                UnidadeCobranca = s.ServicoBase?.UnidadeCobranca ?? FormatoCobranca.Total
            }).ToList()
        };



    public static AvaliacaoDto ParaDto(this Avaliacao avaliacao) =>
        new()
        {
            Id = avaliacao.Id,
            AgendamentoId = avaliacao.AgendamentoId,
            ClienteId = avaliacao.ClienteId,
            PrestadorId = avaliacao.PrestadorId,
            Nota = avaliacao.Nota,
            Comentario = avaliacao.Comentario,
            DataAvaliacao = avaliacao.DataAvaliacao,
            Visivel = avaliacao.Visivel
        };

    public static Avaliacao ParaEntidade(this AvaliacaoDto dto)
    {
        var avaliacao = new Avaliacao();
        avaliacao.DefinirDados(
            dto.Id,
            dto.AgendamentoId,
            dto.ClienteId,
            dto.PrestadorId,
            dto.Nota,
            dto.Comentario,
            dto.DataAvaliacao,
            dto.Visivel);
        return avaliacao;
    }

    public static PagamentoDto ParaDto(this Pagamento pagamento) =>
        new()
        {
            Id = pagamento.Id,
            AgendamentoId = pagamento.AgendamentoId,
            Valor = pagamento.Valor,
            TipoPagamento = pagamento.TipoPagamento,
            Status = pagamento.Status,
            TransacaoId = pagamento.TransacaoId,
            DataCriacao = pagamento.DataCriacao,
            DataProcessamento = pagamento.DataProcessamento,
            DataConfirmacao = pagamento.DataConfirmacao,
            MotivoRecusa = pagamento.MotivoRecusa
        };

    public static Pagamento ParaEntidade(this PagamentoDto dto)
    {
        var pagamento = new Pagamento();
        pagamento.DefinirDados(
            dto.Id,
            dto.AgendamentoId,
            dto.Valor,
            dto.TipoPagamento,
            dto.Status,
            dto.TransacaoId,
            dto.DataCriacao,
            dto.DataProcessamento,
            dto.DataConfirmacao,
            dto.MotivoRecusa);
        return pagamento;
    }

    public static MensagemDto ParaDto(this Mensagem mensagem) =>
        new()
        {
            Id = mensagem.Id,
            RemetenteId = mensagem.RemetenteId,
            ConversaId = mensagem.ConversaId,
            AgendamentoId = mensagem.AgendamentoId,
            Conteudo = mensagem.Conteudo,
            DataEnvio = mensagem.DataEnvio,
            DataLeitura = mensagem.DataLeitura,
            Lida = mensagem.Lida
        };

    public static Mensagem ParaEntidade(this MensagemDto dto)
    {
        var mensagem = new Mensagem();
        mensagem.DefinirDados(
            dto.Id,
            dto.RemetenteId,
            dto.ConversaId,
            dto.AgendamentoId,
            dto.Conteudo,
            dto.DataEnvio,
            dto.DataLeitura,
            dto.Lida);
        return mensagem;
    }

    public static ConversaDto ParaDto(this Conversa conversa) =>
        new()
        {
            Id = conversa.Id,
            ClienteId = conversa.ClienteId,
            PrestadorId = conversa.PrestadorId,
            DataCriacao = conversa.DataCriacao,
            Mensagens = conversa.Mensagens.Select(ParaDto).ToList()
        };

    public static ServicoPrestadorDto ParaDto(this ServicoPrestador servico)
    {
        var endereco = servico.Prestador?.Usuario?.Endereco;

        return new ServicoPrestadorDto
        {
            Id = servico.Id,
            PrestadorId = servico.PrestadorId,
            Categoria = servico.Categoria,
            Titulo = servico.Titulo,
            Descricao = servico.Descricao,
            UnidadeCobranca = servico.UnidadeCobranca,
            PrecoBase = servico.PrecoBase,
            DuracaoEstimadaMinutos = servico.DuracaoEstimadaMinutos,
            AceitaPagamentoAposFinalizacao = servico.AceitaPagamentoAposFinalizacao,
            Ativo = servico.Ativo,
            DataCriacao = servico.DataCriacao,
            TipoAnuncio = servico.TipoAnuncio,
            PrestadorNome = servico.Prestador?.Usuario?.Nome,
            Cidade = endereco?.Cidade?.Nome,
            Estado = endereco?.Cidade?.Estado,
            MediaAvaliacoes = servico.Prestador?.MediaAvaliacoes
        };
    }

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
            dto.AceitaPagamentoAposFinalizacao,
            dto.Ativo,
            dto.DataCriacao == default ? DateTime.UtcNow : dto.DataCriacao);
        return servico;
    }

    public static ServicoClienteDto ParaDto(this ServicoCliente servico)
    {
        var endereco = servico.Cliente?.Usuario?.Endereco;

        return new ServicoClienteDto
        {
            Id = servico.Id,
            ClienteId = servico.ClienteId,
            Categoria = servico.Categoria,
            Titulo = servico.Titulo,
            Descricao = servico.Descricao,
            UnidadeCobranca = servico.UnidadeCobranca,
            PrecoBase = servico.PrecoBase,
            DataDesejada = servico.DataDesejada,
            Ativo = servico.Ativo,
            DataCriacao = servico.DataCriacao,
            TipoAnuncio = servico.TipoAnuncio,
            ClienteNome = servico.Cliente?.Usuario?.Nome,
            Cidade = endereco?.Cidade?.Nome,
            Estado = endereco?.Cidade?.Estado
        };
    }

    public static ServicoCliente ParaEntidade(this ServicoClienteDto dto)
    {
        var servico = new ServicoCliente();
        servico.DefinirDados(
            dto.Id,
            dto.ClienteId ?? Guid.Empty,
            dto.Categoria,
            dto.Titulo,
            dto.Descricao,
            dto.PrecoBase,
            dto.UnidadeCobranca,
            dto.DataDesejada,
            dto.Ativo,
            dto.DataCriacao == default ? DateTime.UtcNow : dto.DataCriacao);
        return servico;
    }

    public static PortfolioDto ParaDto(this Portfolio portfolio) =>
        new()
        {
            Id = portfolio.Id,
            PrestadorId = portfolio.PrestadorId,
            Titulo = portfolio.Titulo,
            Descricao = portfolio.Descricao,
            UrlImagem = portfolio.UrlImagem,
            DataCadastro = portfolio.DataCadastro,
            Ordem = portfolio.Ordem
        };

    public static CertificacaoDto ParaDto(this Certificacao certificacao) =>
        new()
        {
            Id = certificacao.Id,
            PrestadorId = certificacao.PrestadorId,
            Nome = certificacao.Nome,
            Instituicao = certificacao.Instituicao,
            DataEmissao = certificacao.DataEmissao,
            DataValidade = certificacao.DataValidade,
            UrlDocumento = certificacao.UrlDocumento,
            Verificada = certificacao.Verificada,
            DataCadastro = certificacao.DataCadastro
        };

    public static ServicoBuscaPaginadaDto ParaDto(this PaginacaoResultado<ServicoBase> pagina)
    {
        var itens = pagina.Itens.Select(servico =>
            servico switch
            {
                ServicoPrestador prestador => (object)prestador.ParaDto(),
                ServicoCliente cliente => cliente.ParaDto(),
                _ => throw new InvalidOperationException($"Tipo de serviço não suportado: {servico.GetType().Name}")
            }).ToArray();

        return new ServicoBuscaPaginadaDto
        {
            Itens = itens,
            PaginaAtual = pagina.PaginaAtual,
            TamanhoPagina = pagina.TamanhoPagina,
            TotalRegistros = pagina.TotalRegistros,
            TotalPaginas = pagina.TotalPaginas
        };
    }
}
