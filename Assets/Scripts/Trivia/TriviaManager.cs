using System;
using System.Collections;
using UnityEngine;
public partial class TriviaManager : MonoBehaviour
{
    private GameState gameState;
    public GameState GetSetGameState
    {
        get => gameState;
        set
        {
            if (gameState != null)
                gameState.OnExit();
            gameState = value;
            gameState.OnEnter();
        }
    }
    public string username;
    public string GetSetUsername
    {
        get => username;
        set
        {
            username = value;
        }
    }
    public int playerID;
    public int gameroomID;
    public string roomPassword;
    public event Action<bool> SetLoadingEvent;
    //TriviaUIManager uIManager;

    public static TriviaManager _instance;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    private void Start()
    {
        //uIManager = TriviaUIManager._instance;
        State.uiManager = TriviaUIManager._instance;
        GetSetGameState = new LoginState();
        SetLoadingEvent += (value) => { gameState.SetInputState(!value); };
    }
    public void SetLoading(bool value) => SetLoadingEvent?.Invoke(value);
    public void UpdateGameroomID(Action callback = null) => StartCoroutine(UpdateRoomIdCoro(callback));
    private IEnumerator UpdateRoomIdCoro(Action callback = null)
    {
        yield return StartCoroutine(WebFetch.HttpGet(WebFetch.GetRoomIdURI(playerID), (success, json, errorMessage) =>
        {
            GetRoomIdResponse(success, json, errorMessage);
            callback();
        }));
    }
    private void GetRoomIdResponse(bool success, string json, string errorMessage)
    {
        if (success && bool.TryParse(json, out bool jsonSuccess))
        {
            if (int.TryParse(json, out int gameroomID))
                this.gameroomID = gameroomID;
            else
            {
                GetSetGameState.SetErrorMessage("Room ID parse error.");
                Debug.LogError("Room ID parse error.");
            }
        }
        else
            GetSetGameState.SetErrorMessage(errorMessage);
            SetLoadingEvent(false);
    }
    public abstract class State
    {
        static public TriviaUIManager uiManager;
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void SetInputState(bool state);
    }
    public abstract class GameState : State
    {
        public abstract void SetErrorMessage(string value);
    }
}