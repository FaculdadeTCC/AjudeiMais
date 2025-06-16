namespace AjudeiMais.API.Services
{
    using System.Net.Http;
    using System.Text.Json;
    using AjudeiMais.API.Models;

    public class NominatimService
    {
        private readonly HttpClient _httpClient;

        public NominatimService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SeuSistemaDeDoacao/1.0 (seu@email.com)");
        }

        public async Task<NominatimResultDto> ObterCoordenadasPorCepAsync(string cep, string cidade)
        {
            try
            {
                string cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
                var url = $"https://nominatim.openstreetmap.org/search?postalcode={cepLimpo}&city={cidade}&country=Brazil&format=json";

                var response = await _httpClient.GetStringAsync(url);

                var resultados = JsonSerializer.Deserialize<List<NominatimResultDto>>(response);

                if (resultados == null || resultados.Count == 0)
                {
                    return new NominatimResultDto
                    {
                        Latitude = "",
                        Longitude = ""
                    };
                }

                return resultados.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Aqui você pode logar a exceção se quiser
                return new NominatimResultDto
                {
                    Latitude = "",
                    Longitude = ""
                };
            }
        }

    }

}
