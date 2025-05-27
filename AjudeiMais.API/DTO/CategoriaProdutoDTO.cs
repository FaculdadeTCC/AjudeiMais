namespace AjudeiMais.API.DTO
{
    public class CategoriaProdutoDTO
    {
        public int? CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public IFormFile Icone { get; set; }
        public DateTime? DataUpdate { get; set; }
        public bool? Habilitado { get; set; }
        public bool? Excluido { get; set; }
    }
}
