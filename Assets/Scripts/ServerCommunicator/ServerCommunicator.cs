using System;
using System.Collections;
using SCGLobby;
using UnityEngine;
using UnityEngine.Networking;

public class ServerCommunicator : MonoBehaviour
{
    private const string URL = "https://script.google.com/macros/s/AKfycbzvsgJfeXsqIyXbsKm9HX8ShA6SJysfbAx7Coinbj2YR7efbCD9zTY6UiPC1UZYaWlDMA/exec";
    private static ServerCommunicator _instance;

    private RequestSender requestSender;
    public static ServerCommunicator Instance => _instance;

    public void SendRequest<T>(Action<T> callback = null) where T : ServerResponse
    {
        if (requestSender == null)
        {
            Debug.Log("[ServerCommunicator] requestSender is null");
            return;
        }

        StartCoroutine(Cor_SendRequest(requestSender.Action, requestSender.RequestType, callback, requestSender.Parameters));
    }

    public ServerCommunicator CreatePostRequest(string action)
    {
        requestSender = new RequestSender(action, RequestType.Post);
        return this;
    }

    [ContextMenu("Test Add Record")]
    public void TestSendRequest_AddRecord()
    {
        CreatePostRequest("api_add_record")
            .AddParameter("playerName", "NKShien123")
            .AddParameter("costTimeSeonds", 15000)
            .SendRequest<PlayerRecordResponse>((res) =>
            {
            });
    }

    [ContextMenu("Test Get Record")]
    public void TestSendRequest_GetRecord()
    {
        CreatePostRequest("api_get_record")
            .SendRequest<PlayerRecordResponse>((res) =>
            {
            });
    }

    private ServerCommunicator AddParameter(string filedName, object fieldValue)
    {
        if (requestSender == null)
            return this;

        requestSender.AddParameter(filedName, fieldValue.ToString());
        return this;
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private WWWForm CreateWWWForm(string action, (string, string)[] parameters)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", action);
        if (parameters != null)
        {
            foreach ((string fieldName, string fieldValue) parameter in parameters)
            {
                if (int.TryParse(parameter.fieldValue, out int parseIntValue))
                    form.AddField(parameter.fieldName, parseIntValue);
                else
                    form.AddField(parameter.fieldName, parameter.fieldValue);
            }
        }

        return form;
    }

    private T CreateErrorResponse<T>(UnityWebRequest requestInfo) where T : ServerResponse
    {
        T errorResponse = Activator.CreateInstance<T>();
        errorResponse.SetError(requestInfo.error);
        return errorResponse;
    }

    private IEnumerator Cor_SendRequest<T>(string action, RequestType requestType, Action<T> callback, params (string, string)[] parameters) where T : ServerResponse
    {
        Debug.Log($"[ServerCommunicator] SendRequest, action: {action}, requestType: {requestType}");

        WWWForm form = CreateWWWForm(action, parameters);

        if (requestType == RequestType.Post)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success || string.IsNullOrEmpty(www.downloadHandler.text))
                    callback?.Invoke(CreateErrorResponse<T>(www));
                else
                {
                    Debug.Log($"[ServerCommunicator] Response, action: {action}, content: {www.downloadHandler.text}");
                    ActivityJsonParser jsonParser = new ActivityJsonParser();
                    callback?.Invoke(jsonParser.TryDeserializeObject(www.downloadHandler.text, out T response) ?
                        response :
                        CreateErrorResponse<T>(www));
                }
            }
        }
    }
}