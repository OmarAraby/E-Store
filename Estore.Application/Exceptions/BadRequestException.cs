namespace Estore.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public override int StatusCode => 400;

        public BadRequestException(string message) : base(message) { }
    }
}
