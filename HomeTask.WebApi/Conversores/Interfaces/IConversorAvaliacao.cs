using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorAvaliacao
    {
        public AvaliacaoContrato ConverterViewModelparaContrato(AvaliacaoViewModel viewModel);
        public AvaliacaoViewModel? ConverterContratoparaViewModel(AvaliacaoContrato contrato);
        public Avaliacao? ConverterContratoparaAvaliacao(AvaliacaoContrato contrato);
        public AvaliacaoContrato ConverterAvaliacaoparaContrato(Avaliacao? avaliacao);
    }
}
