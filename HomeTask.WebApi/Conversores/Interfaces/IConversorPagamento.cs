using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorPagamento
    {
        public PagamentoContrato ConverterViewModelparaContrato(PagamentoViewModel viewModel);
        public PagamentoViewModel? ConverterContratoparaViewModel(PagamentoContrato contrato);
        public Domain.Entities.Pagamento? ConverterContratoparaPagamento(PagamentoContrato contrato);
        public PagamentoContrato ConverterPagamentoparaContrato(Domain.Entities.Pagamento? pagamento);
    }
}
