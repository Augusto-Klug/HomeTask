using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorServicoOferecido
    {
        public ServicoOferecidoContrato ConverterViewModelparaContrato(ServicoOferecidoViewModel viewModel);
        public ServicoOferecidoViewModel? ConverterContratoparaViewModel(ServicoOferecidoContrato contrato);
        public Domain.Entities.ServicoOferecido? ConverterContratoparaServicoOferecido(ServicoOferecidoContrato contrato);
        public ServicoOferecidoContrato ConverterServicoOferecidoparaContrato(Domain.Entities.ServicoOferecido? servico);
    }
}
