using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.Data.Models.AvaliacaoModel
{
    public class Avaliacao
    {
        [Key]
        public int Avaliacao_ID { get; set; }
        public int Nota { get; set; }

        public string Comentario { get; set; }

        public DateTime DataAvaliacao { get; set; }
        [ForeignKey("Usuario")]
        public int Usuario_ID { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        public Instituicao Instituicao { get; set; }

        public TipoAvaliacao Tipo { get; set; } // Enum
        public bool Excluido { get; set; } 
        public bool Habilitado { get; set; }
    }

    public enum TipoAvaliacao
    {
        UsuarioParaInstituicao,
        InstituicaoParaUsuario
    }

}
