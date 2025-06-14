using System;
using System.Collections.Generic; // Para IEnumerable
using AjudeiMais.Ecommerce.Models.Produto;
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace AjudeiMais.Ecommerce.Models.Usuario
{
    public class UsuarioPerfilModel
    {
        // Propriedades do perfil (presentes em ambos)
        public int? Usuario_ID { get; set; }
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
        public DateTime? DataCriacao { get; set; }
        public string Role { get; set; } = "role"; // Defina um valor padrão razoável, se aplicável

        // Propriedades específicas de cadastro/login
        public string? Senha { get; set; } // Nullable, pois não será sempre necessário para atualização

        // Propriedades que variam em tipo ou uso
        public string? GUID { get; set; } // Pode ser nulo no cadastro inicial
        public string? FotoDePerfil { get; set; } // Para exibir a URL da foto existente

        // Propriedades de geolocalização (ajustando o tipo)
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        // Propriedades de navegação (para exibição de dados relacionados)
        public IEnumerable<ProdutoModel>? Produtos { get; set; } // Nullable, pois nem sempre virá carregado
    }
}