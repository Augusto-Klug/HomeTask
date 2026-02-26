using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.ServicoOferecido
{
    public interface IConversorServicoOferecido
    {
        public ServicoOferecidoContrato ConverterViewModelparaContrato(ServicoOferecidoViewModel viewModel);
        public ServicoOferecidoViewModel? ConverterContratoparaViewModel(ServicoOferecidoContrato contrato);
        public HomeTask.Domain.Entities.ServicoOferecido? ConverterContratoparaServicoOferecido(ServicoOferecidoContrato contrato);
        public ServicoOferecidoContrato ConverterServicoOferecidoparaContrato(HomeTask.Domain.Entities.ServicoOferecido? servico);
    }
}
