namespace AjudeiMais.API.Tools
{
    /// <summary>
    /// Classe utilitária com métodos auxiliares diversos.
    /// </summary>
    public static class Helpers
    {
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
    }
}
