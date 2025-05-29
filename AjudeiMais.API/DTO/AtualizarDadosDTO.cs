namespace AjudeiMais.API.DTO
{
    public class AtualizarInstituicaoDTO
    {
        public InstituicaoDadosDTO Instituicao { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public InstituicaoSenhaDTO Senha { get; set; }
    }
    public class InstituicaoDadosDTO
    {
        public string? GUID { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public IFormFile? FotoPerfil { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Descricao { get; set; }
        public List<IFormFile> Fotos { get; set; } = new List<IFormFile>();
    }

    public class InstituicaoSenhaDTO
    {
        public string GUID { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmarNovaSenha { get; set; }
    }

    public class InstituicaoValidarSenhaDTO
    {
        public string GUID { get; set; }
        public string Senha { get; set; }
    }
}
