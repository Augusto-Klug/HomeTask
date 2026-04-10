using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorUsuario
    {
        public UsuarioContrato ConverterViewModelparaContrato(UsuarioViewModel viewModel);
        public UsuarioViewModel? ConverterContratoparaViewModel(UsuarioContrato contrato);
        public Domain.Entities.Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato);
        public UsuarioContrato ConverterUsuarioparaContrato(Domain.Entities.Usuario? usuario);
    }
}
