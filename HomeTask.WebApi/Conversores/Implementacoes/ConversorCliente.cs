using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;
using HomeTask.WebApi.Conversores.Interfaces;

namespace HomeTask.WebApi.Conversores.Implementacoes
{
    public class ConversorCliente : IConversorCliente
    {
        public ClienteContrato ConverterClienteparaContrato(Cliente? cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var retorno = new ClienteContrato();
                retorno.Id = cliente.Id;
                retorno.UsuarioId = cliente.UsuarioId;

                return retorno;
        }

        public Cliente? ConverterContratoparaCliente(ClienteContrato contrato)
        {
            if (contrato == null)
                throw new ArgumentNullException(nameof(contrato));

            var retorno = new Cliente();
                retorno.DefinirDados(contrato.Id, contrato.UsuarioId);

                return retorno;
        }

        public ClienteViewModel? ConverterContratoparaViewModel(ClienteContrato contrato)
        {
            if (contrato == null)
                return null;

            var retorno = new ClienteViewModel();
                retorno.Id = contrato.Id;
                retorno.UsuarioId = contrato.UsuarioId;

                return retorno;
        }

        public ClienteContrato ConverterViewModelparaContrato(ClienteViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            var retorno = new ClienteContrato();
                retorno.Id = viewModel.Id;
                retorno.UsuarioId = viewModel.UsuarioId;

                return retorno;
        }
    }
}
