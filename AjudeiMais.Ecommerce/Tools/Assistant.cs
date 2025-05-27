using System;
using Microsoft.AspNetCore.Http; // Necessary for IFormFile
using System.Net.Http;
using System.Security.Claims; // Necessary for ClaimTypes and ClaimsPrincipal
using System.Reflection; // Necessary for PropertyInfo

namespace AjudeiMais.Ecommerce.Tools
{
    /// <summary>
    /// Auxiliary class with extension methods to facilitate the manipulation of MultipartFormDataContent,
    /// especially for adding files and objects with simple properties in HTTP requests.
    /// And other utility methods.
    /// </summary>
    public static class Assistant
    {
        public static string ServerURL()
        {
            // This URL should ideally be loaded from configuration (e.g., appsettings.json)
            // rather than hardcoded, for easier environment management.
            return "https://localhost:7271/";
        }

        /// <summary>
        /// Attempts to retrieve the user's GUID from the claims of the authenticated user.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the authenticated user.</param>
        /// <param name="claimTypeForGuid">The type of the claim where the user's GUID is stored (e.g., ClaimTypes.NameIdentifier).</param>
        /// <returns>The user's GUID as a string, or null if not found.</returns>
        public static string GetUserGuidFromClaims(ClaimsPrincipal user, string claimTypeForGuid = ClaimTypes.NameIdentifier)
        {
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            // Tries to find the claim that stores the user's GUID
            var guidClaim = user.FindFirst(claimTypeForGuid)?.Value;

            return guidClaim;
        }

        /// <summary>
        /// Extension method to add a file (IFormFile) to MultipartFormDataContent.
        /// </summary>
        /// <param name="formData">The MultipartFormDataContent object that will receive the file.</param>
        /// <param name="file">The file to be added (IFormFile).</param>
        /// <param name="fieldName">The form-data field name that will be used for the file.</param>
        public static void AddFileContent(this MultipartFormDataContent formData, IFormFile file, string fieldName)
        {
            if (file == null || file.Length == 0)
                return; // Do not add if file is null or empty

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            formData.Add(streamContent, fieldName, file.FileName);
        }

        /// <summary>
        /// Método de extensão que adiciona todas as propriedades públicas simples (string, int, bool, etc)
        /// de um objeto genérico como campos string no MultipartFormDataContent.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser convertido em campos form-data.</typeparam>
        /// <param name="formData">O MultipartFormDataContent que receberá os campos.</param>
        /// <param name="obj">O objeto com as propriedades a serem adicionadas.</param>
        public static void AddObjectAsFormFields(this MultipartFormDataContent formData, object obj, string prefix = "")
        {
            if (obj == null)
                return;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Ignora propriedades que exigem parâmetros (ex: indexadores)
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                // Ignora arquivos
                if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                    continue;

                var value = prop.GetValue(obj);
                if (value == null)
                    continue;

                string propName = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

                if (prop.PropertyType.IsClass && !(value is string))
                {
                    formData.AddObjectAsFormFields(value, propName); // recursão para objetos aninhados
                }
                else
                {
                    formData.Add(new StringContent(value.ToString()), propName);
                }
            }
        }

        /// <summary>
        /// Checks if a type is considered "simple" for sending as a string field.
        /// Simple types include primitives, string, decimal, DateTime, Guid, TimeSpan, and their nullables.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>True if it's a simple type, false otherwise.</returns>
        private static bool IsSimpleType(Type type)
        {
            // Handle Nullable types
            Type actualType = Nullable.GetUnderlyingType(type) ?? type;

            return
                actualType.IsPrimitive ||
                actualType == typeof(string) ||
                actualType == typeof(decimal) ||
                actualType == typeof(DateTime) ||
                actualType == typeof(Guid) ||
                actualType == typeof(DateTimeOffset) ||
                actualType == typeof(TimeSpan);
        }
    }
}