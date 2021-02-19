using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        public void SetInputState(bool value) {
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
        public void SetInputState(bool value) {
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
        public void SetInputState(bool value) {
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
        public void SetInputState(bool value) {
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
    [SerializeField] TextMeshProUGUI errorMessage;
    public void InputFieldEvent_Register(TMP_InputField inputField, UnityAction<string> unityEvent) => inputField.onEndEdit.AddListener(unityEvent);
    public void InputFieldEvent_Unregister(TMP_InputField inputField, UnityAction<string> unityEvent) => inputField.onEndEdit.RemoveListener(unityEvent);
    public void ButtonEvent_Register(Button button, UnityAction unityEvent) => button.onClick.AddListener(unityEvent);
    public void ButtonEvent_Unregister(Button button, UnityAction unityEvent) => button.onClick.RemoveListener(unityEvent);
    #endregion
    #region Trivia elements
    [Serializable]
    public class TriviaWindow
    {
        public GameObject mainGameobject;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI questionText;
        public TextMeshProUGUI[] answersText;
    }
    [SerializeField] TriviaWindow triviaWindow;

    #endregion
    #endregion
    public static TriviaUIManager _instance;
    public TriviaManager triviaElements;

    private void Start() {
        triviaElements = TriviaManager._instance;
    }
    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #region Trivia login windows
    //Signup window
    public void OpenSignupUI() => signupWindow.mainGameobject.SetActive(true);
    public void CloseSignupUI() {
        signupWindow.mainGameobject.SetActive(false);
        HideErrorMessage();
    }
    //General
    public void SetMainLoginWindow(bool state) => mainLoginWindow.mainGameobject.SetActive(state);

    //Create room window
    public void SetCreateRoomWindow(bool state) {
        createRoomWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }
    //Join room window
    public void SetJoinRoomWindow(bool state) {
        joinRoomWindow.mainGameobject.SetActive(state);
        HideErrorMessage();
    }
    public void SetErrorMessage(string errorText) {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = errorText;
        Debug.LogError(errorText);
    }
    private void HideErrorMessage() {
        errorMessage.text = "";
        errorMessage.gameObject.SetActive(false);
    }
    #endregion
    #region Trivia UI
    public void OpenTriviaUI() => triviaWindow.mainGameobject.SetActive(true);
    public void CloseTriviaUI() => triviaWindow.mainGameobject.SetActive(false);
    public void UpdateQuestion(string Question, string answer1, string answer2, string answer3, string answer4) {
        UpdateQuestionUIText(Question);
        string[] AnswerCache = new string[] { answer1, answer2, answer3, answer4 };
        UpdateAnswerUIText(AnswerCache);
    }
    void UpdateQuestionUIText(string text) => triviaWindow.questionText.text = text;
    void UpdateAnswerUIText(string[] answers) {
        for (int i = 0; i < answers.Length; i++)
            triviaWindow.answersText[i].text = answers[i];
    }
    void AnswerButton(int answerNum) { }
    public void UpdateTimerUI(float Time) {
        triviaWindow.timerText.text = Mathf.CeilToInt(Time).ToString();
        if (Time < 5) {

        }
    }
    #endregion
    private void ParseString(string inputString, ref int outputInt, string errorMessage = "") {
        if (int.TryParse(inputString, out int parsedInt))
            outputInt = parsedInt;
        else if (errorMessage != "")
            SetErrorMessage(errorMessage);
    }
}