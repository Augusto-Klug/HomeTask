using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorServicoOferecido
    {
        // Novos métodos específicos
        public ServicoPrestadorContrato ConverterPrestadorViewModelparaContrato(ServicoPrestadorViewModel viewModel);
        public ServicoPrestadorViewModel? ConverterContratoparaPrestadorViewModel(ServicoPrestadorContrato contrato);
        public ServicoPrestador? ConverterPrestadorContratoparaEntidade(ServicoPrestadorContrato contrato);
        public ServicoPrestadorContrato ConverterEntidadeparaPrestadorContrato(ServicoPrestador? servico);

        public ServicoClienteContrato ConverterClienteViewModelparaContrato(ServicoClienteViewModel viewModel);
        public ServicoClienteViewModel? ConverterContratoparaClienteViewModel(ServicoClienteContrato contrato);
        public ServicoCliente? ConverterClienteContratoparaEntidade(ServicoClienteContrato contrato);
        public ServicoClienteContrato ConverterEntidadeparaClienteContrato(ServicoCliente? servico);
    }
}
