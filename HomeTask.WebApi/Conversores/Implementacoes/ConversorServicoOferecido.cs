using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;
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
                retorno.Categoria = servico.Categoria;
                retorno.Titulo = servico.Titulo;
                retorno.Descricao = servico.Descricao;
                retorno.Valor = servico.Valor;
                retorno.UnidadeCobranca = servico.UnidadeCobranca;
                retorno.AceitaPagamentoAposFinalizacao = servico.AceitaPagamentoAposFinalizacao;
                retorno.DataAgendamento = servico.DataAgendamento;
                retorno.Ativo = servico.Ativo;
                retorno.DataCriacao = servico.DataCriacao;

                return retorno;
        }

        public ServicoOferecido? ConverterContratoparaServicoOferecido(ServicoOferecidoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new ServicoOferecido();

                retorno.Id = contrato.Id;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.Categoria = contrato.Categoria;
                retorno.Titulo = contrato.Titulo;
                retorno.Descricao = contrato.Descricao;
                retorno.Valor = contrato.Valor;
                retorno.UnidadeCobranca = contrato.UnidadeCobranca;
                retorno.AceitaPagamentoAposFinalizacao = contrato.AceitaPagamentoAposFinalizacao;
                retorno.DataAgendamento = contrato.DataAgendamento;
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
                retorno.Valor = contrato.Valor;
                retorno.UnidadeCobranca = contrato.UnidadeCobranca;
                retorno.AceitaPagamentoAposFinalizacao = contrato.AceitaPagamentoAposFinalizacao;
                retorno.DataAgendamento = contrato.DataAgendamento;
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
                retorno.Valor = viewModel.Valor;
                retorno.UnidadeCobranca = viewModel.UnidadeCobranca;
                retorno.AceitaPagamentoAposFinalizacao = viewModel.AceitaPagamentoAposFinalizacao;
                retorno.DataAgendamento = viewModel.DataAgendamento;
                retorno.Ativo = viewModel.Ativo;
                retorno.DataCriacao = viewModel.DataCriacao;

                return retorno;
            }
            return null;

        }
    }
}
