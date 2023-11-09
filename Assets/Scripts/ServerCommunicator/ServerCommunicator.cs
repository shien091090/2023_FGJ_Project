using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerCommunicator : MonoBehaviour
{
    private const string URL = "https://script.google.com/macros/s/AKfycbxhQd9l5YvrmyHRPlRQlnat9-YjDhcA-f2AN-QfOd-au7aIgHnGzD_CLfq0cOWS8QSFXw/exec";
    private RequestSender requestSender;

    public ServerCommunicator CreatePostRequest(string action)
    {
        requestSender = new RequestSender(action, RequestType.Post);
        return this;
    }

    [ContextMenu("Test Send Post Request")]
    public void TestSendPostRequest()
    {
        CreatePostRequest("api_get_record")
            .SendRequest();
    }

    private void SendRequest()
    {
        if (requestSender == null)
        {
            Debug.Log("[ServerCommunicator] requestSender is null");
            return;
        }

        StartCoroutine(Cor_SendRequest(requestSender.Action, requestSender.RequestType, requestSender.Parameters));
    }

    private IEnumerator Cor_SendRequest(string action, RequestType requestType, params (string, string)[] parameters)
    {
        Debug.Log($"[ServerCommunicator] SendRequest, action: {action}, requestType: {requestType}");
        
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

        if (requestType == RequestType.Post)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.Log("Error: " + www.error);
                else
                    Debug.Log("Response: " + www.downloadHandler.text);
            }
        }
    }
}