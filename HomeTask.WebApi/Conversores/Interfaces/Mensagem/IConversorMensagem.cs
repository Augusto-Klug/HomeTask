using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Mensagem
{
    public interface IConversorMensagem
    {
        public MensagemContrato ConverterViewModelparaContrato(MensagemViewModel viewModel);
        public MensagemViewModel? ConverterContratoparaViewModel(MensagemContrato contrato);
        public HomeTask.Domain.Entities.Mensagem? ConverterContratoparaMensagem(MensagemContrato contrato);
        public MensagemContrato ConverterMensagemparaContrato(HomeTask.Domain.Entities.Mensagem? mensagem);
    }
}
