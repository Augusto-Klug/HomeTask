using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Enums;
using System.Linq;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorServicoOferecido : IConversorServicoOferecido
    {
        public ServicoPrestadorContrato ConverterPrestadorViewModelparaContrato(ServicoPrestadorViewModel viewModel)
        {
            return new ServicoPrestadorContrato
            {
                Id = viewModel.Id,
                PrestadorId = viewModel.PrestadorId,
                Categoria = viewModel.Categoria,
                Titulo = viewModel.Titulo,
                Descricao = viewModel.Descricao,
                UnidadeCobranca = viewModel.UnidadeCobranca,
                PrecoBase = viewModel.PrecoBase,
                DuracaoEstimadaMinutos = viewModel.DuracaoEstimadaMinutos,
                AceitaPagamentoAposFinalizacao = viewModel.AceitaPagamentoAposFinalizacao,
                Ativo = viewModel.Ativo,
                TipoAnuncio = TipoAnuncio.Oferta
            };
        }

        public ServicoPrestadorViewModel? ConverterContratoparaPrestadorViewModel(ServicoPrestadorContrato contrato)
        {
            return new ServicoPrestadorViewModel
            {
                Id = contrato.Id,
                PrestadorId = contrato.PrestadorId,
                TipoAnuncio = contrato.TipoAnuncio,
                Categoria = contrato.Categoria,
                Titulo = contrato.Titulo,
                Descricao = contrato.Descricao,
                UnidadeCobranca = contrato.UnidadeCobranca,
                PrecoBase = contrato.PrecoBase,
                DuracaoEstimadaMinutos = contrato.DuracaoEstimadaMinutos,
                AceitaPagamentoAposFinalizacao = contrato.AceitaPagamentoAposFinalizacao,
                Ativo = contrato.Ativo,
                PrestadorNome = contrato.PrestadorNome,
                Cidade = contrato.Cidade,
                Estado = contrato.Estado,
                MediaAvaliacoes = contrato.MediaAvaliacoes
            };
        }

        public ServicoPrestador? ConverterPrestadorContratoparaEntidade(ServicoPrestadorContrato contrato)
        {
            var servico = new ServicoPrestador();
            var dataCriacao = contrato.DataCriacao == default ? DateTime.UtcNow : contrato.DataCriacao;
            
            servico.DefinirDados(
                contrato.Id,
                contrato.PrestadorId ?? Guid.Empty,
                contrato.Categoria,
                contrato.Titulo,
                contrato.Descricao,
                contrato.PrecoBase,
                contrato.UnidadeCobranca,
                contrato.DuracaoEstimadaMinutos,
                contrato.AceitaPagamentoAposFinalizacao,
                contrato.Ativo,
                dataCriacao
            );
            return servico;
        }

        public ServicoPrestadorContrato ConverterEntidadeparaPrestadorContrato(ServicoPrestador? servico)
        {
            if (servico == null) return null!;
            
            var endereco = servico.Prestador?.Usuario?.Endereco;

            return new ServicoPrestadorContrato
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

        public ServicoClienteContrato ConverterClienteViewModelparaContrato(ServicoClienteViewModel viewModel)
        {
            return new ServicoClienteContrato
            {
                Id = viewModel.Id,
                ClienteId = viewModel.ClienteId,
                Categoria = viewModel.Categoria,
                Titulo = viewModel.Titulo,
                Descricao = viewModel.Descricao,
                UnidadeCobranca = viewModel.UnidadeCobranca,
                PrecoBase = viewModel.PrecoBase,
                DataDesejada = viewModel.DataDesejada,
                Ativo = viewModel.Ativo,
                TipoAnuncio = TipoAnuncio.Pedido
            };
        }

        public ServicoClienteViewModel? ConverterContratoparaClienteViewModel(ServicoClienteContrato contrato)
        {
            return new ServicoClienteViewModel
            {
                Id = contrato.Id,
                ClienteId = contrato.ClienteId,
                TipoAnuncio = contrato.TipoAnuncio,
                Categoria = contrato.Categoria,
                Titulo = contrato.Titulo,
                Descricao = contrato.Descricao,
                UnidadeCobranca = contrato.UnidadeCobranca,
                PrecoBase = contrato.PrecoBase,
                DataDesejada = contrato.DataDesejada,
                Ativo = contrato.Ativo,
                ClienteNome = contrato.ClienteNome,
                Cidade = contrato.Cidade,
                Estado = contrato.Estado
            };
        }

        public ServicoCliente? ConverterClienteContratoparaEntidade(ServicoClienteContrato contrato)
        {
            var servico = new ServicoCliente();
            var dataCriacao = contrato.DataCriacao == default ? DateTime.UtcNow : contrato.DataCriacao;

            servico.DefinirDados(
                contrato.Id,
                contrato.ClienteId ?? Guid.Empty,
                contrato.Categoria,
                contrato.Titulo,
                contrato.Descricao,
                contrato.PrecoBase,
                contrato.UnidadeCobranca,
                contrato.DataDesejada,
                contrato.Ativo,
                dataCriacao
            );
            return servico;
        }

        public ServicoClienteContrato ConverterEntidadeparaClienteContrato(ServicoCliente? servico)
        {
            if (servico == null) return null!;

            var endereco = servico.Cliente?.Usuario?.Endereco;

            return new ServicoClienteContrato
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
    }
}
