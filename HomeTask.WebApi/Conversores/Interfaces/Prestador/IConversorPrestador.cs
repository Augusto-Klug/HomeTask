using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Prestador
{
    public interface IConversorPrestador
    {
        public PrestadorContrato ConverterViewModelparaContrato(PrestadorViewModel viewModel);
        public PrestadorViewModel? ConverterContratoparaViewModel(PrestadorContrato contrato);
        public HomeTask.Domain.Entities.Prestador? ConverterContratoparaPrestador(PrestadorContrato contrato);
        public PrestadorContrato ConverterPrestadorparaContrato(HomeTask.Domain.Entities.Prestador? prestador);
    }
}
