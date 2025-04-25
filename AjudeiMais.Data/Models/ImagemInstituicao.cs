using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class ImagemInstituicao
    {
        [Key]
        public int InsituicaoImagem_ID { get; set; }
        public string Imagem { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        public bool Instituicao { get; set; }



    }
}
