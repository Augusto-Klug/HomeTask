using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Entities;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorServicoOferecido : IConversorServicoOferecido
    {
        public ServicoOferecidoContrato ConverterServicoOferecidoparaContrato(ServicoOferecido? servico)
        {
            if (servico == null)
                throw new ArgumentNullException(nameof(servico));

            var retorno = new ServicoOferecidoContrato();

                retorno.Id = servico.Id;
                retorno.PrestadorId = servico.PrestadorId;
                retorno.CategoriaId = servico.CategoriaId;
                retorno.ClienteId = servico.ClienteId;
                retorno.Titulo = servico.Titulo;
                retorno.Descricao = servico.Descricao;
                retorno.PrecoBase = servico.PrecoBase;
                retorno.UnidadeCobranca = servico.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = servico.DuracaoEstimadaMinutos;
                retorno.TipoAnuncio = servico.TipoAnuncio;
                retorno.Ativo = servico.Ativo;
                retorno.DataCriacao = servico.DataCriacao;

                return retorno;
        }

        public ServicoOferecido? ConverterContratoparaServicoOferecido(ServicoOferecidoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new ServicoOferecido();
                retorno.DefinirDados(
                    contrato.Id,
                    contrato.PrestadorId,
                    contrato.CategoriaId,
                    contrato.ClienteId,
                    contrato.Titulo,
                    contrato.Descricao,
                    contrato.PrecoBase,
                    contrato.UnidadeCobranca,
                    contrato.DuracaoEstimadaMinutos,
                    contrato.TipoAnuncio,
                    contrato.Ativo,
                    contrato.DataCriacao);

                return retorno;
        }

        public ServicoOferecidoViewModel? ConverterContratoparaViewModel(ServicoOferecidoContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new ServicoOferecidoViewModel();

                retorno.Id = contrato.Id;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.CategoriaId = contrato.CategoriaId;
                retorno.Titulo = contrato.Titulo;
                retorno.Descricao = contrato.Descricao;
                retorno.PrecoBase = contrato.PrecoBase;
                retorno.UnidadeCobranca = contrato.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = contrato.DuracaoEstimadaMinutos;
                retorno.TipoAnuncio = contrato.TipoAnuncio;
                retorno.Ativo = contrato.Ativo;
                retorno.DataCriacao = contrato.DataCriacao;

                return retorno;
            }
            return null;
        }

        public ServicoOferecidoContrato ConverterViewModelparaContrato(ServicoOferecidoViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new ServicoOferecidoContrato();

                retorno.Id = viewModel.Id;
                retorno.PrestadorId = viewModel.PrestadorId;
                retorno.CategoriaId = viewModel.CategoriaId;
                retorno.Titulo = viewModel.Titulo;
                retorno.Descricao = viewModel.Descricao;
                retorno.PrecoBase = viewModel.PrecoBase;
                retorno.UnidadeCobranca = viewModel.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = viewModel.DuracaoEstimadaMinutos;
                retorno.TipoAnuncio = viewModel.TipoAnuncio;
                retorno.Ativo = viewModel.Ativo;
                retorno.DataCriacao = viewModel.DataCriacao;

                return retorno;
            }
            return null;

        }
    }
}
