using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorMensagem : IConversorMensagem
    {
        public MensagemContrato ConverterMensagemparaContrato(Mensagem? mensagem)
        {
            if (mensagem == null)
                throw new ArgumentNullException(nameof(mensagem));

            var retorno = new MensagemContrato();
                retorno.Id = mensagem.Id;
                retorno.RemetenteId = mensagem.RemetenteId;
                retorno.ConversaId = mensagem.ConversaId;
                retorno.AgendamentoId = mensagem.AgendamentoId;
                retorno.Conteudo = mensagem.Conteudo;
                retorno.DataEnvio = mensagem.DataEnvio;
                retorno.DataLeitura = mensagem.DataLeitura;
                retorno.Lida = mensagem.Lida;

                return retorno;
        }

        public Mensagem? ConverterContratoparaMensagem(MensagemContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Mensagem();
                retorno.DefinirDados(
                    contrato.Id,
                    contrato.RemetenteId,
                    contrato.ConversaId,
                    contrato.AgendamentoId,
                    contrato.Conteudo,
                    contrato.DataEnvio,
                    contrato.DataLeitura,
                    contrato.Lida);

                return retorno;
        }

        public MensagemViewModel? ConverterContratoparaViewModel(MensagemContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new MensagemViewModel();

                retorno.Id = contrato.Id;
                retorno.RemetenteId = contrato.RemetenteId;
                retorno.ConversaId = contrato.ConversaId;
                retorno.AgendamentoId = contrato.AgendamentoId;
                retorno.Conteudo = contrato.Conteudo;
                retorno.DataEnvio = contrato.DataEnvio;
                retorno.DataLeitura = contrato.DataLeitura;
                retorno.Lida = contrato.Lida;

                return retorno;
            }
            return null;
        }

        public MensagemContrato ConverterViewModelparaContrato(MensagemViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new MensagemContrato();

                retorno.Id = viewModel.Id;
                retorno.RemetenteId = viewModel.RemetenteId;
                retorno.ConversaId = viewModel.ConversaId;
                retorno.AgendamentoId = viewModel.AgendamentoId;
                retorno.Conteudo = viewModel.Conteudo;
                retorno.DataEnvio = viewModel.DataEnvio;
                retorno.DataLeitura = viewModel.DataLeitura;
                retorno.Lida = viewModel.Lida;

                return retorno;
            }
            return null;

        }
    }
}
