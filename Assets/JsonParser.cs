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
            public int steamid;
            public bool success;
        }


    }



    public void GetPlayerLibraryJson(string Json)
    {
        PlayerLibraryResponse playerLibrary = new PlayerLibraryResponse();
        JsonUtility.FromJsonOverwrite(Json, playerLibrary);

        Debug.Log(playerLibrary.response.game_count.ToString() + " ID: " + playerLibrary.response.games[0].appid + " Time: " + playerLibrary.response.games[0].playtime_forever);
    }

    public int GetPlayerID(string Json)
    {
        PlayerIDResponse playerId = new PlayerIDResponse();
        JsonUtility.FromJsonOverwrite(Json, playerId);

        Debug.Log(playerId.response.steamid);
        return playerId.response.steamid;
    }

}

