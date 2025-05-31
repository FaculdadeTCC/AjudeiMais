using AjudeiMais.API.DTO;
using System.ComponentModel.DataAnnotations;

public class ProdutoPostDto
{
    public int? Produto_ID { get; set; }
    public string Usuario_GUID { get; set; }

    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
    public int? CategoriaProduto_ID { get; set; }

    public List<CategoriaProdutoDTO>? Categorias { get; set; }

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


    public decimal? Peso { get; set; }

}
