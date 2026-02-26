using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomeTask.Domain.Enums;

namespace HomeTask.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<CategoriaViewModel> Categorias { get; set; } = [];

        public void OnGet()
        {
            Categorias =
            [
                new CategoriaViewModel { Id = (int)CategoriaServico.Faxina, Nome = "Faxina", Descricao = "Limpeza residencial completa", Icone = "bi bi-house" },
                new CategoriaViewModel { Id = (int)CategoriaServico.Jardinagem, Nome = "Jardinagem", Descricao = "Cuidados com jardim e plantas", Icone = "bi bi-flower1" },
                new CategoriaViewModel { Id = (int)CategoriaServico.Reparos, Nome = "Reparos", Descricao = "Manutenção e consertos", Icone = "bi bi-tools" },
                new CategoriaViewModel { Id = (int)CategoriaServico.Lavanderia, Nome = "Lavanderia", Descricao = "Lavagem e cuidados com roupas", Icone = "bi bi-basket" },
                new CategoriaViewModel { Id = (int)CategoriaServico.Babysitter, Nome = "Babá", Descricao = "Cuidados com crianças", Icone = "bi bi-emoji-smile" },
                new CategoriaViewModel { Id = (int)CategoriaServico.PetSitter, Nome = "Pet Sitter", Descricao = "Cuidados com animais", Icone = "bi bi-hearts" },
                new CategoriaViewModel { Id = (int)CategoriaServico.Cozinheiro, Nome = "Cozinheiro", Descricao = "Preparo de refeições", Icone = "bi bi-cup-hot" },
                new CategoriaViewModel { Id = (int)CategoriaServico.ServicosGerais, Nome = "Serviços Gerais", Descricao = "Diversos serviços domésticos", Icone = "bi bi-gear" }
            ];
        }
    }

    public class CategoriaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Icone { get; set; } = string.Empty;
    }
}
