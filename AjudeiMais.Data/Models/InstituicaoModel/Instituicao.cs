using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.Data.Models.InstituicaoModel
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
        public string Guid { get; set; }
        public string Role { get; set; }
        public string? Avaliacao { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }
        
        public ICollection<InstituicaoImagem> instituicaoImagems { get; set; } = new List<InstituicaoImagem>();

        public ICollection<Endereco>? Enderecos { get; set; } = new List<Endereco>();
        public ICollection<InstituicaoCategoria>? InstituicaoCategorias { get; set; }

    }
}
