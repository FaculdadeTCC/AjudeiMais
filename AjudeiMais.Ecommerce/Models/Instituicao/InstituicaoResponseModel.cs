using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AjudeiMais.Ecommerce.Models.Instituicao
{
    public class InstituicaoResponseModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public int? Instituicao_ID { get; set; }
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Informe um número de telefone válido.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Uma foto de perfil é obrigatória.")]
        public string FotoPerfil { get; set; }

        [Required(ErrorMessage = "O documento é obrigatório.")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo 20 caracteres.")]
        public string Documento { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A confirmação da senha é obrigatória.")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

        public string? Guid { get; set; }

        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Role { get; set; }

        public double? Avaliacao { get; set; }
        public List<IFormFile> Fotos { get; set; } = new List<IFormFile>();
        public List<string>? InstituicaoImagems { get; set; } = new List<string>();
        public List<EnderecoModel>? Enderecos { get; set; } = new List<EnderecoModel>();
        public List<CategoriaModel>? InstituicaoCategorias { get; set; } = new List<CategoriaModel>();
    }
}
