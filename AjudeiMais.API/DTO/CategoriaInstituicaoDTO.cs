using AjudeiMais.Data.Models.InstituicaoModel;
using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.API.DTO
{
    public class CategoriaInstituicaoDTO
    {
		public int Categoria_ID { get; set; }
		public string Nome { get; set; }
		public bool Habilitado { get; set; }
		public bool Excluido { get; set; }
		public string Icone { get; set; }
		public string? GUID { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataUpdate { get; set; }
	}
}
