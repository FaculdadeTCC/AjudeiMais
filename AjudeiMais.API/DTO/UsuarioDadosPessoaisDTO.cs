namespace AjudeiMais.API.DTO
{
    public class UsuarioDadosPessoaisDTO
    {
        public int Usuario_ID { get; set; }
        public string GUID { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? TelefoneFixo { get; set; }
        public string? Documento { get; set; }
        public IFormFile? FotoDePerfil { get; set; }
    }
}
