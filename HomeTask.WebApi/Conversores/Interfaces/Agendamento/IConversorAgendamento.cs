using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Agendamento
{
    public interface IConversorAgendamento
    {
        public AgendamentoContrato ConverterViewModelparaContrato(AgendamentoViewModel viewModel);
        public AgendamentoViewModel? ConverterContratoparaViewModel(AgendamentoContrato contrato);
        public HomeTask.Domain.Entities.Agendamento? ConverterContratoparaAgendamento(AgendamentoContrato contrato);
        public AgendamentoContrato ConverterAgendamentoparaContrato(HomeTask.Domain.Entities.Agendamento? agendamento);
    }
}
