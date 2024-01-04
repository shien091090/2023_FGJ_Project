using System;

public interface IServerCommunicator
{
    event Action OnRequestCompleted;
    bool IsWaitingResponse { get; }
    ServerCommunicator CreatePostRequest(string action);
}