using System;
using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    ApiUIManager uiManager;

    public static AppManager _instance;
    #region UrlLinks
    const string steamID_baseURL = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=";
    const string steamLibrary_baseURL = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=";
    const string steamGameLogo_baseURL = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/";
    public string GetSteamId_Url(string key, string username) => steamID_baseURL + key + "&vanityurl=" + username;
    public string GetPlayerGameData_Url(string keyString, string steamID) => steamLibrary_baseURL + keyString + "&steamid=" + steamID + "&format=json&include_appinfo=true";
    public string GetGameImage_Url(string AppId, string ImageID) => steamGameLogo_baseURL + AppId + "/" + ImageID + ".jpg";
    #endregion
    [SerializeField]
    private string key;
    [SerializeField]
    private int numOfGames;


    public event Action<bool> LoadingData;
    string loadedUserName;
    PlayerLibrary loadedPlayerLibrary;
    bool loadLogos = true;
    Sprite[] logos;
    Sprite[] icons;
    string jsonData;
    Sprite gameSprite;
    void SetJsonData(string json) => jsonData = json;
    void SetSprite(Sprite sprite) => gameSprite = sprite;

    void Awake() {
        _instance = this;
        uiManager = GetComponent<ApiUIManager>();
    }
    public void LoadData(string userName) {
        if (loadedUserName != userName) {
            logos = null;
            icons = null;
            loadedUserName = userName;
            StartCoroutine(GetPlayerLibrary(key, userName, () => { StartCoroutine(LoadAppsImages()); }));
        }
    }
    public void SetImageState(bool loadLogos) {
        this.loadLogos = loadLogos;
        if ((loadLogos ? logos : icons) != null && (loadLogos ? logos : icons).Length > 0)
            uiManager.SetImageArray((loadLogos ? logos : icons), loadLogos);
        else
            StartCoroutine(LoadAppsImages());
    }
    IEnumerator LoadAppsImages() {
        LoadingData(true);
        Array.Sort(loadedPlayerLibrary.games, (G1, G2) => -G1.playtime_forever.CompareTo(G2.playtime_forever));
        Debug.Log(loadedPlayerLibrary.games[0].appid);

        Sprite[] images = new Sprite[numOfGames];
        for (int i = 0; i < numOfGames; i++) {

            string appId = loadedPlayerLibrary.games[i].appid.ToString();
            string imageID = (loadLogos ? loadedPlayerLibrary.games[i].img_logo_url : loadedPlayerLibrary.games[i].img_icon_url);



            yield return StartCoroutine(WebFetch.GetTexture(GetGameImage_Url(appId, imageID), SetSprite));
            images[i] = gameSprite;
        }
        if (loadLogos)
            logos = images;
        else
            icons = images;
        uiManager.SetImageArray(images, loadLogos);
        LoadingData(false);
    }
    IEnumerator GetPlayerLibrary(string key, string userName, Action callback = null) {
        LoadingData(true);

        yield return StartCoroutine(WebFetch.ConnectToAPI(GetSteamId_Url(key, userName), SetJsonData));


        if (JsonParser.TryGetPlayerID(jsonData, out long playerID)) {



            yield return StartCoroutine(WebFetch.ConnectToAPI(GetPlayerGameData_Url(key, playerID.ToString()), SetJsonData));


            if (JsonParser.TryGetPlayerLibraryJson(jsonData, out PlayerLibrary playerLibrary)) {
                loadedPlayerLibrary = playerLibrary;
                callback?.Invoke();
            }
            else {
                LoadingData(false);
            }
        }
        else {
            LoadingData(false);
        }
    }
}

