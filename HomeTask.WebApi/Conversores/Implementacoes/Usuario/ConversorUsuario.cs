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
                retorno.Telefone = usuario.Telefone;
                retorno.Tipo = usuario.TipoUsuario;
                retorno.DataCadastro = usuario.DataCadastro;
                retorno.UltimoAcesso = usuario.UltimoAcesso;
                retorno.Ativo = usuario.Ativo;

                return retorno;
        }

        public Domain.Entities.Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Usuario();

                retorno.Id = contrato.Id;
                retorno.Nome = contrato.Nome;
                retorno.Email = contrato.Email;
                retorno.Documento = contrato.Documento;
                retorno.Endereco = contrato.Endereco;
                retorno.Telefone = contrato.Telefone;
                retorno.TipoUsuario = contrato.Tipo;
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
                retorno.Telefone = contrato.Telefone;
                retorno.Tipo = contrato.Tipo;
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
                retorno.Tipo = viewModel.TipoUsuario;
                retorno.Nome = viewModel.Nome;
                retorno.Email = viewModel.Email;
                retorno.Endereco = viewModel.Endereco;
                retorno.Senha = viewModel.Senha;
                retorno.Documento = viewModel.Documento;
                retorno.Telefone = viewModel.Telefone;
                retorno.Tipo = viewModel.Tipo;
                retorno.DataCadastro = viewModel.DataCadastro;
                retorno.UltimoAcesso = viewModel.UltimoAcesso;
                retorno.Ativo = viewModel.Ativo;

                return retorno;
            }
            return null;

        }
    }
}
