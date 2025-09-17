namespace Estore.Application.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result Failure(List<string> errors, string message = "Request failed")
            => new Result { Success = false, Errors = errors, Message = message };
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> SuccessResult(T data, string message = "Request successful")
            => new Result<T> { Success = true, Data = data, Message = message };

        public new static Result<T> Failure(List<string> errors, string message = "Request failed")
            => new Result<T> { Success = false, Errors = errors, Message = message };
    }
}
