public class ProdutoResponseDto
{
    public int Produto_ID { get; set; }
    public string Guid { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Condicao { get; set; }
    public DateTime? Validade { get; set; }
    public int Quantidade { get; set; }
    public double Peso { get; set; }
    public bool Disponivel { get; set; }
    public bool Habilitado { get; set; }
    public bool Excluido { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataUpdate { get; set; }
    public string UnidadeMedida { get; set; }
    public List<string> Imagens { get; set; }

    public CategoriaProdutoDto CategoriaProduto { get; set; }
    public UsuarioDto Usuario { get; set; }
}

public class CategoriaProdutoDto
{
    public int CategoriaProduto_ID { get; set; }
    public string Nome { get; set; }
    public string Icone { get; set; }
}

public class UsuarioDto
{
    public int Usuario_ID { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Guid { get; set; }
    public string Telefone { get; set; }
    public string FotoDePerfil { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
}