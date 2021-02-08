using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AppManager : MonoBehaviour
{
    UIManager uiManager;

    public static AppManager _instance;
    #region UrlLinks
    const string api = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=5D782C1F7C23790946875A09EACF822B&steamid=76561197960434622&format=json&include_appinfo=true";
    const string url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=";
    const string steamId = "&steamid=";
    const string endOfURL = "&format=json&include_appinfo=true";
    const string ISteamUserAPI = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=";
    const string vanityURL = "&vanityurl=";

    /// <summary>
    /// https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/APPID/LOGO_ID.jpg
    /// </summary>
    const string logoUrl = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/";
    const string logoEndOfUrl = ".jpg";


    [SerializeField]
    private string userName;
    [SerializeField]
    private string key;

    [SerializeField]
    private int numOfGames;

    Sprite logoSprite;

    #endregion



    string jsonData;


    public string GetSteamId_Url(string key, string username) => ISteamUserAPI + key + vanityURL + username;
 
    public string GetPlayerGameData_Url(string keyString, string steamID)  => url + keyString + steamId + steamID + endOfURL;



    public string GetGameLogo_Url(string AppId, string LogoUrl) => logoUrl + AppId + "/" + LogoUrl + logoEndOfUrl;

    void Start()
    {
        _instance = this;
        uiManager = GetComponent<UIManager>();
        //StartCoroutine(ConnectAPI(api));

        StartCoroutine(GetPlayerLibrary(key, userName));

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


    IEnumerator GetPlayerLibrary(string key, string userName)
    {



        yield return StartCoroutine(ConnectToAPI(GetSteamId_Url(key, userName)));


        if (JsonParser.GetInstance.TryGetPlayerID(jsonData, out long playerID))
        {



            yield return StartCoroutine(ConnectToAPI(GetPlayerGameData_Url(key, playerID.ToString())));


            if (JsonParser.GetInstance.TryGetPlayerLibraryJson(jsonData, out PlayerLibrary playerLibrary))
            {

                Array.Sort(playerLibrary.games, (G1, G2) => -G1.playtime_forever.CompareTo(G2.playtime_forever));
                Debug.Log(playerLibrary.games[0].appid);

                StartCoroutine(GetLogos(playerLibrary));
              
            }
            else
            {
                Debug.Log("Error");
            }

        }
    }

    IEnumerator GetLogos(PlayerLibrary playerLibrary)
    {
        Sprite[] games = new Sprite[numOfGames];
        for (int i = 0; i < numOfGames; i++)
        {

            string appId = playerLibrary.games[i].appid.ToString();
            string logoUrl = playerLibrary.games[i].img_logo_url;



            yield return StartCoroutine(GetTexture(GetGameLogo_Url(appId, logoUrl)));
            games[i] = logoSprite;
        }

        uiManager.GetLogoArray(games);
    }

    IEnumerator GetTexture(string uri)
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
                    logoSprite = Sprite.Create((Texture2D)myTexture, new Rect(Vector2.zero, new Vector2(myTexture.width, myTexture.height)), Vector2.zero);
                   
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }



}

