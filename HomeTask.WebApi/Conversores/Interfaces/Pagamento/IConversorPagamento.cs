using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Pagamento
{
    public interface IConversorPagamento
    {
        public PagamentoContrato ConverterViewModelparaContrato(PagamentoViewModel viewModel);
        public PagamentoViewModel? ConverterContratoparaViewModel(PagamentoContrato contrato);
        public HomeTask.Domain.Entities.Pagamento? ConverterContratoparaPagamento(PagamentoContrato contrato);
        public PagamentoContrato ConverterPagamentoparaContrato(HomeTask.Domain.Entities.Pagamento? pagamento);
    }
}
