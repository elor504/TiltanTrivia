using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriviaUIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI questionText;
    [SerializeField]
    TextMeshProUGUI[] answersText;
    [SerializeField]
    Button[] answerButts;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    float TimerTest;

    public LoginUI loginUI;

    private void Start()
    {

        UpdateQuestion("Question Test", "This Test Is Stupid", "It Works!", "Stop it, it tickles!", "kaki");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            UpdateQuestion("Question Test", "This Test Is Stupid", "It Works!", "Stop it, it tickles!", "kaki");
        }


        if (TimerTest >= 0)
        {
            TimerTest -= Time.deltaTime;
            UpdateTimerUI(TimerTest);
        }


    }


    #region TriviaUI
    [SerializeField]
    GameObject TriviaUIGO;
    public void OpenTriviaUI()
    {
        TriviaUIGO.SetActive(true);
    }

    public void CloseTriviaUI()
    {
        TriviaUIGO.SetActive(false);
    }


    public void UpdateQuestion(string Question, string answer1, string answer2, string answer3, string answer4)
    {
        RemoveButtonsEvent();
        questionText.text = Question;

        string[] AnswerCache = GetComponent<TriviaManager>().AnswerRandomizer(answer1, answer2, answer3, answer4);

        UpdateAnswerText(AnswerCache);
        UpdateButtonsEvent(AnswerCache);

    }

    void UpdateAnswerText(string[] Answers)
    {
        answersText[0].text = Answers[0];
        answersText[1].text = Answers[1];
        answersText[2].text = Answers[2];
        answersText[3].text = Answers[3];
    }

    void UpdateButtonsEvent(string[] Answer)
    {
        answerButts[0].onClick.AddListener(delegate { ClickOnAnswer(Answer[0]); });
        answerButts[1].onClick.AddListener(delegate { ClickOnAnswer(Answer[1]); });
        answerButts[2].onClick.AddListener(delegate { ClickOnAnswer(Answer[2]); });
        answerButts[3].onClick.AddListener(delegate { ClickOnAnswer(Answer[3]); });
    }

    void RemoveButtonsEvent()
    {
        for (int i = 0; i < answerButts.Length; i++)
        {
            answerButts[i].onClick.RemoveAllListeners();
        }
    }


    void ClickOnAnswer(string Answer)
    {
        Debug.Log("Test On Buttons: " + Answer);
    }


    public void UpdateTimerUI(float Time)
    {
        timerText.text = Mathf.CeilToInt(Time).ToString();
        if (Time < 5)
        {

        }
    }


    #endregion


    #region TriviaLogin
    [SerializeField]
    GameObject loginUIGO;
    [SerializeField]
    GameObject PasswordPanel;

    public void OpenLoginUI()
    {
        loginUIGO.SetActive(true);
    }

    public void CloseLoginUI()
    {
        loginUIGO.SetActive(false);
    }

    public void OnChangeNameInput()
    {
        loginUI.GetPlayerName(loginUI.getPlayerNameInput.text);
    }

    public void OnChangeRoomId()
    {


        if (loginUI.getRoomIdInput.text.Length != 0)
        {
            loginUI.GetRoomId(int.Parse(loginUI.getRoomIdInput.text));
        }
        else
        {
            loginUI.GetRoomId(0);
        }


    }
        #endregion
    public void OnChangePasswordInput()
    {
        loginUI.OnChangePassword();
    }
    public void OnChangeJoiningPasswordInput()
    {
        loginUI.OnChangeRoomJoinPassword();
    }


    public void OnClickHasPasswordToggle()
    {
        loginUI.ToggleHasPassword();
    }



    public void OnClickRandomMatch()
    {
        Debug.Log("Searching for random game");
    }

    public void OnClickCreateRoom()
    {
        if (loginUI.getHasPasswordToggle.isOn)
        {
            Debug.Log("Creating room with password");
        }
        else
        {
            Debug.Log("Creating room without password");
        }
      
    }

    public void OnClickJoinRoomWithId()
    {
        Debug.Log("Joining Room With Id that has no password");
        //add functionality that see if room ask for password if it does it will pop up a panel with password
        if (true)
        {
            PasswordPanel.SetActive(true);
        }

    }

    public void ClosePasswordPanel()
    {
        PasswordPanel.SetActive(false);
    }


    public void OnClickJoinRoomWithPassword()
    {

    }

    }

[Serializable]
public class LoginUI
{
    [Header("Login Related")]
    [SerializeField]
    TMP_InputField playerNameInput;
    public TMP_InputField getPlayerNameInput => playerNameInput;

    [SerializeField]
    string thisPlayerName;

    [SerializeField]
    bool isRoomPrivate;


    [SerializeField]
    TMP_InputField roomIdInput;
    public TMP_InputField getRoomIdInput => roomIdInput;

    [SerializeField]
    TMP_InputField passwordInput;

    [SerializeField]
    TMP_InputField roomPasswordInput;

    [SerializeField]
    string roomPassword;
    [SerializeField]
    string joiningRoomPassword;
    public string getJoiningRoomPassword => joiningRoomPassword;
    [SerializeField]
    int searchRoomId;
    public int getSearchRoomId => searchRoomId;



    [SerializeField]
    Toggle hasPaswordToggle;
    public Toggle getHasPasswordToggle => hasPaswordToggle;





    public void CreateRoom()
    {
        Debug.Log("Creating Room");
    }

    public void GetRoomId(int Id)
    {
        searchRoomId = Id;
    }

    public void GetPlayerName(string Name)
    {
        thisPlayerName = Name;
    }

    public void ToggleHasPassword()
    {
        isRoomPrivate = hasPaswordToggle.isOn;

        passwordInput.interactable = hasPaswordToggle.isOn;

    }

    public void OnChangePassword()
    {
        if(passwordInput.text.Length != 0)
        {
            roomPassword = passwordInput.text;
        }
        else
        {
            roomPassword = null;
        }
       
    }

    public void OnChangeRoomJoinPassword()
    {
        if (roomPasswordInput.text.Length != 0)
        {
            joiningRoomPassword = roomPasswordInput.text;
        }
        else
        {
            joiningRoomPassword = null;
        }
    }



}
