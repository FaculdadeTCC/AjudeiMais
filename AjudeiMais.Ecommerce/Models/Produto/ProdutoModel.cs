using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Models.CategoriaProduto;

namespace AjudeiMais.Ecommerce.Models.Produto
{
    public class ProdutoModel
    {
        public int? Produto_ID { get; set; }

        public string Usuario_GUID { get; set; }
        public int? Usuario_ID { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
        public int? CategoriaProduto_ID { get; set; }

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        [StringLength(4000, ErrorMessage = "A descrição pode ter no máximo 4000 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A condição do produto é obrigatória.")]
        [StringLength(50)]
        public string Condicao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Validade { get; set; }

        [Required(ErrorMessage = "A quantidade disponível é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        [StringLength(50)]
        public string UnidadeMedida { get; set; }

        public bool? Disponivel { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataUpdate { get; set; }
        public IEnumerable<CategoriaProdutoResponse>? Categorias { get; set; }
        public decimal Peso { get; set; }
        public List<string>? Imagens { get; set; }
    }

    public class ProdutoEditarModel
    {
        public int? Produto_ID { get; set; }
        public string Guid { get; set; }

        public string Usuario_GUID { get; set; }
        public int? Usuario_ID { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
        public int? CategoriaProduto_ID { get; set; }

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        [StringLength(4000, ErrorMessage = "A descrição pode ter no máximo 4000 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A condição do produto é obrigatória.")]
        [StringLength(50)]
        public string Condicao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Validade { get; set; }

        [Required(ErrorMessage = "A quantidade disponível é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        [StringLength(50)]
        public string UnidadeMedida { get; set; }

        public bool? Disponivel { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataUpdate { get; set; }
        public IEnumerable<CategoriaProdutoResponse>? Categorias { get; set; }
        public decimal Peso { get; set; }
        public List<ProdutoImagemModel>? ProdutoImagens { get; set; }
        public UsuarioPerfilModel Usuario { get; set; }
        public List<IFormFile> NovasImagensUpload { get; set; }
    }
    public class ProdutoViewModel
    {
        public UsuarioPerfilModel Usuario;
        public IEnumerable<CategoriaProdutoResponse> Categorias;
    }

    public class ProdutoResponse
    {
        public int Produto_ID { get; set; }
        public string Guid { get; set; }
        public string Usuario_GUID { get; set; }
        public string Nome { get; set; }
        public int? CategoriaProduto_ID { get; set; }
        public string Descricao { get; set; }
        public string Condicao { get; set; }
        public DateTime? Validade { get; set; }
        public int Quantidade { get; set; }
        public string UnidadeMedida { get; set; }
        public bool? Disponivel { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataUpdate { get; set; }
        public decimal? Peso { get; set; }
        public IEnumerable<ProdutoImagemModel>? ProdutoImagens { get; set; }
        public CategoriaProdutoDto? CategoriaProduto { get; set; }
    }

    public class CategoriaProdutoDto
    {
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
    }

    public class ProdutosProximosDto
    {
        public int Produto_ID { get; set; }
        public string Guid { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? Condicao { get; set; }
        public DateTime? Validade { get; set; }
        public int Quantidade { get; set; }
        public decimal? Peso { get; set; }
        public bool Disponivel { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }
        public string UnidadeMedida { get; set; }

        public ICollection<ProdutoImagemResponseDto> ProdutoImagens { get; set; } = new List<ProdutoImagemResponseDto>();
        public UsuarioResumoDTO Usuario { get; set; }
        public CategoriaProdutoResponse CategoriaProduto { get; set; }
    }

    public class ProdutoImagemUploadDto
    {
        [Required]
        public int Produto_ID { get; set; }

        [Required]
        public IFormFile Imagem { get; set; }

        [Required]
        public bool IsPrincipal { get; set; }
    }
    public class ProdutoImagemResponseDto
    {
        [Required]
        public int Produto_ID { get; set; }

        [Required]
        public string Imagem { get; set; }

        [Required]
        public bool IsPrincipal { get; set; }
    }

    public class ProdutoImagemModel
    {
        public int ProdutoImagem_ID { get; set; }
        public string Imagem { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public bool IsPrincipal { get; set; }

        public int Produto_ID { get; set; }
        public ProdutoModel? Produto { get; set; }
    }

}
