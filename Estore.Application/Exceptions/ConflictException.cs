namespace Estore.Application.Exceptions
{
    public class ConflictException : BaseException
    {
        public override int StatusCode => 409;

        public ConflictException(string message) : base(message) { }
    }
}
