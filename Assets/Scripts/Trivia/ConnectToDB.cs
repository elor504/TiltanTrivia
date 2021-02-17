using System;
using UnityEngine;


public class ConnectToDB : MonoBehaviour
{
    //string db_IP = "80.230.92.74:3306";
    //string db_PW = "elor2020";
    // int id = 1;
    //string db_server = "trivia";


    #region API COMMANDS


    string uriCreatePlayerByString = "https://localhost:44306/api/Player?playerName=";
    string uriGetQuestionByID = "https://localhost:44306/api/Question/";
    string uriGameRoom = "https://localhost:44306/api/GameRooms?player1iD=";

    #endregion


    string jsonData;
    void SetJsonData(string json)
    {
        jsonData = json;
        
    }


    void TryAddToInventory(string Done) {
        Debug.Log(Done);


    }
    private void Start()
    {


        //var QuestionData = new QuestionLoader();

        ////  QuestionData.qd.QuestionId = "21";
        //QuestionData.qd.Question = "TRY";
        //QuestionData.qd.CorrectAnswer = "TRY";
        //QuestionData.qd.Answer1 = "TRY";
        //QuestionData.qd.Answer2 = "TRY";
        //QuestionData.qd.Answer3 = "TRY";
        //string obj = JsonUtility.ToJson(QuestionData.qd);



        //Debug.Log(obj);

        //StartCoroutine(WebFetch.HttpPost(uriGetQuestionByID + 2, obj, TryAddToInventory));

        PlayerLoader playerLoader = new PlayerLoader();
        var  player= playerLoader.pd;
        player.PlayerId = 2;
        player.PlayerName = "Rei";
        StartCoroutine(WebFetch.HttpGet(uriCreatePlayerByString + player.PlayerName, SetJsonData));
        StartCoroutine(WebFetch.HttpGet(uriCreatePlayerByString + player.PlayerName+2, SetJsonData));
        StartCoroutine(WebFetch.HttpGet(uriGameRoom + player.PlayerId, SetJsonData));
  

    }


    [Serializable]
    public class PlayerLoader {
        public PlayerData pd;
        public PlayerLoader() { pd = new PlayerData(); }
        [Serializable]
        public class PlayerData {
            public int PlayerId;
            public string PlayerName;
        }
    }

    [Serializable]
    public class QuestionLoader
    {
        public QuestionData qd;
        public QuestionLoader()
        {
            qd = new QuestionData();
        }
        [Serializable]
        public class QuestionData
        {
            public int QuestionId;
            public string Question;
            public string CorrectAnswer;
            public string Answer1;
            public string Answer2;
            public string Answer3;
        }
    }


    [Serializable]
    public class GameRoomLoader {
        public GameRoomData grd;
        public GameRoomLoader() {
            grd = new GameRoomData();
        }

        [Serializable]
        public class GameRoomData {
            public int GameRoomId;
            public string RoomPassword;
            public int Player1Id;
            public int Player2Id;
            public int Player1Score;
            public int Player2Score;
            public float Player1Time;
            public float Player2Time;
            public bool isOnePlayerFinished;
        }
    }
}
