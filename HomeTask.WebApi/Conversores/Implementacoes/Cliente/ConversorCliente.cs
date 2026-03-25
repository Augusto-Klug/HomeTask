using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces.Cliente;

namespace HomeTask.WebApi.Conversores.Implementacoes.Cliente
{
    public class ConversorCliente : IConversorCliente
    {
        public ClienteContrato ConverterClienteparaContrato(Domain.Entities.Cliente? cliente)
        {
            if(cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var retorno = new ClienteContrato();
                retorno.Id = cliente.Id;
                retorno.UsuarioId = cliente.UsuarioId;
                retorno.Documento = cliente.Documento;
                retorno.Endereco = cliente.Endereco;
                retorno.Cidade = cliente.Cidade;
                retorno.Estado = cliente.Estado;
                retorno.Cep = cliente.Cep;
                retorno.Bairro = cliente.Bairro;

                return retorno;
        }

        public Domain.Entities.Cliente? ConverterContratoparaCliente(ClienteContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Domain.Entities.Cliente();

                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;
                retorno.Documento = contrato.Documento;
                retorno.Endereco = contrato.Endereco;
                retorno.Cidade = contrato.Cidade;
                retorno.Estado = contrato.Estado;
                retorno.Cep = contrato.Cep;
                retorno.Bairro = contrato.Bairro;

                return retorno;
        }

        public ClienteViewModel? ConverterContratoparaViewModel(ClienteContrato contrato)
        {
            if (contrato != null)
            {
                var retorno = new ClienteViewModel();

                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;
                retorno.Documento = contrato.Documento;
                retorno.Endereco = contrato.Endereco;
                retorno.Cidade = contrato.Cidade;
                retorno.Estado = contrato.Estado;
                retorno.Cep = contrato.Cep;
                retorno.Bairro = contrato.Bairro;

                return retorno;
            }
            return null;
        }

        public ClienteContrato ConverterViewModelparaContrato(ClienteViewModel viewModel)
        {
            if (viewModel != null)
            {
                var retorno = new ClienteContrato();

                retorno.Id = viewModel.Id;
                retorno.UsuarioId = viewModel.UsuarioId;
                retorno.Documento = viewModel.Documento;
                retorno.Endereco = viewModel.Endereco;
                retorno.Cidade = viewModel.Cidade;
                retorno.Estado = viewModel.Estado;
                retorno.Cep = viewModel.Cep;
                retorno.Bairro = viewModel.Bairro;

                return retorno;
            }
            return null;

        }
    }
}
