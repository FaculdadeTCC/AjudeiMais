namespace AjudeiMais.Ecommerce.Models.Usuario
{
    public class UsuarioEndereco
    {
        public int Usuario_ID { get; set; }
        public string? GUID { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}
