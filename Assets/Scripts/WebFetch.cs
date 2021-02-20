using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class WebFetch
{
    const int timeout = 1;
    public static string SignupURI(object username) => "https://localhost:44306/api/Player?playerName=" + username;
    public static string FindMatchURI(object playerID) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID;
    public static string GetRoomURI(object roomID) => "https://localhost:44306/api/GameRooms/" + roomID;
    public static string GetRoomIdURI(object playerID) => "https://localhost:44306/api/GetGameRoomID?playerId=" + playerID;
    public static string GetRoomIdURI(object roomID, object playerID) => "https://localhost:44306/api/GameRooms?GameRoomId="+ roomID + "&PlayerId=" + playerID;
    public static string GetQuestionURI(object roomID, object playerID) => "https://localhost:44306/api/Question?gameRoomId=" + roomID + "&playerId=" + playerID;
    public static string GetPlayerFinishedURI(object roomID, object playerID) => "https://localhost:44306/api/CheckPlayerConnection?gameRoomId=" + roomID + "&PlayerId=" + playerID;
    public static string GetInsertAnswerURI(object roomID, object playerID, object answerNum) => "https://localhost:44306/api/Question?gameRoomId=" + roomID + "&playerId=" + playerID + "&AnswerNumber=" + answerNum;
    public static string GetOpponentUsernameURI(object roomID, object playerID) => "https://localhost:44306/api/GameRoomLobby?GameRoomId=" + roomID + "&playerId=" + playerID;
    public static string GetIsPlayer2LoggedInURI(object roomID) => "https://localhost:44306/api/CheckPlayerConnection?gameRoomId=" + roomID;
    public static string CreateRooomURI(object playerID, object password) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID + "&PW=" + password;
    public static string JoinRooomURI(object playerID, object roomID, object password) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID + "&GameRoomId=" + roomID + "&PW=" + password;
    public static IEnumerator ConnectToAPI(string api, Action<string> callback)
    {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // build the url and query
        webReq.url = api;

        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError)
        {
            Debug.Log(webReq.error);
        }
        else
        {

            callback?.Invoke(Encoding.UTF8.GetString(webReq.downloadHandler.data));

        }
    }
    public static IEnumerator GetTexture(string uri, Action<Sprite> callback)
    {

        bool isValid = false;
        try
        {
            isValid = new Uri(uri).IsWellFormedOriginalString();
            if (!isValid && Path.IsPathRooted(uri))
            {
                uri = Path.GetFullPath(uri);
                isValid = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        if (isValid)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
            yield return request.SendWebRequest();
            try
            {
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Error getting: " + uri);
                    Debug.Log(request.error);
                }
                else
                {
                    Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    callback?.Invoke(Sprite.Create((Texture2D)myTexture, new Rect(Vector2.zero, new Vector2(myTexture.width, myTexture.height)), Vector2.zero));

                }

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="callback">
    /// Call the callback action on finish with the values: (success, json, error)
    /// </param>
    /// <returns></returns>
    public static IEnumerator HttpGet(string uri, Action<HttpResponse> callback = null)
    {
        using (UnityWebRequest webReq = UnityWebRequest.Get(uri))
        {
            webReq.timeout = timeout;
            yield return webReq.SendWebRequest();
            if (webReq.isNetworkError || webReq.isHttpError)
            {
                Debug.Log("Error");
                callback?.Invoke(new HttpResponse(false, "", webReq.error));
            }
            else
            {
                string data = Encoding.UTF8.GetString(webReq.downloadHandler.data);
                Debug.Log(data);
                Debug.Log("Recieved!");
                callback?.Invoke(new HttpResponse(true, data, ""));
            }
        }
    }
    private const string defaultContentType = "application/json";
    public static IEnumerator HttpPost(string uri, string jsonBody, Action<string> callback = null)
    {

        using (UnityWebRequest webReq = UnityWebRequest.Post(uri, jsonBody))
        {
            webReq.SetRequestHeader("Content-Type", defaultContentType);
            webReq.uploadHandler.contentType = defaultContentType;
            webReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));

            yield return webReq.SendWebRequest();

            if (webReq.isNetworkError)
            {
                callback?.Invoke("Error");
            }
            else
            {
                //string data = System.Text.Encoding.UTF8.GetString(webReq.downloadHandler.data);
                //Debug.Log(data);

                callback?.Invoke("Send");
            }

        }


    }
}
public struct HttpResponse
{
    public bool success;
    public string json;
    public string errorMessage;

    public HttpResponse(bool success, string json, string error)
    {
        this.success = success;
        this.json = json;
        this.errorMessage = error;
    }
} 
