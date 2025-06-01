using System;
using System.ComponentModel.DataAnnotations; // Don't forget to include this namespace!
using AjudeiMais.Ecommerce.Models.Instituicao; // Assuming these are needed for context, though not directly annotated here
using AjudeiMais.Ecommerce.Models.Usuario;     // Assuming these are needed for context, though not directly annotated here

namespace AjudeiMais.Ecommerce.Models.Avaliacao
{
    public class AvaliacaoModel
    {
        [Key] // Denotes the primary key of the entity
        public int? Avaliacao_ID { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória.")]
        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5.")]
        public int Nota { get; set; }

        [MaxLength(500, ErrorMessage = "O comentário não pode exceder 500 caracteres.")]
        [DataType(DataType.MultilineText)] // Suggests a multi-line input field in UI
        public string Comentario { get; set; }

        public DateTime? DataAvaliacao { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do usuário deve ser um número positivo.")]
        public int Usuario_ID { get; set; }

        [Required(ErrorMessage = "O ID da instituição é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID da instituição deve ser um número positivo.")]
        public int Instituicao_ID { get; set; }

        [Required(ErrorMessage = "O tipo de avaliação é obrigatório.")]
        public TipoAvaliacao Tipo { get; set; }
        public bool? Habilitado { get; set; }
    }

    public enum TipoAvaliacao
    {
        UsuarioParaInstituicao,
        InstituicaoParaUsuario
    }
}