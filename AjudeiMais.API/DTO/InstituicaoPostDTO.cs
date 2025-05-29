using AjudeiMais.API.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.DTO
{
    public class InstituicaoPostDTO
    {
        public int Instituicao_ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public IFormFile FotoPerfil { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string? GUID { get; set; }
        public string Role { get; set; }

        [ModelBinder(BinderType = typeof(FromJsonBinder<List<EnderecoDTO>>))]
        public List<EnderecoDTO> Enderecos { get; set; }

        public IFormFile[] Fotos { get; set; }
    }

    public class AtualizarDadosDTO
    {
        public string? GUID { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public IFormFile FotoPerfil { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Descricao { get; set; }
        public string Role { get; set; }

    }

    public class AtualizarFotosInstituicaoDTO
    {
        public IFormFile[] Fotos { get; set; }

        public string Instituicao_GUID { get; set; }
    }
}
