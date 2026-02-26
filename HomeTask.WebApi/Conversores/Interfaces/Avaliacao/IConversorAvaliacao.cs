using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Avaliacao
{
    public interface IConversorAvaliacao
    {
        public AvaliacaoContrato ConverterViewModelparaContrato(AvaliacaoViewModel viewModel);
        public AvaliacaoViewModel? ConverterContratoparaViewModel(AvaliacaoContrato contrato);
        public HomeTask.Domain.Entities.Avaliacao? ConverterContratoparaAvaliacao(AvaliacaoContrato contrato);
        public AvaliacaoContrato ConverterAvaliacaoparaContrato(HomeTask.Domain.Entities.Avaliacao? avaliacao);
    }
}
