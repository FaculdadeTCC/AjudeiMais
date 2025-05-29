using AjudeiMais.Data.Models.InstituicaoModel;
using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.API.DTO
{
    public class CategoriaDTO
    {
        public int Categoria_ID { get; set; }
        public string Nome { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public IFormFile Icone { get; set; }
        public string? GUID  { get; set; }
        public ICollection<InstituicaoCategoria>? InstituicaoCategorias { get; set; }
    }

    public class CategoriaDtoGet
    {
        public int Categoria_ID { get; set; }
        public string Nome { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public string Icone { get; set; }
        public string? GUID { get; set; }
        public ICollection<InstituicaoCategoria>? InstituicaoCategorias { get; set; }
    }
}
