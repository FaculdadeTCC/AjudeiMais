using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models.InstituicaoModel
{
    public class Instituicao
    {
        [Key]
        public int Instituicao_ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Guid { get; set; }
        public string Avaliacao { get; set; }
        public ICollection<Endereco> Enderecos { get; set;}
        public ICollection<InstituicaoCategoria> InstituicaoCategorias { get; set; }
    }
}
