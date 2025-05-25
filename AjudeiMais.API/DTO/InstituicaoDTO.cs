using AjudeiMais.API.DTO;

public class InstituicaoDTO
{
    public int Instituicao_ID { get; set; }
    public string Nome { get; set; }
    public string CNPJ { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }

    public string Telefone { get; set; }
    public string GUID { get; set; }

    public List<EnderecoDTO> Enderecos { get; set; }

    public IFormFile[] Imagens { get; set; } // múltiplas imagens
}
