using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum QuestionRecord { Unanswered, Wrong, Right }
public enum ResultWindowState { Waiting, Won, Lost }
public class TriviaUIManager : MonoBehaviour
{
    #region UI elements refrence
    #region Login elements
    [Serializable]
    public class MainLoginWindow
    {
        public GameObject mainGameobject;
        [SerializeField] Button findMatchButton;
        public Button GetFindMatchButton => findMatchButton;
        [SerializeField] Button createRoomButton;
        public Button GetCreateRoomButton => createRoomButton;
        [SerializeField] Button joinRoomButton;
        public Button GetJoinRoomButton => joinRoomButton;
        public void SetInputState(bool value)
        {
            findMatchButton.interactable = value;
            createRoomButton.interactable = value;
            joinRoomButton.interactable = value;
        }
    }
    [Serializable]
    public class SignupWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TMP_InputField usernameInput;
        public TMP_InputField GetInput => usernameInput;
        public void SetInputState(bool value)
        {
            usernameInput.interactable = value;
        }
    }
    [Serializable]
    public class CreateRoomWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TMP_InputField passwordInput;
        public TMP_InputField GetInput => passwordInput;
        [SerializeField] Button confirmButton;
        public Button GetConfirmButton => confirmButton;
        [SerializeField] Button exitButton;
        public Button GetExitButton => exitButton;
        public void SetInputState(bool value)
        {
            passwordInput.interactable = value;
            confirmButton.interactable = value;
            exitButton.interactable = value;
        }
    }
    [Serializable]
    public class JoinRoomWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TMP_InputField idInput;
        public TMP_InputField GetIdInput => idInput;
        [SerializeField] TMP_InputField pwInput;
        public TMP_InputField GetPwInput => pwInput;
        [SerializeField] Button confirmButton;
        public Button GetConfirmButton => confirmButton;
        [SerializeField] Button exitButton;
        public Button GetExitButton => exitButton;
        public void SetInputState(bool value)
        {
            idInput.interactable = value;
            pwInput.interactable = value;
            confirmButton.interactable = value;
            exitButton.interactable = value;
        }
    }
    public MainLoginWindow mainLoginWindow;
    public SignupWindow signupWindow;
    public CreateRoomWindow createRoomWindow;
    public JoinRoomWindow joinRoomWindow;
    public void InputFieldEvent_Register(TMP_InputField inputField, UnityAction<string> unityEvent) => inputField.onEndEdit.AddListener(unityEvent);
    public void InputFieldEvent_Unregister(TMP_InputField inputField, UnityAction<string> unityEvent) => inputField.onEndEdit.RemoveListener(unityEvent);
    public void ButtonEvent_Register(Button button, UnityAction unityEvent) => button.onClick.AddListener(unityEvent);
    public void ButtonEvent_Unregister(Button button, UnityAction unityEvent) => button.onClick.RemoveListener(unityEvent);
    #endregion
    #region Trivia elements
    [Serializable]
    public class GameWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TextMeshProUGUI timerText;
        public string GetSetTimerText
        {
            get => timerText.text;
            set => timerText.text = value;
        }
        public TextMeshProUGUI questionText;
        public string GetSetQuestionText
        {
            get => questionText.text;
            set => questionText.text = value;
        }
        [SerializeField] TextMeshProUGUI[] answersText;
        public void UpdateAnswerUIText(string[] answers)
        {
            for (int i = 0; i < answers.Length; i++)
                answersText[i].text = answers[i];
        }
        public void UpdateQuestion(string question, string[] answers)
        {
            GetSetQuestionText = question;
            UpdateAnswerUIText(answers);
        }
    }
    [Serializable]
    public class PlayersWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TextMeshProUGUI player1Name;
        public string GetSetPlayer1Name
        {
            get => player1Name.text;
            set => player1Name.text = value;
        }
        [SerializeField] TextMeshProUGUI player2Name;
        public string GetSetPlayer2Name
        {
            get => player2Name.text;
            set => player2Name.text = value;
        }
        [SerializeField] Image player2Status;
        public void SetPlayer2State(bool loggedIn) => player2Status.color = (loggedIn ? Color.green : Color.red);
        [SerializeField] Image[] player1QuestionBar;
        [SerializeField] Image[] player2QuestionBar;
        private Color RecordToColor(QuestionRecord record)
        {
            switch (record)
            {
                case QuestionRecord.Wrong:
                    return new Color32(255, 0, 0, 220);
                case QuestionRecord.Right:
                    return new Color32(0, 255, 0, 163);
                default:
                    return new Color32(255, 255, 255, 0);
            }
        }
        public void SetPlayer1QuestionRecord(int questionNum, QuestionRecord record) => player1QuestionBar[questionNum].color = RecordToColor(record);
        public void ResetPlayer1Progress()
        {
            for (int i = 0; i < player1QuestionBar.Length; i++)
                SetPlayer1QuestionRecord(i, QuestionRecord.Unanswered);
        }
        public void ResetPlayer2Progress()
        {
            for (int i = 0; i < player2QuestionBar.Length; i++)
                SetPlayer2QuestionRecord(i, QuestionRecord.Unanswered);
        }
        public void SetPlayer2QuestionRecord(int questionNum, QuestionRecord record) => player2QuestionBar[questionNum].color = RecordToColor(record);
    }
    [Serializable]
    public class WaitingWindow
    {
        public GameObject mainGameobject;
        [SerializeField] TextMeshProUGUI roomID;
        public string GetSetRoomID
        {
            get => roomID.text;
            set => roomID.text = value;
        }
    }
    [Serializable]
    public class ResultsWindow
    {
        public GameObject mainGameobject;
        [SerializeField] GameObject WaitingText_GO;
        [SerializeField] GameObject LostText_GO;
        [SerializeField] GameObject WonText_GO;
        public void SetResult(ResultWindowState state)
        {
            WaitingText_GO.SetActive(state == ResultWindowState.Waiting);
            WonText_GO.SetActive(state == ResultWindowState.Won);
            LostText_GO.SetActive(state == ResultWindowState.Lost);
        }
        [SerializeField] TextMeshProUGUI yourTime;
        public string GetSetYourTime
        {
            get => yourTime.text;
            set => yourTime.text = value;
        }
        [SerializeField] TextMeshProUGUI opponentText;
        public string GetSetOpponentText
        {
            get => opponentText.text;
            set => opponentText.text = value;
        }
        [SerializeField] Button returnToMainMenuButton;
        public Button GetReturnToMainMenuButton => returnToMainMenuButton;

    }
    public GameWindow gameWindow;
    public PlayersWindow playersWindow;
    public WaitingWindow waitingWindow;
    public ResultsWindow resultsWindow;
    #endregion
    [SerializeField] TextMeshProUGUI errorMessage;
    #endregion
    public static TriviaUIManager _instance;
    public TriviaManager triviaElements;

    private void Start()
    {
        triviaElements = TriviaManager._instance;
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #region Trivia login windows
    //Signup window
    public void SetSignupWindow(bool state)
    {
        signupWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }
    //General
    public void SetMainLoginWindow(bool state)
    {
        mainLoginWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }

    //Create room window
    public void SetCreateRoomWindow(bool state)
    {
        createRoomWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }
    //Join room window
    public void SetJoinRoomWindow(bool state)
    {
        joinRoomWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }
    public void SetErrorMessage(string errorText)
    {
        if (errorText != "")
        {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = errorText;
            Debug.LogError(errorText);
        }
    }
    private void HideErrorMessage()
    {
        errorMessage.text = "";
        errorMessage.gameObject.SetActive(false);
    }
    #endregion
    #region Trivia UI
    public void OpenTriviaUI() => playersWindow.mainGameobject.SetActive(true);
    public void CloseTriviaUI() => playersWindow.mainGameobject.SetActive(false);
    public void UpdateTimerUI(float Time) => gameWindow.GetSetTimerText = Mathf.CeilToInt(Time).ToString();
    #endregion
    private void ParseString(string inputString, ref int outputInt, string errorMessage = "")
    {
        if (int.TryParse(inputString, out int parsedInt))
            outputInt = parsedInt;
        else if (errorMessage != "")
            SetErrorMessage(errorMessage);
    }
}