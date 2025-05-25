using AjudeiMais.API.DTO;
using Microsoft.AspNetCore.Mvc;

public class InstituicaoDTO
{
    public int Instituicao_ID { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Telefone { get; set; }

    [ModelBinder(BinderType = typeof(FromJsonBinder<List<EnderecoDTO>>))]
    public List<EnderecoDTO> Enderecos { get; set; }

}
