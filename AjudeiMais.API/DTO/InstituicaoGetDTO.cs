using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.DTO
{
    public class InstituicaoGetDTO
    {
        public int Instituicao_ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Documento { get; set; }
        public string FotoPerfil { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string? GUID { get; set; }
        public string Role { get; set; }
        public List<EnderecoDTO>? Enderecos { get; set; }

        public List<InstituicaoImagemDTO>? FotosUrl { get; set; }
    }
}
