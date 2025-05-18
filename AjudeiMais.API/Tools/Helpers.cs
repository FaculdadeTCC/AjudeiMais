using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp.Formats.Webp;
using static System.Net.Mime.MediaTypeNames;

namespace AjudeiMais.API.Tools
{
    /// <summary>
    /// Classe utilitária com métodos auxiliares diversos.
    /// </summary>
    public static class Helpers
    {
        #region Calcular Distancia
        /// <summary>
        /// Calcula a distância entre dois pontos geográficos em quilômetros utilizando a fórmula de Haversine.
        /// </summary>
        /// <param name="lat1">Latitude do primeiro ponto em graus decimais.</param>
        /// <param name="lon1">Longitude do primeiro ponto em graus decimais.</param>
        /// <param name="lat2">Latitude do segundo ponto em graus decimais.</param>
        /// <param name="lon2">Longitude do segundo ponto em graus decimais.</param>
        /// <returns>Distância entre os dois pontos em quilômetros.</returns>
        public static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Raio da Terra em KM
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Converte um ângulo de graus para radianos.
        /// </summary>
        /// <param name="angle">Ângulo em graus.</param>
        /// <returns>Ângulo convertido em radianos.</returns>
        private static double ToRadians(double angle) => angle * Math.PI / 180;
        #endregion

        #region Imagens
        public static async Task<string> SalvarImagemComoWebpAsync(
            IFormFile imagem,
            string[] pastas,        // Exemplo: new[] { "images", "profile", "usuario", usuarioGuid.ToString() }
            int qualidade = 100      // Qualidade da imagem WebP entre 0 e 100
        )
        {
            if (imagem == null || imagem.Length == 0)
                return null;

            // Caminho relativo (salvo no banco) e caminho físico
            string pastaRelativa = Path.Combine(pastas);
            string caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pastaRelativa);

            // Garante que a pasta exista
            Directory.CreateDirectory(caminhoPasta);

            // Nome do arquivo gerado
            string nomeArquivo = Guid.NewGuid().ToString() + ".webp";
            string caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

            // Conversão para WebP usando ImageSharp
            using (var streamEntrada = imagem.OpenReadStream())
            using (var imagemSharp = await SixLabors.ImageSharp.Image.LoadAsync(streamEntrada))
            {
                await imagemSharp.SaveAsync(caminhoCompleto, new WebpEncoder
                {
                    Quality = qualidade,
                });
            }

            // Retorna o caminho relativo com separadores de URL
            string caminhoRelativo = Path.Combine(pastaRelativa, nomeArquivo).Replace("\\", "/");
            return caminhoRelativo;
        }

        #endregion

    }
}
