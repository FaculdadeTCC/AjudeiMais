namespace AjudeiMais.Ecommerce.Models.CategoriaProduto
{
    public class CategoriaProdutoModel
    {
        public int? CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
        public bool Habilitado { get; set; }
    }

    public class CategoriaProdutoResponse
    {
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
        public bool? Habilitado { get; set; }
        public bool? Excluido { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataUpdate { get; set; }
    }

    public class CategoriaProdutoUpdate
    {
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public IFormFile Icone { get; set; }
    }
}
