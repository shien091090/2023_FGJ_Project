using System;

public class RequestSender
{
    public string Action { get; }
    public RequestType RequestType { get; }
    public (string, string)[] Parameters { get; private set; }

    public RequestSender(string action, RequestType requestType)
    {
        Action = action;
        RequestType = requestType;
    }

    public void AddParameter(string fieldName, string fieldValue)
    {
        if (Parameters == null)
            Parameters = Array.Empty<(string, string)>();

        (string, string)[] newParameters = new (string, string)[Parameters.Length + 1];
        for (int i = 0; i < Parameters.Length; i++)
        {
            newParameters[i] = Parameters[i];
        }

        newParameters[Parameters.Length] = (fieldName, fieldValue);
        Parameters = newParameters;
    }
}