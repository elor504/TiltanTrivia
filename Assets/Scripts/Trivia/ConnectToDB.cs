
using NUnit.Framework.Constraints;
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

    string jsonData;
    void SetJsonData(string json)
    {
        jsonData = json;
        Debug.Log(jsonData);
    }
    private void Start()
    {
        StartCoroutine(WebFetch.ConnectToAPI(connStr, SetJsonData));
        Debug.Log(jsonData);
    }

}
