using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class Categoria
    {
        [Key]
        public int Categoria_ID { get; set; }
        public string Nome { get; set; }
        public bool Habilitado { get; set; }
        public bool Exluido { get; set; }
        public string Icone { get; set; }

        public ICollection<InstituicaoCategoria> InstituicaoCategorias { get; set; }
    }
}
