using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AppManager : MonoBehaviour
{

    string api = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=5D782C1F7C23790946875A09EACF822B&steamid=76561197960434622&format=json";
    public static AppManager _instance;

    string url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=";
        
    string steamID = "&steamid=";
    string endOfURL = "&format=json";
    public void AssignURL(string keyString, string steamID)
    {
        string urlString = url + keyString +this.steamID +steamID + endOfURL;
        StartCoroutine(ConnectAPI(urlString));
    }
    void Start()
    {
        _instance = this;

        StartCoroutine(ConnectAPI(api));

    }
    IEnumerator ConnectAPI(string api) {

        UnityWebRequest webReq = new UnityWebRequest();
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // build the url and query
        webReq.url = api;

        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError)
        {
            Debug.Log("Error");
        }
        else
        {

            string jsonDAta = Encoding.UTF8.GetString(webReq.downloadHandler.data);
            Debug.Log(jsonDAta);
            // send json from here
            


        }
    }

}
