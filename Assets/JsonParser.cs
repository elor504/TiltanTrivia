using System;
using UnityEngine;
public class JsonParser
{
    public static JsonParser GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JsonParser();
            }
            return _instance;
        }
    }
    private static JsonParser _instance;


    [Serializable]
    private class PlayerLibraryResponse
    {
        public PlayerLibrary response;


        [System.Serializable]
        public class PlayerLibrary
        {
            public int game_count;
            public Game[] games;

            [System.Serializable]
            public class Game
            {
                public int appid;
                public int playtime_forever;
                int playtime_windows_forever;
                int playtime_mac_forever;
                int playtime_linux_forever;
            }
        }


    }
    private JsonParser()
    {

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



    public bool TryGetPlayerLibraryJson(string Json)
    {
        if (Json.Length < 1 || Json == "")
            return false;
        PlayerLibraryResponse playerLibrary = new PlayerLibraryResponse();
        JsonUtility.FromJsonOverwrite(Json, playerLibrary);
        //if (playerLibrary.response.game_count == 0 && playerLibrary.response.games.Length ==0)
        //{

        //}
        Debug.Log(playerLibrary.response.game_count.ToString() + " ID: " + playerLibrary.response.games[0].appid + " Time: " + playerLibrary.response.games[0].playtime_forever);
        return true;
    }

    public bool TryGetPlayerID(string Json, out long playerID)
    {
        playerID = 0;
        if (Json.Length <1|| Json == "") 
            return false;
        
        PlayerIDResponse response = new PlayerIDResponse();
        JsonUtility.FromJsonOverwrite(Json, response);
     
        playerID = response.response.steamid;
        Debug.Log(response.response.steamid);
        //   return playerId.response.steamid;

        return response.response.success;
    }

}

