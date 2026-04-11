using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorAvaliacao
    {
        public AvaliacaoContrato ConverterViewModelparaContrato(AvaliacaoViewModel viewModel);
        public AvaliacaoViewModel? ConverterContratoparaViewModel(AvaliacaoContrato contrato);
        public Domain.Entities.Avaliacao? ConverterContratoparaAvaliacao(AvaliacaoContrato contrato);
        public AvaliacaoContrato ConverterAvaliacaoparaContrato(Domain.Entities.Avaliacao? avaliacao);
    }
}
