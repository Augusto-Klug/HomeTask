using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorPrestador
    {
        public PrestadorContrato ConverterViewModelparaContrato(PrestadorViewModel viewModel);
        public PrestadorViewModel? ConverterContratoparaViewModel(PrestadorContrato contrato);
        public Prestador? ConverterContratoparaPrestador(PrestadorContrato contrato);
        public PrestadorContrato ConverterPrestadorparaContrato(Prestador? prestador);
    }
}
