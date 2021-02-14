using System;
using UnityEngine;
public static class JsonParser
{
    public static bool TryParseJson<T>(string json, out T jsonObject) where T : class {
        jsonObject = null;
        if (json.Length < 1 || json == "")
            return false;
        try {
            jsonObject = JsonUtility.FromJson<T>(json);
        }
        catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
        if (jsonObject == null) {
            Debug.Log("Error");
            return false;
        }
        return true;
    }
    [Serializable] private class Response<T> where T : class { public T response; }
    public static bool TryParseSteamJson<T>(string json, out T jsonObject) where T : class {
        jsonObject = null;
        if (!TryParseJson(json, out Response<T> response))
            return false;
        jsonObject = response.response;
        return true;
    }
}
[System.Serializable]
public class PlayerLibrary
{
    public int game_count;
    public Game[] games;

    [System.Serializable]
    public class Game
    {
        public int appid;
        public string name;
        public int playtime_forever;
        public string img_icon_url;
        public string img_logo_url;
        //bool has_community_visible_stats;
        //int playtime_windows_forever;
        //int playtime_mac_forever;
        //int playtime_linux_forever;
    }
}
[Serializable]
public class SteamID
{
    public long steamid;
    public bool success;
}
