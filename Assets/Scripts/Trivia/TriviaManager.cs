using UnityEngine;
public partial class TriviaManager : MonoBehaviour
{
    private GameState gameState;
    public string username;
    public int playerID;
    public int gameroomID;
    public string roomPassword;
    TriviaUIManager uIManager;
    public GameState GetSetGameState {
        get => gameState;
        set {
            gameState.OnExit();
            gameState = value;
            gameState.OnEnter();
        }
    }
    public static TriviaManager _instance;
    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    private void Start() {
        uIManager = TriviaUIManager._instance;
        GetSetGameState = new SignupState();
    }

    private void Update() => GetSetGameState.Update();
    public abstract class GameState
    {
        public abstract void OnEnter();
        public abstract void Update();
        public abstract void OnExit();
    }
}