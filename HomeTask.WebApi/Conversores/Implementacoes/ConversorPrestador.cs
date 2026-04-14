using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorPrestador : IConversorPrestador
    {
        public PrestadorContrato ConverterPrestadorparaContrato(Domain.Entities.Prestador? prestador)
        {
            if (prestador == null)
                throw new ArgumentNullException(nameof(prestador));

            var retorno = new PrestadorContrato();
                retorno.Id = prestador.Id;
                retorno.UsuarioId = prestador.UsuarioId;
                retorno.Descricao = prestador.Descricao;
                retorno.RaioAtendimentoKm = prestador.RaioAtendimentoKm;
                retorno.Status = prestador.Status;
                retorno.MediaAvaliacoes = prestador.MediaAvaliacoes;
                retorno.TotalAvaliacoes = prestador.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = prestador.TotalServicosConcluidos;
                retorno.DataVerificacao = prestador.DataVerificacao;

                return retorno;
        }

        public Domain.Entities.Prestador? ConverterContratoparaPrestador(PrestadorContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Prestador();
                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;
                retorno.Descricao = contrato.Descricao;
                retorno.RaioAtendimentoKm = contrato.RaioAtendimentoKm;
                retorno.Status = contrato.Status;
                retorno.MediaAvaliacoes = contrato.MediaAvaliacoes;
                retorno.TotalAvaliacoes = contrato.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = contrato.TotalServicosConcluidos;
                retorno.DataVerificacao = contrato.DataVerificacao;

                return retorno;
        }

        public PrestadorViewModel? ConverterContratoparaViewModel(PrestadorContrato contrato)
        {
            if (contrato == null)
                return null;

            var retorno = new PrestadorViewModel();
                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;
                retorno.Descricao = contrato.Descricao;
                retorno.RaioAtendimentoKm = contrato.RaioAtendimentoKm;
                retorno.Status = contrato.Status;
                retorno.MediaAvaliacoes = contrato.MediaAvaliacoes;
                retorno.TotalAvaliacoes = contrato.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = contrato.TotalServicosConcluidos;
                retorno.DataVerificacao = contrato.DataVerificacao;

                return retorno;
        }

        public PrestadorContrato ConverterViewModelparaContrato(PrestadorViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            var retorno = new PrestadorContrato();
                retorno.Id = viewModel.Id;
                retorno.UsuarioId = viewModel.UsuarioId;
                retorno.Descricao = viewModel.Descricao;
                retorno.RaioAtendimentoKm = viewModel.RaioAtendimentoKm;
                retorno.Status = viewModel.Status;
                retorno.MediaAvaliacoes = viewModel.MediaAvaliacoes;
                retorno.TotalAvaliacoes = viewModel.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = viewModel.TotalServicosConcluidos;
                retorno.DataVerificacao = viewModel.DataVerificacao;

                return retorno;
        }
    }
}
