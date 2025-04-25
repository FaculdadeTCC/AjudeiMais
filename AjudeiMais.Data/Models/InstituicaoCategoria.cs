using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class InstituicaoCategoria
    {
        public int InstituicaoCategoria_ID { get; set; }

        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        public Instituicao Instituicao { get; set; }

        [ForeignKey("Categoria")]
        public int Categoria_ID { get; set; }
        public Categoria Categoria { get; set; }

    }
}
