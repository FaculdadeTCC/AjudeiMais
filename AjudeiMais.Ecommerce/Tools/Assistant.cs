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
        /// Extension method that adds all simple public properties (string, int, bool, etc.)
        /// of a generic object as string fields in MultipartFormDataContent.
        /// </summary>
        /// <typeparam name="T">Type of the object to be converted into form-data fields.</typeparam>
        /// <param name="formData">The MultipartFormDataContent that will receive the fields.</param>
        /// <param name="obj">The object with the properties to be added.</param>
        public static void AddObjectAsFormFields<T>(this MultipartFormDataContent formData, T obj)
        {
            if (obj == null) return;

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