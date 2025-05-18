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
            var url = $"https://nominatim.openstreetmap.org/search?postalcode={cep}&city={cidade}&country=Brazil&format=json";

            var response = await _httpClient.GetStringAsync(url);

            var resultados = JsonSerializer.Deserialize<List<NominatimResultDto>>(response);

            if (resultados == null)
            {
                resultados.FirstOrDefault().Longitude = "";
                resultados.FirstOrDefault().Latitude = "";
            }

            return resultados?.FirstOrDefault();
        }
    }

}
