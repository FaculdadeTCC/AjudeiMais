﻿namespace AjudeiMais.Ecommerce.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? GUID { get; set; }
        public int? UserType { get; set; }
    }
}
