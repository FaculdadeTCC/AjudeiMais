﻿namespace AjudeiMais.Ecommerce.Models
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
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public string? TelefoneFixo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Role { get; set; } = "role";
        public IFormFile FotoDePerfil { get; set; }
    }

}
