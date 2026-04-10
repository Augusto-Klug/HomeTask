using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorCliente
    {
        public ClienteContrato ConverterViewModelparaContrato(ClienteViewModel viewModel);
        public ClienteViewModel? ConverterContratoparaViewModel(ClienteContrato contrato);
        public Domain.Entities.Cliente? ConverterContratoparaCliente(ClienteContrato contrato);
        public ClienteContrato ConverterClienteparaContrato(Domain.Entities.Cliente? cliente);
    }
}
