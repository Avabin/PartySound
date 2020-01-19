namespace Core.Errors
{
    public class GenericError : IError
    {
        public GenericError(string message, object cause)
        {
            Message = message;
            Cause = cause;
        }

        public string Message { get; }
        public object Cause { get; }
    }
}