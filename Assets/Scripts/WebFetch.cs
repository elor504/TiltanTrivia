using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class WebFetch
{
    const int timeout = 1;
    public static string SignupURI<T>(T username) => "https://localhost:44306/api/Player?playerName=" + username.ToString();
    public static string FindMatchURI<T>(T playerID) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID.ToString();
    public static string GetRoomIdURI<T>(T playerID) => "https://localhost:44306/api/GetGameRoomID?playerId=" + playerID.ToString();
    public static string GetRoomIdURI<T1,T2>(T1 roomID,T2 playerID) => "https://localhost:44306/api/GameRooms?GameRoomId="+ roomID + "&PlayerId=" + playerID.ToString();
    public static string GetIsPlayer2LoggedInURI<T>(T roomID) => "https://localhost:44306/api/CheckPlayerConnection?gameRoomId=" + roomID.ToString();
    public static string CreateRooomURI<T1,T2>(T1 playerID, T2 password) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID.ToString() + "&PW=" + password.ToString();
    public static string JoinRooomURI<T1,T2,T3>(T1 playerID, T2 roomID, T3 password) => "https://localhost:44306/api/GameRoomLobby?playerId=" + playerID.ToString() + "&GameRoomId=" + roomID.ToString() + "&PW=" + password.ToString();
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
