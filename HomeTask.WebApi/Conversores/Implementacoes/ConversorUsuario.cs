using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.Entities;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorUsuario : IConversorUsuario
    {
        public UsuarioContrato ConverterUsuarioparaContrato(Usuario? usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // pega o endereço principal para exibir no retorno
            var enderecoPrincipal = usuario.Enderecos.FirstOrDefault(e => e.Principal)
                                    ?? usuario.Enderecos.FirstOrDefault();

            var retorno = new UsuarioContrato();
                retorno.Id = usuario.Id;
                retorno.Nome = usuario.Nome;
                retorno.Email = usuario.Email;
                retorno.Documento = usuario.Documento;
                retorno.Telefone = usuario.Telefone;
                retorno.TipoUsuario = usuario.TipoUsuario;
                retorno.DataCadastro = usuario.DataCadastro;
                retorno.UltimoAcesso = usuario.UltimoAcesso;
                retorno.Ativo = usuario.Ativo;

                // campos do endereço principal
                retorno.Logradouro = enderecoPrincipal?.Logradouro ?? string.Empty;
                retorno.Numero = enderecoPrincipal?.Numero;
                retorno.Complemento = enderecoPrincipal?.Complemento;
                retorno.Bairro = enderecoPrincipal?.Bairro ?? string.Empty;
                retorno.Cep = enderecoPrincipal?.Cep ?? string.Empty;
                retorno.CidadeId = enderecoPrincipal?.CidadeId ?? Guid.Empty;

                return retorno;
        }

        public Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato)
        {
            ArgumentNullException.ThrowIfNull(contrato);

            var retorno = new Usuario();
                retorno.Id = contrato.Id;
                retorno.Nome = contrato.Nome;
                retorno.Email = contrato.Email;
                retorno.Documento = contrato.Documento;
                retorno.Telefone = contrato.Telefone;
                retorno.TipoUsuario = contrato.TipoUsuario;
                retorno.DataCadastro = contrato.DataCadastro;
                retorno.UltimoAcesso = contrato.UltimoAcesso;
                retorno.Ativo = contrato.Ativo;

                // monta o endereço e adiciona à coleção do usuário
                retorno.Enderecos.Add(new Endereco
                {
                    Logradouro = contrato.Logradouro,
                    Numero = contrato.Numero,
                    Complemento = contrato.Complemento,
                    Bairro = contrato.Bairro,
                    Cep = contrato.Cep,
                    CidadeId = contrato.CidadeId,
                    Principal = true
                });

                return retorno;
        }

        public UsuarioViewModel? ConverterContratoparaViewModel(UsuarioContrato contrato)
        {
            if (contrato == null)
                return null;

            var retorno = new UsuarioViewModel();
                retorno.Id = contrato.Id;
                retorno.Nome = contrato.Nome;
                retorno.Email = contrato.Email;
                retorno.Senha = contrato.Senha;
                retorno.Documento = contrato.Documento;
                retorno.Telefone = contrato.Telefone;
                retorno.TipoUsuario = contrato.TipoUsuario;
                retorno.DataCadastro = contrato.DataCadastro;
                retorno.UltimoAcesso = contrato.UltimoAcesso;
                retorno.Ativo = contrato.Ativo;
                retorno.Logradouro = contrato.Logradouro;
                retorno.Numero = contrato.Numero;
                retorno.Complemento = contrato.Complemento;
                retorno.Bairro = contrato.Bairro;
                retorno.Cep = contrato.Cep;
                retorno.CidadeId = contrato.CidadeId;

                return retorno;
        }

        public UsuarioContrato ConverterViewModelparaContrato(UsuarioViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            var retorno = new UsuarioContrato();
                retorno.Id = viewModel.Id;
                retorno.Nome = viewModel.Nome;
                retorno.Email = viewModel.Email;
                retorno.Senha = viewModel.Senha;
                retorno.Documento = viewModel.Documento;
                retorno.Telefone = viewModel.Telefone;
                retorno.TipoUsuario = viewModel.TipoUsuario;
                retorno.DataCadastro = viewModel.DataCadastro;
                retorno.UltimoAcesso = viewModel.UltimoAcesso;
                retorno.Ativo = viewModel.Ativo;
                retorno.Logradouro = viewModel.Logradouro;
                retorno.Numero = viewModel.Numero;
                retorno.Complemento = viewModel.Complemento;
                retorno.Bairro = viewModel.Bairro;
                retorno.Cep = viewModel.Cep;
                retorno.CidadeId = viewModel.CidadeId;

                return retorno;
        }
    }
}
