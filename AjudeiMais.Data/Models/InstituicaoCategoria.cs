using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class InstituicaoCategoria
    {
        public int InstituicaoCategoria_ID { get; set; }

        public int Instituicao_ID { get; set; }
        public Instituicao Instituicao { get; set; }

        public int Categoria_ID { get; set; }
        public Categoria Categoria { get; set; }

    }
}
