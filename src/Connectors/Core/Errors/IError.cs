namespace Core
{
    public interface IError
    {
        string Message { get; }
        object Cause { get; }
    }
}