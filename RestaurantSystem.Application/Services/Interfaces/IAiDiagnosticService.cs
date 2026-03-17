// ✅ المسار الصحيح:
// RestaurantSystem.Application/Services/Interfaces/IAiDiagnosticService.cs

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IAiDiagnosticService
    {
        Task<string> DiagnoseErrorAsync(string errorMessage, string? stackTrace); // ✅ nullable
        Task<string> GetSuggestionsAsync(string errorMessage);
        Task<string> AnalyzePerformanceAsync(string code);
        Task<string> ReviewCodeQualityAsync(string code);
    }
}
