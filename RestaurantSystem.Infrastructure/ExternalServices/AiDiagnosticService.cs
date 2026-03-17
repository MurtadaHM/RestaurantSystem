// ✅ المسار الصحيح:
// RestaurantSystem.Infrastructure/ExternalServices/AiDiagnosticService.cs

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RestaurantSystem.Infrastructure.ExternalServices  // ✅ تأكد من هذا
{
    public class AiDiagnosticService : IAiDiagnosticService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiDiagnosticService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        private const string ApiUrl = "https://api.anthropic.com/v1/messages";

        public AiDiagnosticService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AiDiagnosticService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["Claude:ApiKey"] ?? string.Empty;
            _model = configuration["Claude:Model"] ?? "claude-3-haiku-20240307";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> DiagnoseErrorAsync(string errorMessage, string? stackTrace)
        {
            var prompt = $"""
                You are an expert .NET backend developer.
                Analyze this error and provide a concise fix in 2-3 lines.
                Error: {errorMessage}
                StackTrace: {stackTrace ?? "N/A"}
                Respond ONLY with the fix suggestion.
                """;
            return await CallClaudeAsync(prompt);
        }

        public async Task<string> GetSuggestionsAsync(string errorMessage)
        {
            var prompt = $"""
                You are an expert .NET developer.
                Provide 3 bullet-point suggestions to fix:
                Issue: {errorMessage}
                Respond with only 3 bullet points.
                """;
            return await CallClaudeAsync(prompt);
        }

        public async Task<string> AnalyzePerformanceAsync(string code)
        {
            var prompt = $"""
                You are a .NET performance expert.
                Analyze this code for performance issues:
                ```csharp
                {code}
                ```
                Provide 3-5 bullet points.
                """;
            return await CallClaudeAsync(prompt);
        }

        public async Task<string> ReviewCodeQualityAsync(string code)
        {
            var prompt = $"""
                You are a senior .NET code reviewer.
                Review this code for quality and best practices:
                ```csharp
                {code}
                ```
                Provide 3-5 bullet points.
                """;
            return await CallClaudeAsync(prompt);
        }

        private async Task<string> CallClaudeAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("Claude API Key not configured.");
                return "AI diagnostics not configured.";
            }

            try
            {
                var requestBody = new
                {
                    model = _model,
                    max_tokens = 512,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Claude API error: {StatusCode}", response.StatusCode);
                    return $"AI unavailable (HTTP {(int)response.StatusCode}).";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                return doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString() ?? "No response from AI.";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error calling Claude API.");
                return "AI service temporarily unavailable.";
            }
        }
    }
}
