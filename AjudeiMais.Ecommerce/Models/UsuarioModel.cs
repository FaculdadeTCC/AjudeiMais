namespace AjudeiMais.Ecommerce.Models
{
    public class UsuarioModel
    {
        public string NomeCompleto { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }
    }

}
