namespace AjudeiMais.API.Models
{
    public class ProdutoImagemUploadDto
    {
        public int ProdutoImagem_ID { get; set; }
        public IFormFile Imagem { get; set; }
        public int ProdutoId { get; set; }
    }

}
