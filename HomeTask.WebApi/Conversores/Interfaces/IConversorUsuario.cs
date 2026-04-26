using HomeTask.Domain.Contratos;
using HomeTask.Domain.Entidades;
using HomeTask.Domain.ViewModel;

namespace HomeTask.WebApi.Conversores.Interfaces
{
    public interface IConversorUsuario
    {
        public UsuarioContrato ConverterViewModelparaContrato(UsuarioViewModel viewModel);
        public UsuarioViewModel? ConverterContratoparaViewModel(UsuarioContrato contrato);
        public Usuario? ConverterContratoparaUsuario(UsuarioContrato contrato);
        public UsuarioContrato ConverterUsuarioparaContrato(Usuario? usuario);

        // conversores do Usuario-Perfil
        public PerfilContrato ConverterPerfilViewModelparaPerfilContrato(PerfilViewModel viewModel);
        public PerfilViewModel? ConverterPerfilContratoparaPerfilViewModel(PerfilContrato contrato);
        public PerfilContrato ConverterUsuarioparaPerfilContrato(Usuario usuario, Prestador? prestador = null);
       
        //public Domain.Entities.Usuario? ConverterPerfilContratoparaUsuario(PerfilContrato contrato);

    }
}
