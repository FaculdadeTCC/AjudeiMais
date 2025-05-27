namespace AjudeiMais.Ecommerce.Models.CategoriaProduto
{
    public class CategoriaProdutoModel
    {
        public string Nome { get; set; }
        public IFormFile Icone { get; set; }
    }

    public class CategoriaProdutoResponse
    {
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
    }

    public class CategoriaProdutoUpdate
    {
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public IFormFile Icone { get; set; }
    }
}
