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
                gameState.Exit();
            gameState = value;
            gameState.Enter();
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
    public abstract class State
    {
        static public TriviaUIManager uiManager;
        public void Enter()
        {
            OnEnter();
            SetInputState(true);
        }
        public void Exit()
        {
            SetLoadingEvent(false);
            OnExit();
        }
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void SetInputState(bool state);
        protected void SetLoadingEvent(bool state) => _instance.SetLoading(state);
        protected void SetErrorMessage(string error) => TriviaUIManager._instance.SetErrorMessage(error);
        protected virtual void FailureResponse<T>(HttpResponse<T> response)
        {
            SetErrorMessage(response.errorMessage);
            SetLoadingEvent(false);
        }
    }
    public abstract class GameState : State
    {
        public abstract void SetErrorMessage(string value);
    }
}