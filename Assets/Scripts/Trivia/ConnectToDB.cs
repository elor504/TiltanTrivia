
using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToDB : MonoBehaviour
{
    //string db_IP = "80.230.92.74:3306";
    //string db_PW = "elor2020";
    // int id = 1;
    //string db_server = "trivia";

    string connStr = "https://localhost:44306/api/Question/1";
    public const string uriCreatePlayerByString = "https://localhost:44306/api/Player?playerName=";

    string jsonData;
    void SetJsonData(string json)
    {
        jsonData = json;
        Debug.Log(jsonData);
    }


    void TryAddToInventory(string Done ) {
        Debug.Log(Done);


    }
    private void Start()
    {


        var QuestionData = new QuestionLibrary();

      //  QuestionData.qd.QuestionId = "21";
        QuestionData.qd.Question = "TRY";
        QuestionData.qd.CorrectAnswer = "TRY";
        QuestionData.qd.Answer1 = "TRY";
        QuestionData.qd.Answer2 = "TRY";
        QuestionData.qd.Answer3 = "TRY";
        string obj = JsonUtility.ToJson(QuestionData.qd);



        string uri = "https://localhost:44306/api/Question";
        Debug.Log(obj);

        StartCoroutine(WebFetch.HttpPost(uri, obj, TryAddToInventory));
    }

    [Serializable]
    public class QuestionLibrary
    {

        public QuestionLibrary()
        {
            qd = new QuestionData();
        }
        public QuestionData qd;



        [Serializable]
        public class QuestionData
        {
            //public string QuestionId;
            public string Question;
            public string CorrectAnswer;
            public string Answer1;
            public string Answer2;
            public string Answer3;
        }

    }
}
