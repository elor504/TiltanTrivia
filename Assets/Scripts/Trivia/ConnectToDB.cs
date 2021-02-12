
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectToDB : MonoBehaviour
{
    string db_IP = "80.230.92.74:3306";
    string db_PW = "elor2020";

    string db_server = "trivia";
    //  mysql --host=localhost --user=myname --password=password mydb
    int id = 1;
        string connStr = "http://localhost:44306/api/Question/";
    string selectAll = "Select * From testTable";
    IEnumerator SendRequest(string url) {
        UnityWebRequest webReq = UnityWebRequest.Get(url);
             
        yield return webReq.SendWebRequest();

        if (webReq.isNetworkError || webReq.isHttpError)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log("ConnectionAcksType");
        

        }
    }
    private void Start()
    {
        StartCoroutine(SendRequest(connStr));
    }


}
