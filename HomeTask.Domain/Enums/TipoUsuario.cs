namespace HomeTask.Domain.Enums;

/// <summary>
/// Tipo de usuário na plataforma HomeTask.
/// Um usuário pode ser Cliente, Prestador ou ambos ao mesmo tempo
/// mantendo um único cadastro (e-mail e CPF únicos).
/// </summary>
public enum TipoUsuario
{
    Cliente   = 1,
    Prestador = 2,
    Ambos     = 3
}
