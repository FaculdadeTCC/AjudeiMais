using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.Ecommerce.Models
{
    public class InstituicaoModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Telefone { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? Guid { get; set; }
        public string? Role { get; set; }
        public string? Avaliacao { get; set; }

        public EnderecoModel Endereco { get; set; }
            
        public List<IFormFile> Fotos { get; set; }
    }
}
