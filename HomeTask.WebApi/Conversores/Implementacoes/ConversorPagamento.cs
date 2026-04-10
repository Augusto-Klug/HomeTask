using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorPagamento : IConversorPagamento
    {
        public PagamentoContrato ConverterPagamentoparaContrato(Domain.Entities.Pagamento? pagamento)
        {
            if (pagamento == null)
                throw new ArgumentNullException(nameof(pagamento));

            var retorno = new PagamentoContrato();
                retorno.Id = pagamento.Id;
                retorno.AgendamentoId = pagamento.AgendamentoId;
                retorno.Valor = pagamento.Valor;
                retorno.TipoPagamento = pagamento.TipoPagamento;
                retorno.Status = pagamento.Status;
                retorno.TransacaoId = pagamento.TransacaoId;
                retorno.DataCriacao = pagamento.DataCriacao;
                retorno.DataProcessamento = pagamento.DataProcessamento;
                retorno.DataConfirmacao = pagamento.DataConfirmacao;
                retorno.MotivoRecusa = pagamento.MotivoRecusa;

                return retorno;
        }

        public Domain.Entities.Pagamento? ConverterContratoparaPagamento(PagamentoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Pagamento();

                retorno.Id = contrato.Id;
                retorno.AgendamentoId = contrato.AgendamentoId;
                retorno.Valor = contrato.Valor;
                retorno.TipoPagamento = contrato.TipoPagamento;
                retorno.Status = contrato.Status;
                retorno.TransacaoId = contrato.TransacaoId;
                retorno.DataCriacao = contrato.DataCriacao;
                retorno.DataProcessamento = contrato.DataProcessamento;
                retorno.DataConfirmacao = contrato.DataConfirmacao;
                retorno.MotivoRecusa = contrato.MotivoRecusa;

                return retorno;
        }

        public PagamentoViewModel? ConverterContratoparaViewModel(PagamentoContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new PagamentoViewModel();

                retorno.Id = contrato.Id;
                retorno.AgendamentoId = contrato.AgendamentoId;
                retorno.Valor = contrato.Valor;
                retorno.TipoPagamento = contrato.TipoPagamento;
                retorno.Status = contrato.Status;
                retorno.TransacaoId = contrato.TransacaoId;
                retorno.DataCriacao = contrato.DataCriacao;
                retorno.DataProcessamento = contrato.DataProcessamento;
                retorno.DataConfirmacao = contrato.DataConfirmacao;
                retorno.MotivoRecusa = contrato.MotivoRecusa;

                return retorno;
            }
            return null;
        }

        public PagamentoContrato ConverterViewModelparaContrato(PagamentoViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new PagamentoContrato();

                retorno.Id = viewModel.Id;
                retorno.AgendamentoId = viewModel.AgendamentoId;
                retorno.Valor = viewModel.Valor;
                retorno.TipoPagamento = viewModel.TipoPagamento;
                retorno.Status = viewModel.Status;
                retorno.TransacaoId = viewModel.TransacaoId;
                retorno.DataCriacao = viewModel.DataCriacao;
                retorno.DataProcessamento = viewModel.DataProcessamento;
                retorno.DataConfirmacao = viewModel.DataConfirmacao;
                retorno.MotivoRecusa = viewModel.MotivoRecusa;

                return retorno;
            }
            return null;

        }
    }
}
