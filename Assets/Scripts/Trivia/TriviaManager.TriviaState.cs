using System.Collections;
using UnityEngine;
public partial class TriviaManager
{
    //Main state when starting the game
    class TriviaState : GameState
    {
        public static TriviaState triviaState;
        StateAtTrivia stateAtLogin;
        public StateAtTrivia GetSetStateAtTrivia
        {
            get => stateAtLogin;
            set
            {
                if (stateAtLogin != null)
                    stateAtLogin.OnExit();
                stateAtLogin = value;
                stateAtLogin.OnEnter();
            }
        }
        public override void OnEnter()
        {
            triviaState = this;
            _instance.SetLoadingEvent += SetInputState;
            uiManager.playersWindow.mainGameobject.SetActive(true);
            uiManager.playersWindow.SetPlayer2State(false);
            uiManager.playersWindow.ResetPlayer1Progress();
            uiManager.playersWindow.ResetPlayer2Progress();
            uiManager.playersWindow.GetSetPlayer1Name = _instance.GetSetUsername;
            uiManager.playersWindow.GetSetPlayer2Name = "NOT LOGGED IN";
        }
        public override void OnExit()
        {
            uiManager.playersWindow.mainGameobject.SetActive(false);
            _instance.SetLoadingEvent -= SetInputState;
        }
        public override void SetErrorMessage(string value)
        {
        }
        public override void SetInputState(bool state)
        {
        }
        public abstract class StateAtTrivia : State
        {
            protected void SetTriviaState(StateAtTrivia stateAtTrivia) => triviaState.GetSetStateAtTrivia = stateAtTrivia;
            protected void SetLoadingEvent(bool state) => _instance.SetLoadingEvent(state);
            protected void SetErrorMessage(string error) => TriviaUIManager._instance.SetErrorMessage(error);
        }
        private class WaitingRoomState : StateAtTrivia
        {
            const float updateInterval = 1f;
            const string gameRoomIdText = "Room ID: ";
            public override void OnEnter()
            {
                uiManager.waitingWindow.mainGameobject.SetActive(true);
                uiManager.waitingWindow.GetSetRoomID = gameRoomIdText + _instance.roomID.ToString();
                _instance.StartCoroutine(CheckForOpponent(0));
            }
            public override void OnExit()
            {
                uiManager.waitingWindow.mainGameobject.SetActive(false);
            }
            public override void SetInputState(bool state) { }
            IEnumerator CheckForOpponent(float delay)
            {
                if (delay > 0)
                    yield return new WaitForSeconds(delay);
                WebFetch.HttpGet(WebFetch.GetIsPlayer2LoggedInURI(_instance.roomID), CheckForOpponentResponse);
            }
            void CheckForOpponentResponse(HttpResponse response)
            {
                if (response.success)
                {
                    if (bool.TryParse(response.json, out bool jsonSuccess) && jsonSuccess)
                    {
                        Debug.Log(jsonSuccess);
                        if (jsonSuccess)
                        {
                            uiManager.playersWindow.SetPlayer2State(true);
                            SetTriviaState(new GameRunningState());
                        }
                        else
                            CheckForOpponent(updateInterval);
                    }
                    else
                        SetErrorMessage("Json success parse error.");
                }
                else
                    SetErrorMessage(response.errorMessage);
            }
            //void UpdateOpponentUsername() =>  WebFetch.HttpGet(WebFetch.)
        }
        private class GameRunningState : StateAtTrivia
        {
            const float opponentProgressUpdateInterval = 1f;
            bool IsUpdatingOpponentProgress;
            Question currentQuestion;
            public override void OnEnter()
            {
                uiManager.gameWindow.mainGameobject.SetActive(true);
                uiManager.gameWindow.GetSetTimerText = "0";
                IsUpdatingOpponentProgress = true;
                _instance.StartCoroutine(StartUpdatingOpponentProgress());
            }
            public override void OnExit()
            {
                uiManager.gameWindow.mainGameobject.SetActive(false);
                IsUpdatingOpponentProgress = false;
                _instance.StopCoroutine(StartUpdatingOpponentProgress());
            }

            public override void SetInputState(bool state)
            {

            }
            IEnumerator StartUpdatingOpponentProgress()
            {
                while (IsUpdatingOpponentProgress)
                {
                    yield return new WaitForSeconds(opponentProgressUpdateInterval);
                }
            }
            private void LoadQuestion()
            {
                currentQuestion = new Question()
                {
                    question = "How nice is my coding?",
                    Answer1 = "WOW!",
                    Answer2 = "OMG!",
                    Answer3 = "Mehhh...",
                    Answer4 = "Coolio"
                };
                Answer[] answers = new Answer[4];
                answers[0] = new Answer(1, currentQuestion.Answer1);
                answers[1] = new Answer(2, currentQuestion.Answer2);
                answers[2] = new Answer(3, currentQuestion.Answer3);
                answers[3] = new Answer(4, currentQuestion.Answer4);
                HelperFunc.ArrayShuffle(ref answers);
            }
            class Question
            {
                public string question;
                public string Answer1;
                public string Answer2;
                public string Answer3;
                public string Answer4;
            }
            struct Answer
            {
                public int answerNum;
                public string answer;
                public Answer(int answerNum, string answer)
                {
                    this.answerNum = answerNum;
                    this.answer = answer;
                }
                public static string[] extractStrings(Answer[] answers)
                {
                    string[] answersText = new string[answers.Length];
                    for (int i = 0; i < answers.Length; i++)
                        answersText[i] = answers[i].answer;
                    return answersText;
                }
            }
        }
    }
}