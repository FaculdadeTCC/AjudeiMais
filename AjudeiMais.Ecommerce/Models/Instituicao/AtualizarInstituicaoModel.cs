namespace AjudeiMais.Ecommerce.Models.Instituicao
{
    public class AtualizarInstituicaoModel
    {
        public InstituicaoDadosModel Instituicao { get; set; }
        public EnderecoModel Endereco { get; set; }
        public InstituicaoSenhaModel Senha { get; set; }
    }
    public class InstituicaoDadosModel
    {
        public string? GUID { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public IFormFile FotoPerfil { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Descricao { get; set; }
    }

    public class InstituicaoSenhaModel
    {
        public string GUID { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmarNovaSenha { get; set; }

    }
    public class InstituicaoExcluirContaDTO
    {
        public string GUID { get; set; }
        public string Senha { get; set; }
    }
}
