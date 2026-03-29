using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.Usuario;

namespace HomeTask.WebApi.Conversores.Implementacoes.Usuario
{
    public class ConversorUsuario : IConversorUsuario
    {
        public UsuarioContrato ConverterUsuarioparaContrato(Domain.Entities.Usuario? usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var retorno = new UsuarioContrato();
                retorno.Id = usuario.Id;
                retorno.Nome = usuario.Nome;
                retorno.Email = usuario.Email;
                retorno.Documento = usuario.Documento;
                retorno.Endereco = usuario.Endereco;
                retorno.Cidade = usuario.Cidade;
                retorno.Estado = usuario.Estado;
                retorno.Cep = usuario.Cep;
                retorno.Bairro = usuario.Bairro;
                retorno.RaioAtendimentoKm = usuario.RaioAtendimentoKm;
                retorno.Status = usuario.Status;
                retorno.MediaAvaliacoes = usuario.MediaAvaliacoes;
                retorno.TotalAvaliacoes = usuario.TotalAvaliacoes;
                retorno.TotalServicosConcluidos = usuario.TotalServicosConcluidos;
                retorno.DataVerificacao = usuario.DataVerificacao;
                retorno.Telefone = usuario.Telefone;
                retorno.TipoUsuario = usuario.TipoUsuario;
                retorno.DataCadastro = usuario.DataCadastro;
                retorno.UltimoAcesso = usuario.UltimoAcesso;
                retorno.Ativo = usuario.Ativo;

                return retorno;
        }

        public Domain.Entities.Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato)
        {
            ArgumentNullException.ThrowIfNull(contrato);

            var retorno = new Domain.Entities.Usuario();

                retorno.Id = contrato.Id;
                retorno.Nome = contrato.Nome;
                retorno.Email = contrato.Email;
                retorno.Documento = contrato.Documento;
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
                retorno.Telefone = contrato.Telefone;
                retorno.TipoUsuario = contrato.TipoUsuario;
                retorno.DataCadastro = contrato.DataCadastro;
                retorno.UltimoAcesso = contrato.UltimoAcesso;
                retorno.Ativo = contrato.Ativo;

                return retorno;
        }

        public UsuarioViewModel? ConverterContratoparaViewModel(UsuarioContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new UsuarioViewModel();

                retorno.Id = contrato.Id;
                retorno.Nome = contrato.Nome;
                retorno.Email = contrato.Email;
                retorno.Senha = contrato.Senha;
                retorno.Documento = contrato.Documento;
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
                retorno.Telefone = contrato.Telefone;
                retorno.TipoUsuario = contrato.TipoUsuario;
                retorno.DataCadastro = contrato.DataCadastro;
                retorno.UltimoAcesso = contrato.UltimoAcesso;
                retorno.Ativo = contrato.Ativo;

                return retorno;
            }
            return null;
        }

        public UsuarioContrato ConverterViewModelparaContrato(UsuarioViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new UsuarioContrato();

                retorno.Id = viewModel.Id;
                retorno.Nome = viewModel.Nome;
                retorno.Email = viewModel.Email;
                retorno.Senha = viewModel.Senha;
                retorno.Documento = viewModel.Documento;
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
                retorno.Telefone = viewModel.Telefone;
                retorno.TipoUsuario = viewModel.TipoUsuario;
                retorno.DataCadastro = viewModel.DataCadastro;
                retorno.UltimoAcesso = viewModel.UltimoAcesso;
                retorno.Ativo = viewModel.Ativo;

                return retorno;
            }
            return null;

        }
    }
}
