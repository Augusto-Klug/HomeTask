using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorPrestador
    {
        public PrestadorContrato ConverterViewModelparaContrato(PrestadorViewModel viewModel);
        public PrestadorViewModel? ConverterContratoparaViewModel(PrestadorContrato contrato);
        public Domain.Entities.Prestador? ConverterContratoparaPrestador(PrestadorContrato contrato);
        public PrestadorContrato ConverterPrestadorparaContrato(Domain.Entities.Prestador? prestador);
    }
}
