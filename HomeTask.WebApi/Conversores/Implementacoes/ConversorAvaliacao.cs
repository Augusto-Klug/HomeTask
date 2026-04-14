using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorAvaliacao : IConversorAvaliacao
    {
        public AvaliacaoContrato ConverterAvaliacaoparaContrato(Domain.Entities.Avaliacao? avaliacao)
        {
            if (avaliacao == null)
                throw new ArgumentNullException(nameof(avaliacao));

            var retorno = new AvaliacaoContrato();
                retorno.Id = avaliacao.Id;
                retorno.AgendamentoId = avaliacao.AgendamentoId;
                retorno.ClienteId = avaliacao.ClienteId;
                retorno.PrestadorId = avaliacao.PrestadorId;
                retorno.Nota = avaliacao.Nota;
                retorno.Comentario = avaliacao.Comentario;
                retorno.DataAvaliacao = avaliacao.DataAvaliacao;
                retorno.Visivel = avaliacao.Visivel;

                return retorno;
        }

        public Domain.Entities.Avaliacao? ConverterContratoparaAvaliacao(AvaliacaoContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Avaliacao();
                retorno.DefinirDados(
                    contrato.Id,
                    contrato.AgendamentoId,
                    contrato.ClienteId,
                    contrato.PrestadorId,
                    contrato.Nota,
                    contrato.Comentario,
                    contrato.DataAvaliacao,
                    contrato.Visivel);

                return retorno;
        }

        public AvaliacaoViewModel? ConverterContratoparaViewModel(AvaliacaoContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new AvaliacaoViewModel();

                retorno.Id = contrato.Id;
                retorno.AgendamentoId = contrato.AgendamentoId;
                retorno.ClienteId = contrato.ClienteId;
                retorno.PrestadorId = contrato.PrestadorId;
                retorno.Nota = contrato.Nota;
                retorno.Comentario = contrato.Comentario;
                retorno.DataAvaliacao = contrato.DataAvaliacao;
                retorno.Visivel = contrato.Visivel;

                return retorno;
            }
            return null;
        }

        public AvaliacaoContrato ConverterViewModelparaContrato(AvaliacaoViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new AvaliacaoContrato();

                retorno.Id = viewModel.Id;
                retorno.AgendamentoId = viewModel.AgendamentoId;
                retorno.ClienteId = viewModel.ClienteId;
                retorno.PrestadorId = viewModel.PrestadorId;
                retorno.Nota = viewModel.Nota;
                retorno.Comentario = viewModel.Comentario;
                retorno.DataAvaliacao = viewModel.DataAvaliacao;
                retorno.Visivel = viewModel.Visivel;

                return retorno;
            }
            return null;

        }
    }
}
