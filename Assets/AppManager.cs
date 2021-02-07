using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AppManager : MonoBehaviour
{


    public static AppManager _instance;
    #region UrlLinks
    const string api = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=5D782C1F7C23790946875A09EACF822B&steamid=76561197960434622&format=json";
    const string url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=";
    const string steamId = "&steamid=";
    const string endOfURL = "&format=json";
    const string ISteamUserAPI = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=";
    const string vanityURL = "&vanityurl=";


    #endregion



    string jsonData;


    public string GetSteamID(string key, string username) 
    {
        string assignIDURL = ISteamUserAPI + key + vanityURL + username ;
        return assignIDURL;
    }

    public string GetPlayerGameData(string keyString, string steamID)
    {
        string urlString = url + keyString + steamId + steamID + endOfURL;
        return urlString;
    }
    void Start()
    {
        _instance = this;

        //StartCoroutine(ConnectAPI(api));
       // GetSteamID("F799A742D77E4E760DDABF6DA251243D", "elor504");
    }



    bool isCompleted;
    IEnumerator ConnectToAPI(string api)
    {

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

            jsonData = Encoding.UTF8.GetString(webReq.downloadHandler.data);
            
        }
    }


    IEnumerator GetPlayerLibrary(string key , string userName) {



        yield return StartCoroutine(ConnectToAPI(GetSteamID(key, userName)));


        if (JsonParser.GetInstance.TryGetPlayerID(jsonData, out long playerID))
        { 
       
        

        yield return StartCoroutine(ConnectToAPI(GetPlayerGameData(key, playerID.ToString())));


            if (!JsonParser.GetInstance.TryGetPlayerLibraryJson(jsonData))
            {
                Debug.Log("Error");

            }

        }
    }
}
