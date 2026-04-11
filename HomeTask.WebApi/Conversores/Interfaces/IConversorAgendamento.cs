using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entities;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorAgendamento
    {
        public AgendamentoContrato ConverterViewModelparaContrato(AgendamentoViewModel viewModel);
        public AgendamentoViewModel? ConverterContratoparaViewModel(AgendamentoContrato contrato);
        public Agendamento? ConverterContratoparaAgendamento(AgendamentoContrato contrato);
        public AgendamentoContrato ConverterAgendamentoparaContrato(Agendamento? agendamento);
    }
}
