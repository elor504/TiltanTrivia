using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonParser 
{
    private class PlayerLibrary
    {
        int game_count;
        private class Game
        {
            int appid;
            int playtime_forever;
            int playtime_windows_forever;
            int playtime_mac_forever;
            int playtime_linux_forever;
        }
    }

    public void GetPlayerLibraryJson(string Json)
    {
        //object JsonTest = JsonUtility.FromJson(Json, Json.GetType());
    }



}

