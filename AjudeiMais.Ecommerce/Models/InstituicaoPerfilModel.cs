namespace AjudeiMais.Ecommerce.Models
{
    public class InstituicaoPerfilModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Telefone { get; set; }
        public string Documento { get; set; }
        public string FotoPerfil { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmarSenha { get; set; }
        public string? Guid { get; set; }
        public string? Role { get; set; }
        public string? Avaliacao { get; set; }

        public List<EnderecoModel> Enderecos { get; set; }
        public List<InstituicaoImagemModel>? FotosUrl { get; set; } = new();
    }
}
