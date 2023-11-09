public class RequestSender
{
    public string Action { get; }
    public RequestType RequestType { get; }
    public (string, string)[] Parameters { get; }

    public RequestSender(string action, RequestType requestType)
    {
        Action = action;
        RequestType = requestType;
    }
}