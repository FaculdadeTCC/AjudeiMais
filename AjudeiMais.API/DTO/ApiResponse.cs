namespace AjudeiMais.API.DTO
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; } // O tipo de dado que a API vai retornar (ex: um UsuarioDTO, ou null)
        public List<string>? Errors { get; set; } // Para múltiplos erros ou validações

        public ApiResponse()
        {
            Errors = new List<string>(); // Inicializa a lista para evitar NullReferenceException
        }
    }
}
