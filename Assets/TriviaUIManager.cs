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
    Button[] AnswerButts;


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
        AnswerButts[0].onClick.AddListener(delegate { ClickOnAnswer(Answer[0]); });
        AnswerButts[1].onClick.AddListener(delegate { ClickOnAnswer(Answer[1]); });
        AnswerButts[2].onClick.AddListener(delegate { ClickOnAnswer(Answer[2]); });
        AnswerButts[3].onClick.AddListener(delegate { ClickOnAnswer(Answer[3]); });
    }

    void RemoveButtonsEvent()
    {
        for (int i = 0; i < AnswerButts.Length; i++)
        {
            AnswerButts[i].onClick.RemoveAllListeners();
        }
    }


    void ClickOnAnswer(string Answer)
    {
        Debug.Log("Test On Buttons: " + Answer);
    }


}
