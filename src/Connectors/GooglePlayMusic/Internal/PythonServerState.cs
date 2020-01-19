namespace GooglePlayMusic.Internal
{
    public enum PythonServerState
    {
        Created,
        Starting,
        Running,
        Closing,
        Closed,
        Authenticating,
        AwaitingAuthenticationToken,
        Authenticated,
        Error = -1,
        AuthenticationError = -2
    }
}