namespace Estore.Application.Exceptions
{
    public abstract class BaseException : Exception
    {
        public abstract int StatusCode { get; }

        protected BaseException(string message) : base(message) { }
        protected BaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
