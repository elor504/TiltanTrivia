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

        TimerTest -= Time.deltaTime;
        if (TimerTest > 0)
        {
            UpdateTimerUI(TimerTest);
        }
       

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
    }



}
