using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace api_flms_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIChatController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AIChatController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            //string Encrypt(string text, string key) => new string(text.Select((c, i) => (char)(c ^ key[i % key.Length])).ToArray());
            string Decrypt(string text, string key) => new string(text.Select((c, i) => (char)(c ^ key[i % key.Length])).ToArray());

            
            

            var apiKey = Decrypt(
                "\u0017\u001eC\u0017\u001c\u0005\u000eX_\u0004\b\u0006=<\u0002!\u0018Z\a\r'6\u001a\t \0=\u001e\u0019+\u0012\u0017\u0005\bV;\u0012??\v,>U\u0004(\u0012C\u001eP#\u000f&*\u001f\n\0\u0005\v\a5\a\u0014\u0004,^>3B</[\"\u00141\u0006\u0003\u001eR\f\u001f(\r:Y&\u0019\f\f( \u0013\u001a(1X\r7\u0016+\u001f\a9/2&\u0016\u0019\u0003%\u001f\u000f%\u0005\bW\u001f!8\n;\n\u001bX\n\0\b4\r\u0006?\r[.;\f+(\u0019\u000f\u0006!\u0013\u0004%/\u001b\u001a5CG\u0011\u0001\u00190?\u0006Q\u001a\v\0\u00189T4",
                 "dungnj"
                );
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "user", content = request.Message }
            }
            };

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseString);
            var reply = jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return Ok(new { reply });
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}
