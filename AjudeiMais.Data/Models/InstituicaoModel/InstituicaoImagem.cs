using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AjudeiMais.Data.Models.InstituicaoModel
{
    public class InstituicaoImagem
    {
        [Key]
        public int InsituicaoImagem_ID { get; set; }
        public string CaminhoImagem { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }

        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        
        [JsonIgnore]
        public Instituicao? Instituicao { get; set; }


    }
}
