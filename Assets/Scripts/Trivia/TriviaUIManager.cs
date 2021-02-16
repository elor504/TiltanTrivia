using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriviaUIManager : MonoBehaviour
{
    #region UI elements refrence
    #region Login elements
    #endregion
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI[] answersText;
    [SerializeField] Button[] answerButts;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float TimerTest;
    [Header("Login Related")]
    [SerializeField] string thisPlayerName;
    [SerializeField] bool isRoomPrivate;
    [SerializeField] string roomPassword;
    [SerializeField] string joiningRoomPassword;
    [SerializeField] int searchRoomId;
    [SerializeField] Toggle hasPaswordToggle;
    [SerializeField] GameObject joinRoomUIGO;
    [SerializeField] GameObject createRoomUIGO;
    [SerializeField] GameObject triviaUIGO;
    [SerializeField] GameObject PasswordPanel;
    #endregion
    #region UI variables
    private string userName;
    private int joinRoomID;
    private string joinRoomPW;
    private string createRoomPW;
    #endregion

    private void Start() {
        UpdateQuestion("Question Test", "This Test Is Stupid", "It Works!", "Stop it, it tickles!", "kaki");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            UpdateQuestion("Question Test", "This Test Is Stupid", "It Works!", "Stop it, it tickles!", "kaki");
        }
        if (TimerTest >= 0) {
            TimerTest -= Time.deltaTime;
            UpdateTimerUI(TimerTest);
        }
    }

    #region TriviaUI

    public void OpenTriviaUI() {
        triviaUIGO.SetActive(true);
    }

    public void CloseTriviaUI() {
        triviaUIGO.SetActive(false);
    }


    public void UpdateQuestion(string Question, string answer1, string answer2, string answer3, string answer4) {
        RemoveButtonsEvent();
        questionText.text = Question;

        string[] AnswerCache = GetComponent<TriviaManager>().AnswerRandomizer(answer1, answer2, answer3, answer4);

        UpdateAnswerText(AnswerCache);
        UpdateButtonsEvent(AnswerCache);

    }

    void UpdateAnswerText(string[] Answers) {
        answersText[0].text = Answers[0];
        answersText[1].text = Answers[1];
        answersText[2].text = Answers[2];
        answersText[3].text = Answers[3];
    }

    void UpdateButtonsEvent(string[] Answer) {
        answerButts[0].onClick.AddListener(delegate { ClickOnAnswer(Answer[0]); });
        answerButts[1].onClick.AddListener(delegate { ClickOnAnswer(Answer[1]); });
        answerButts[2].onClick.AddListener(delegate { ClickOnAnswer(Answer[2]); });
        answerButts[3].onClick.AddListener(delegate { ClickOnAnswer(Answer[3]); });
    }
    void RemoveButtonsEvent() {
        for (int i = 0; i < answerButts.Length; i++) {
            answerButts[i].onClick.RemoveAllListeners();
        }
    }
    void ClickOnAnswer(string Answer) => Debug.Log("Test On Buttons: " + Answer);
    public void UpdateTimerUI(float Time) {
        timerText.text = Mathf.CeilToInt(Time).ToString();
        if (Time < 5) {

        }
    }
    #endregion
    #region Trivia login windows
    //Main login window
    public void SetUserName(string userName) => this.userName = userName;

    //Create room window
    public void SetCreateRoomPW(string PW) => createRoomPW = PW;
    public void OpenCreateRoomUI() => createRoomUIGO.SetActive(true);
    public void CloseCreateRoomUI() => createRoomUIGO.SetActive(false);
    public void ConfirmRoomCreation() { }

    //Join room window
    public void OpenJoinRoomUI() => joinRoomUIGO.SetActive(true);
    public void CloseJoinRoomUI() => joinRoomUIGO.SetActive(false);
    public void SetJoinRoomID(int ID) => joinRoomID = ID;
    public void SetJoinRoomPW(string PW) => joinRoomPW = PW;
    public void ConfirmJoinRoom() { }

    //Find room
    public void FindRoomButton() => Debug.Log("Searching for random game");
    #endregion
}