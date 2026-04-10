using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorMensagem
    {
        public MensagemContrato ConverterViewModelparaContrato(MensagemViewModel viewModel);
        public MensagemViewModel? ConverterContratoparaViewModel(MensagemContrato contrato);
        public Domain.Entities.Mensagem? ConverterContratoparaMensagem(MensagemContrato contrato);
        public MensagemContrato ConverterMensagemparaContrato(Domain.Entities.Mensagem? mensagem);
    }
}
