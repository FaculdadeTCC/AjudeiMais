using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp.Formats.Webp;
using static System.Net.Mime.MediaTypeNames;

namespace AjudeiMais.API.Tools
{
    /// <summary>
    /// Classe utilitária com métodos auxiliares como cálculo de distância geográfica e manipulação de imagens.
    /// </summary>
    public static class Helpers
    {
        #region Calcular Distância

        /// <summary>
        /// Calcula a distância entre dois pontos geográficos (latitude/longitude) em quilômetros,
        /// utilizando a fórmula de Haversine.
        /// </summary>
        /// <param name="lat1">Latitude do primeiro ponto.</param>
        /// <param name="lon1">Longitude do primeiro ponto.</param>
        /// <param name="lat2">Latitude do segundo ponto.</param>
        /// <param name="lon2">Longitude do segundo ponto.</param>
        /// <returns>Distância entre os dois pontos em quilômetros.</returns>
        public static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Raio da Terra em km
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
        /// <returns>Ângulo em radianos.</returns>
        private static double ToRadians(double angle) => angle * Math.PI / 180;

        #endregion

        #region Imagens

        /// <summary>
        /// Salva uma imagem enviada pelo usuário no formato WebP com qualidade configurável.
        /// O arquivo é salvo em uma pasta específica dentro da aplicação.
        /// </summary>
        /// <param name="imagem">Arquivo de imagem enviado pelo formulário (IFormFile).</param>
        /// <param name="pastas">Caminho em pastas onde a imagem será salva (ex: ["images", "usuarios", "id"]).</param>
        /// <param name="qualidade">Qualidade da imagem WebP (0 a 100).</param>
        /// <returns>
        /// Caminho relativo do arquivo salvo (ex: "images/usuarios/123/abc.webp"),
        /// ou <c>null</c> se a imagem estiver vazia.
        /// </returns>
        public static async Task<string> SalvarImagemComoWebpAsync(
            IFormFile imagem,
            string[] pastas,
            int qualidade = 100
        )
        {
            if (imagem == null || imagem.Length == 0)
                return null;

            // Define os caminhos relativos e físicos
            string pastaRelativa = Path.Combine(pastas);
            string caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pastaRelativa);

            // Cria o diretório se não existir
            Directory.CreateDirectory(caminhoPasta);

            // Gera um nome único para o arquivo
            string nomeArquivo = Guid.NewGuid().ToString() + ".webp";
            string caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

            // Carrega a imagem e salva em WebP
            using (var streamEntrada = imagem.OpenReadStream())
            using (var imagemSharp = await SixLabors.ImageSharp.Image.LoadAsync(streamEntrada))
            {
                await imagemSharp.SaveAsync(caminhoCompleto, new WebpEncoder
                {
                    Quality = qualidade,
                });
            }

            // Retorna o caminho relativo (com barras de URL)
            string caminhoRelativo = Path.Combine(pastaRelativa, nomeArquivo).Replace("\\", "/");
            return caminhoRelativo;
        }

        #endregion
    }
}
