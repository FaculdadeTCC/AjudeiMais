using AjudeiMais.Data.Models.ProdutoModel;

namespace AjudeiMais.API.DTO
{
    public class UsuarioResumoDTO
    {
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string? GUID { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string FotoDePerfil { get; set; }
        public string Telefone { get; set; }
        public string? TelefoneFixo { get; set; }
    }
}
