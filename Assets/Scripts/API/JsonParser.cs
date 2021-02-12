using System;
using UnityEngine;
public static class JsonParser
{


    [Serializable]
    private class PlayerLibraryResponse
    {
        public PlayerLibrary response;
    }

    [Serializable]
    private class PlayerIDResponse
    {
        public SteamID response;

        [Serializable]
        public class SteamID
        {
            public long steamid;
            public bool success;
        }


    }



    public static bool TryGetPlayerLibraryJson(string Json, out PlayerLibrary playerLibrary) {
        playerLibrary = null;
        if (Json.Length < 1 || Json == "")
            return false;

        PlayerLibraryResponse response = new PlayerLibraryResponse();
        try {
            JsonUtility.FromJsonOverwrite(Json, response);
        }
        catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
        if (response.response.games == null ) {
            Debug.Log("Error");
            return false;
        }

        playerLibrary = response.response;
        Debug.Log(playerLibrary.game_count.ToString() + " ID: " + playerLibrary.games[0].appid + " Time: " + playerLibrary.games[0].playtime_forever);
        return true;
    }

    public static bool TryGetPlayerID(string Json, out long playerID) {
        playerID = 0;
        if (Json.Length < 1 || Json == "")
            return false;

        PlayerIDResponse response = new PlayerIDResponse();
        try {
            JsonUtility.FromJsonOverwrite(Json, response);
        }
        catch (Exception e) {
            Debug.LogError(e);
            return false;
        }




        playerID = response.response.steamid;
        Debug.Log(response.response.steamid);
        //   return playerId.response.steamid;

        return response.response.success;
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

        bool has_community_visible_stats;
        int playtime_windows_forever;
        int playtime_mac_forever;
        int playtime_linux_forever;
    }
}
