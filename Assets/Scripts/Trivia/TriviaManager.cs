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
    private string username;
    public string GetSetUsername
    {
        get => username;
        set => username = value;
    }
    private string opponentUsername;
    public int playerID;
    public int roomID;
    public float totalTime;
    public string roomPassword;
    public event Action<bool> SetLoadingEvent;

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
        Reset();
        //uIManager = TriviaUIManager._instance;
        State.uiManager = TriviaUIManager._instance;
        GetSetGameState = new LoginState();
        SetLoadingEvent += (value) => { gameState.SetInputState(!value); };
    }
    public void Reset()
    {
        totalTime = 0;
        GetSetUsername = "";
        opponentUsername = "";
        playerID = 0;
        roomID = 0;
        roomPassword = "";
    }
    public void SetLoading(bool value) => SetLoadingEvent?.Invoke(value);
    public void UpdateGameroomID(Action callback = null) => StartCoroutine(UpdateRoomIdCoro(callback));
    private IEnumerator UpdateRoomIdCoro(Action callback = null)
    {
        yield return StartCoroutine(WebFetch.HttpGet(WebFetch.GetRoomIdURI(playerID), (response) =>
        {
            GetRoomIdResponse(response);
            callback?.Invoke();
        }));
    }
    private void GetRoomIdResponse(HttpResponse response)
    {
        if (response.success)
        {
            if (int.TryParse(response.json, out int gameroomID))
                this.roomID = gameroomID;
            else
            {
                GetSetGameState.SetErrorMessage("Room ID parse error.");
                Debug.LogError("Room ID parse error.");
            }
        }
        else
            GetSetGameState.SetErrorMessage(response.errorMessage);
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