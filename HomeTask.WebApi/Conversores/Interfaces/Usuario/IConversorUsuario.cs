using HomeTask.Domain.Contratos;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces.Usuario
{
    public interface IConversorUsuario
    {
        public UsuarioContrato ConverterViewModelparaContrato(UsuarioViewModel viewModel);
        public UsuarioViewModel? ConverterContratoparaViewModel(UsuarioContrato contrato);
        public HomeTask.Domain.Entities.Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato);
        public UsuarioContrato ConverterUsuarioparaContrato(HomeTask.Domain.Entities.Usuario? usuario);
    }
}
