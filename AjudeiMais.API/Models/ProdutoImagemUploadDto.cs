namespace AjudeiMais.API.Models
{
    public class ProdutoImagemUploadDto
    {
        public int ProdutoImagem_ID { get; set; }
        public IFormFile Imagem { get; set; }
        public int Produto_ID { get; set; }
        public bool IsPrincipal { get; set; }
    }

}
