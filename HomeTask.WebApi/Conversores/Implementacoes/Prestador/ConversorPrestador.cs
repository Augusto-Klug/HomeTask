using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.Prestador;

namespace HomeTask.WebApi.Conversores.Implementacoes.Prestador
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
                retorno.Documento = prestador.Documento;
                retorno.Descricao = prestador.Descricao;
                retorno.Endereco = prestador.Endereco;
                retorno.Cidade = prestador.Cidade;
                retorno.Estado = prestador.Estado;
                retorno.Cep = prestador.Cep;
                retorno.Bairro = prestador.Bairro;
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
                retorno.Documento = contrato.Documento; 
                retorno.Descricao = contrato.Descricao;
                retorno.Endereco = contrato.Endereco;
                retorno.Cidade = contrato.Cidade;
                retorno.Estado = contrato.Estado;
                retorno.Cep = contrato.Cep;
                retorno.Bairro = contrato.Bairro;
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
            if (contrato != null)
            {
                var retorno = new PrestadorViewModel();

                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;
                retorno.Documento = contrato.Documento;
                retorno.Descricao = contrato.Descricao;
                retorno.Endereco = contrato.Endereco;
                retorno.Cidade = contrato.Cidade;
                retorno.Estado = contrato.Estado;
                retorno.Cep = contrato.Cep;
                retorno.Bairro = contrato.Bairro;
                retorno.RaioAtendimentoKm = contrato.RaioAtendimentoKm;
                retorno.Status = contrato.Status;
                retorno.MediaAvaliacoes = contrato.MediaAvaliacoes;
                retorno.TotalAvaliacoes = contrato.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = contrato.TotalServicosConcluidos;
                retorno.DataVerificacao = contrato.DataVerificacao;

                return retorno;
            }
            return null;
        }

        public PrestadorContrato ConverterViewModelparaContrato(PrestadorViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new PrestadorContrato();

                retorno.Id = viewModel.Id;
                retorno.UsuarioId = viewModel.UsuarioId;
                retorno.Documento = viewModel.Documento;
                retorno.Descricao = viewModel.Descricao;
                retorno.Endereco = viewModel.Endereco;
                retorno.Cidade = viewModel.Cidade;
                retorno.Estado = viewModel.Estado;
                retorno.Cep = viewModel.Cep;
                retorno.Bairro = viewModel.Bairro;
                retorno.RaioAtendimentoKm = viewModel.RaioAtendimentoKm;
                retorno.Status = viewModel.Status;
                retorno.MediaAvaliacoes = viewModel.MediaAvaliacoes;
                retorno.TotalAvaliacoes = viewModel.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = viewModel.TotalServicosConcluidos;
                retorno.DataVerificacao = viewModel.DataVerificacao;

                return retorno;
            }
            return null;

        }
    }
}
