using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Cliente
{
    public interface IConversorCliente
    {
        public ClienteContrato ConverterViewModelparaContrato(ClienteViewModel viewModel);
        public ClienteViewModel? ConverterContratoparaViewModel(ClienteContrato contrato);
        public HomeTask.Domain.Entities.Cliente? ConverterContratoparaCliente(ClienteContrato contrato);
        public ClienteContrato ConverterClienteparaContrato(HomeTask.Domain.Entities.Cliente? cliente);
    }
}
