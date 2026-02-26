using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.ServicoOferecido;

namespace HomeTask.WebApi.Conversores.Implementacoes.ServicoOferecido
{
    public class ConversorServicoOferecido : IConversorServicoOferecido
    {
        public ServicoOferecidoContrato ConverterServicoOferecidoparaContrato(Domain.Entities.ServicoOferecido? servico)
        {
            if (servico == null)
                throw new ArgumentNullException(nameof(servico));

            var retorno = new ServicoOferecidoContrato();
                retorno.Id = servico.Id;
                retorno.PrestadorId = servico.PrestadorId;
                retorno.Categoria = servico.Categoria;
                retorno.Titulo = servico.Titulo;
                retorno.Descricao = servico.Descricao;
                retorno.PrecoBase = servico.PrecoBase;
                retorno.UnidadeCobranca = servico.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = servico.DuracaoEstimadaMinutos;
                retorno.Ativo = servico.Ativo;
                retorno.DataCriacao = servico.DataCriacao;

                return retorno;
        }

        public Domain.Entities.ServicoOferecido? ConverterContratoparaServicoOferecido(ServicoOferecidoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.ServicoOferecido();

                retorno.Id = contrato.Id;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.Categoria = contrato.Categoria;
                retorno.Titulo = contrato.Titulo;
                retorno.Descricao = contrato.Descricao;
                retorno.PrecoBase = contrato.PrecoBase;
                retorno.UnidadeCobranca = contrato.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = contrato.DuracaoEstimadaMinutos;
                retorno.Ativo = contrato.Ativo;
                retorno.DataCriacao = contrato.DataCriacao;

                return retorno;
        }

        public ServicoOferecidoViewModel? ConverterContratoparaViewModel(ServicoOferecidoContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new ServicoOferecidoViewModel();

                retorno.Id = contrato.Id;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.Categoria = contrato.Categoria;
                retorno.Titulo = contrato.Titulo;
                retorno.Descricao = contrato.Descricao;
                retorno.PrecoBase = contrato.PrecoBase;
                retorno.UnidadeCobranca = contrato.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = contrato.DuracaoEstimadaMinutos;
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
                retorno.Categoria = viewModel.Categoria;
                retorno.Titulo = viewModel.Titulo;
                retorno.Descricao = viewModel.Descricao;
                retorno.PrecoBase = viewModel.PrecoBase;
                retorno.UnidadeCobranca = viewModel.UnidadeCobranca;
                retorno.DuracaoEstimadaMinutos = viewModel.DuracaoEstimadaMinutos;
                retorno.Ativo = viewModel.Ativo;
                retorno.DataCriacao = viewModel.DataCriacao;

                return retorno;
            }
            return null;

        }
    }
}
