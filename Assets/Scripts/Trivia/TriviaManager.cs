using System;
using UnityEngine;
public partial class TriviaManager : MonoBehaviour
{
    private GameState gameState;
    public GameState GetSetGameState {
        get => gameState;
        set {
            if (gameState != null)
                gameState.OnExit();
            gameState = value;
            gameState.OnEnter();
        }
    }
    public string username;
    public string GetSetUsername {
        get => username;
        set {
            username = value;
            Signup();
        }
    }

    private void Signup() {
        //Signup in the server
        //If fail show error message
        //If works move to login state
    }

    public int playerID;
    public int gameroomID;
    public string roomPassword;
    public event Action<bool> setLoadingEvent;
    //TriviaUIManager uIManager;

    public static TriviaManager _instance;
    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    private void Start() {
        //uIManager = TriviaUIManager._instance;
        State.uiManager = TriviaUIManager._instance;
        GetSetGameState = new LoginState();
    }

    private void Update() => GetSetGameState.Update();
    public abstract class State
    {
        static public TriviaUIManager uiManager;
        public abstract void OnEnter();
        public abstract void Update();
        public abstract void OnExit();
        public abstract void SetInputState(bool state);
    }
    public abstract class GameState : State
    {
        public abstract void SetErrorMessage(string value);
    }
}