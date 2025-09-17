namespace Estore.Application.Common.GeneralResult
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResponse(string message = "Request successful")
            => new() { Success = true, Message = message };
    }
}

