public class ServerResponse
{
    public const int STATUS_CODE_SUCCESS = 10000;
    public const int STATUS_CODE_REQUEST_ERROR = 10001;
    
    public int statusCode;
    public string errorMsg;
    public string responseMsg;

    public ServerResponse()
    {
        statusCode = 0;
        errorMsg = string.Empty;
        responseMsg = string.Empty;
    }

    public void SetError(string errorMsg)
    {
        statusCode = STATUS_CODE_REQUEST_ERROR;
        this.errorMsg = errorMsg;
    }
}