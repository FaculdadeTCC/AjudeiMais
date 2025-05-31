using AjudeiMais.Ecommerce.Models.Produto;

namespace AjudeiMais.Ecommerce.Models.Home
{
    public class HomeModel
    {
        public List<ProdutosProximosDto>? Anuncios { get; set; }
        public string? ErrorMessage { get; set; } // To display API errors in the view
    }
}
