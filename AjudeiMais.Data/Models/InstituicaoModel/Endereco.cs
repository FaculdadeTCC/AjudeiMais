using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models.InstituicaoModel
{
    public class Endereco
    {
        [Key]
        public int Endereco_ID { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }

        // Para relação 1:N com Instituicao (opcional)
        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        public Instituicao? Instituicao { get; set; }
    }
}
