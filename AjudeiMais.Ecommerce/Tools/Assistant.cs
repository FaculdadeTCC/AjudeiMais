using System;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims; // Necessary for ClaimTypes and ClaimsPrincipal
using System.Reflection; // Necessary for PropertyInfo

namespace AjudeiMais.Ecommerce.Tools
{
    /// <summary>
    /// Classe auxiliar com métodos de extensão para facilitar a manipulação de MultipartFormDataContent,
    /// especialmente para adicionar arquivos e objetos com propriedades simples em requisições HTTP.
    /// </summary>
    public static class Assistant
    {

        public static string ImageServerURL()
        {
            return "http://localhost:5168/";
        }
        /// <summary>
        /// Método de extensão para adicionar um arquivo (IFormFile) ao MultipartFormDataContent.
        /// </summary>
        /// <param name="formData">O objeto MultipartFormDataContent que receberá o arquivo.</param>
        /// <param name="file">O arquivo a ser adicionado (IFormFile).</param>
        /// <param name="fieldName">O nome do campo/form-data que será usado para o arquivo.</param>
        public static void AddFileContent(this MultipartFormDataContent formData, IFormFile file, string fieldName)
        {
            if (file == null || file.Length == 0)
                return; // Não adiciona se arquivo for nulo ou vazio

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            formData.Add(streamContent, fieldName, file.FileName);
        }

        /// <summary>
        /// Método de extensão que adiciona todas as propriedades públicas simples (string, int, bool, etc)
        /// de um objeto genérico como campos string no MultipartFormDataContent.
        /// </summary>
        /// <typeparam name="T">Type of the object to be converted into form-data fields.</typeparam>
        /// <param name="formData">The MultipartFormDataContent that will receive the fields.</param>
        /// <param name="obj">The object with the properties to be added.</param>
        public static void AddObjectAsFormFields<T>(this MultipartFormDataContent formData, T obj)
        {
            if (obj == null)
                return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Ensure the property is readable and is a simple type
                if (prop.CanRead && IsSimpleType(prop.PropertyType))
                {
                    var value = prop.GetValue(obj);
                    var stringValue = value?.ToString() ?? string.Empty;
                    formData.Add(new StringContent(stringValue), prop.Name);
                }
            }
        }




        /// <summary>
        /// Verifica se um tipo é considerado "simples" para envio como campo string.
        /// Tipos simples incluem primitivos, string, decimal, DateTime, Guid, TimeSpan, e seus nullables.
        /// </summary>
        /// <param name="type">O tipo a ser verificado.</param>
        /// <returns>True se for tipo simples, false caso contrário.</returns>
        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(Guid) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                (Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type)));
        }
    }
}
