namespace HomeTask.Domain.Enums;

/// <summary>
/// Define se o anúncio foi criado por um prestador oferecendo serviço
/// ou por um cliente solicitando serviço
/// </summary>
public enum TipoAnuncio
{
    Oferta = 1 ,  // prestador oferece o serviço
    Pedido = 2  // cliente solicita o serviço 
}