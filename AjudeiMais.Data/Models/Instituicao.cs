using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class Instituicao
    {
        [Key]
        public int Instituicao_ID { get; set; }

        public ICollection<Endereco> Enderecos { get; set; }
    }
}
