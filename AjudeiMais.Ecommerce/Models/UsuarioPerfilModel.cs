namespace AjudeiMais.Ecommerce.Models
{
    public class UsuarioPerfilModel
    {
        public string NomeCompleto { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public string? TelefoneFixo { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string Role { get; set; }
        public string FotoDePerfil { get; set; }
        public string? GUID { get; set; }
        public IEnumerable<ProdutoModel> Produtos { get; set; }
    }
}
