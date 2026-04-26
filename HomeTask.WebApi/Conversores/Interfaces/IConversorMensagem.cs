using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorMensagem
    {
        public MensagemContrato ConverterViewModelparaContrato(MensagemViewModel viewModel);
        public MensagemViewModel? ConverterContratoparaViewModel(MensagemContrato contrato);
        public Mensagem? ConverterContratoparaMensagem(MensagemContrato contrato);
        public MensagemContrato ConverterMensagemparaContrato(Mensagem? mensagem);
    }
}
