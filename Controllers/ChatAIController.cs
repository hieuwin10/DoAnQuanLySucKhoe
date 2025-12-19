using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnChamSocSucKhoe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatAIController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _ollamaBaseUrl;
        private readonly string _ollamaModel;
        private readonly string _openRouterApiKey;
        private readonly string _openRouterModel;

        public ChatAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _ollamaBaseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
            _ollamaModel = configuration["Ollama:Model"] ?? "llama3.2";
            _openRouterApiKey = configuration["OpenRouter:ApiKey"] ?? "";
            _openRouterModel = configuration["OpenRouter:Model"] ?? "openai/gpt-oss-20b:free";
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { reply = "Tin nhắn không được để trống." });
            }

            // Ưu tiên sử dụng OpenRouter nếu có API key
            if (!string.IsNullOrEmpty(_openRouterApiKey))
            {
                return await AskOpenRouter(request.Message);
            }
            else
            {
                return await AskOllama(request.Message);
            }
        }

        private async Task<IActionResult> AskOpenRouter(string message)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = "https://openrouter.ai/api/v1/chat/completions";

            var payload = new
            {
                model = _openRouterModel,
                messages = new[]
                {
                    new { role = "system", content = "Bạn là một trợ lý AI hữu ích chuyên về tư vấn sức khỏe và thể chất nói chung. Không đưa ra các chẩn đoán hoặc đơn thuốc y tế cụ thể. Trả lời bằng tiếng Việt.\n\nQUAN TRỌNG - HIỂN THỊ CÔNG THỨC TOÁN HỌC:\n- LUÔN LUÔN sử dụng ký hiệu $ cho công thức inline: $công_thức$\n- LUÔN LUÔN sử dụng ký hiệu $$ cho công thức block: $$công_thức$$\n- KHÔNG BAO GIỜ sử dụng [ ] hoặc \\[ \\] cho công thức\n- Ví dụ đúng: $\\text{BMI} = \\frac{\\text{cân nặng}}{\\text{chiều cao}^2}$\n- Ví dụ đúng block: $$\\text{BMI} = \\frac{45}{1.6^2} \\approx 17.6$$\n- Sử dụng \\text{} cho văn bản trong công thức\n- Sử dụng \\frac{tử}{mẫu} cho phân số" },
                    new { role = "user", content = message }
                }
            };

            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Thêm header Authorization
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openRouterApiKey}");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseBody);

                    // Cấu trúc phản hồi của OpenRouter: { "choices": [{ "message": { "content": "..." } }] }
                    var text = jsonDoc.RootElement
                                      .GetProperty("choices")[0]
                                      .GetProperty("message")
                                      .GetProperty("content")
                                      .GetString();

                    return Ok(new { reply = text });
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OpenRouter API Error: {errorBody}");
                    return StatusCode((int)response.StatusCode, new { reply = $"Lỗi từ OpenRouter: {response.ReasonPhrase}. Vui lòng kiểm tra API key." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { reply = $"Đã có lỗi xảy ra khi giao tiếp với OpenRouter: {ex.Message}." });
            }
        }

        private async Task<IActionResult> AskOllama(string message)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = $"{_ollamaBaseUrl}/api/chat";

            var payload = new
            {
                model = _ollamaModel,
                messages = new[]
                {
                    new { role = "system", content = "Bạn là một trợ lý AI hữu ích chuyên về tư vấn sức khỏe và thể chất nói chung. Không đưa ra các chẩn đoán hoặc đơn thuốc y tế cụ thể. Trả lời bằng tiếng Việt.\n\nQUAN TRỌNG - HIỂN THỊ CÔNG THỨC TOÁN HỌC:\n- LUÔN LUÔN sử dụng ký hiệu $ cho công thức inline: $công_thức$\n- LUÔN LUÔN sử dụng ký hiệu $$ cho công thức block: $$công_thức$$\n- KHÔNG BAO GIỜ sử dụng [ ] hoặc \\[ \\] cho công thức\n- Ví dụ đúng: $\\text{BMI} = \\frac{\\text{cân nặng}}{\\text{chiều cao}^2}$\n- Ví dụ đúng block: $$\\text{BMI} = \\frac{45}{1.6^2} \\approx 17.6$$\n- Sử dụng \\text{} cho văn bản trong công thức\n- Sử dụng \\frac{tử}{mẫu} cho phân số" },
                    new { role = "user", content = message }
                },
                stream = false
            };

            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseBody);
                    
                    // Cấu trúc phản hồi của Ollama: { "model": "...", "created_at": "...", "message": { "role": "assistant", "content": "..." }, ... }
                    var text = jsonDoc.RootElement
                                      .GetProperty("message")
                                      .GetProperty("content")
                                      .GetString();

                    return Ok(new { reply = text });
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ollama API Error: {errorBody}");
                    return StatusCode((int)response.StatusCode, new { reply = $"Lỗi từ Ollama: {response.ReasonPhrase}. Hãy đảm bảo bạn đã cài đặt và đang chạy Ollama (ollama run {_ollamaModel})." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { reply = $"Đã có lỗi xảy ra khi giao tiếp với Ollama: {ex.Message}. Hãy đảm bảo Ollama đang chạy." });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
