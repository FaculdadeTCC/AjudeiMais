using AjudeiMais.Data.Models.AvaliacaoModel;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AjudeiMais.API.DTO
{
    public class AvaliacaoPostDTO
    {
        public int? Avaliacao_ID { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public DateTime? DataAvaliacao { get; set; }
        public int Usuario_ID { get; set; }
        public int Instituicao_ID { get; set; }
        public TipoAvaliacao Tipo { get; set; }
        public bool? Habilitado { get; set; }
    }

    public enum TipoAvaliacao
    {
        UsuarioParaInstituicao,
        InstituicaoParaUsuario
    }
}
