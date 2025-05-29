
namespace AjudeiMais.Ecommerce.Controllers
{
    internal class CategoriaDTO
    {
        public int categoria_ID { get; set; }
        public string nome { get; set; }
        public bool habilitado { get; set; }
        public bool excluido { get; set; }
        public IFormFile icone { get; set; }
    }
}