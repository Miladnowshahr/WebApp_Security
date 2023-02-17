using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_Security.Models;

namespace WebApp_Security.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherDto> Weathers { get; set; }
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {


            var httpClient = _httpClientFactory.CreateClient("OurWebApi");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", (await Autheneticate()).AccessToken);
            Weathers = await httpClient.GetFromJsonAsync<List<WeatherDto>>("WeatherForecast");


        }


        private async Task<Token> GetToken()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebApi");
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { Password = "nematpour", UserName = "milad" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<Token>(strJwt);
        }

        private async Task<Token> Autheneticate()
        {
            Token token = null;
            var strToken = HttpContext.Session.GetString("access_token");
            if (strToken != null)
                token = System.Text.Json.JsonSerializer.Deserialize<Token>(strToken);
            else
                token = await GetToken();

            if (token == null
                || string.IsNullOrWhiteSpace(token.AccessToken)
                || token.ExpiredAt <= DateTime.UtcNow)
                token = System.Text.Json.JsonSerializer.Deserialize<Token>(strToken);

            return token;
        }
    }
}
