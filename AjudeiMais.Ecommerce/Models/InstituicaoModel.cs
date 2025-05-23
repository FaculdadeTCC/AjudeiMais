using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.Ecommerce.Models
{
    public class InstituicaoModel
    {
        public class Instituicao
        {
            [Key]
            public int Instituicao_ID { get; set; }
            public string Nome { get; set; }
            public string Descricao { get; set; }
            public string Telefone { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
            public string? Guid { get; set; }
            public string? Role { get; set; }
            public string? Avaliacao { get; set; }
            public DateTime DataCriacao { get; set; }
            public DateTime DataUpdate { get; set; }
        }
    }
}
