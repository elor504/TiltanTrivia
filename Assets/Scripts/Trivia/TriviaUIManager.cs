using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriviaUIManager : MonoBehaviour
{
    #region UI elements refrence
    #region Login elements
    [Header("Login Windows")]
    [SerializeField] GameObject signupRoomUI_GO;
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] GameObject joinRoomUI_GO;
    [SerializeField] GameObject createRoomUI_GO;
    #endregion
    #region Trivia elements

    [SerializeField] GameObject triviaUIGO;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI[] answersText;
    #endregion
    #endregion
    public static TriviaUIManager _instance;
    TriviaManager triviaManager;

    private void Start() {
        triviaManager = TriviaManager._instance;
    }
    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #region Trivia UI
    public void OpenTriviaUI() => triviaUIGO.SetActive(true);
    public void CloseTriviaUI() => triviaUIGO.SetActive(false);
    public void UpdateQuestion(string Question, string answer1, string answer2, string answer3, string answer4) {
        UpdateQuestionUIText(Question);
        string[] AnswerCache = new string[] { answer1, answer2, answer3, answer4 };
        UpdateAnswerUIText(AnswerCache);
    }
    void UpdateQuestionUIText(string text) => questionText.text = text;
    void UpdateAnswerUIText(string[] answers) {
        for (int i = 0; i < answers.Length; i++)
            answersText[i].text = answers[i];
    }
    void AnswerButton(int answerNum) { }
    public void UpdateTimerUI(float Time) {
        timerText.text = Mathf.CeilToInt(Time).ToString();
        if (Time < 5) {

        }
    }
    #endregion
    #region Trivia login windows
    //Signup window
    public void SetUserName(string userName) => triviaManager.username = userName;

    //Create room window
    public void OpenCreateRoomUI() => createRoomUI_GO.SetActive(true);
    public void CloseCreateRoomUI() {
        createRoomUI_GO.SetActive(false);
        HideErrorMessage();
    }
    public void ConfirmRoomCreation() { }

    //Join room window
    public void OpenJoinRoomUI() => joinRoomUI_GO.SetActive(true);
    public void CloseJoinRoomUI() {
        joinRoomUI_GO.SetActive(false);
        HideErrorMessage();
    }
    public void ConfirmJoinRoom() { }
    //General
    public void SetRoomID(string ID) => ParseString(ID, ref triviaManager.gameroomID, "Password's not a int");
    public void SetRoomPW(string PW) => triviaManager.roomPassword = PW;
    //Find room
    public void FindRoomButton() => Debug.Log("Searching for random game");

    private void SetErrorMessage(string errorText) {
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = errorText;
    }
    private void HideErrorMessage() => errorMessage.gameObject.SetActive(false);
    #endregion
    private void ParseString(string inputString, ref int outputInt, string errorMessage = "") {
        if (int.TryParse(inputString, out int parsedInt))
            outputInt = parsedInt;
        else if (errorMessage != "")
            SetErrorMessage(errorMessage);
    }
}