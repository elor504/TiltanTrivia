using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class WebFetch
{
    public static IEnumerator ConnectToAPI(string api, Action<string> callback) {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // build the url and query
        webReq.url = api;

        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError) {
            Debug.Log(webReq.error);
        }
        else {

            callback?.Invoke(Encoding.UTF8.GetString(webReq.downloadHandler.data));

        }
    }
    public static IEnumerator GetTexture(string uri, Action<Sprite> callback) {

        bool isValid = false;
        try {
            isValid = new Uri(uri).IsWellFormedOriginalString();
            if (!isValid && Path.IsPathRooted(uri)) {
                uri = Path.GetFullPath(uri);
                isValid = true;
            }
        }
        catch (Exception e) {
            Debug.Log(e.Message);
        }
        if (isValid) {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
            yield return request.SendWebRequest();
            try {
                if (request.isNetworkError || request.isHttpError) {
                    Debug.Log("Error getting: " + uri);
                    Debug.Log(request.error);
                }
                else {
                    Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    callback?.Invoke(Sprite.Create((Texture2D)myTexture, new Rect(Vector2.zero, new Vector2(myTexture.width, myTexture.height)), Vector2.zero));

                }

            }
            catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
    }

}
