namespace RestaurantSystem.Application.DTOs.Common
{
    /// <summary>
    /// DTO للاستجابة الموحدة من جميع الـ APIs
    /// </summary>
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = default!;

        public T Data { get; set; } = default!;

        public List<string> Errors { get; set; } = new();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ✅ Factory Methods
        public static ApiResponseDto<T> Success(T data, string message = "العملية نجحت")
            => new() { IsSuccess = true, Data = data, Message = message };

        public static ApiResponseDto<T> Failure(string message, List<string>? errors = null)
            => new() { IsSuccess = false, Message = message, Errors = errors ?? new() };
    }

    /// <summary>
    /// DTO لاستجابة الـ Pagination
    /// </summary>
    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
