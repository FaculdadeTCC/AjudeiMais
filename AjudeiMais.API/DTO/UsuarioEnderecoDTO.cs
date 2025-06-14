﻿namespace AjudeiMais.API.DTO
{
    public class UsuarioEnderecoDTO
    {
        public int? Usuario_ID { get; set; }
        public string? GUID { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
