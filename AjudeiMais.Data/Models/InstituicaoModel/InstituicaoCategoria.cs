using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AjudeiMais.Data.Models.InstituicaoModel
{
    public class InstituicaoCategoria
    {
        [Key]
        public int InstituicaoCategoria_ID { get; set; }

        // Atributo de chave estrangeira, não precisa de ForeignKey explicitamente se seguir a convenção
        public int Instituicao_ID { get; set; }
        public Instituicao? Instituicao { get; set; }

        public int Categoria_ID { get; set; }
        public CategoriaInstituicao? Categoria { get; set; }
    }
}
