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
            uiManager.playersWindow.ResetPlayerProgress();
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
                            UpdateOpponentUsername();
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
            void UpdateOpponentUsername()
            {
                SetLoadingEvent(true);
                WebFetch.HttpGet(WebFetch.GetOpponentUsernameURI(_instance.roomID, _instance.playerID), UpdateOpponentUsernameResponse);
            }
            void UpdateOpponentUsernameResponse(HttpResponse response)
            {
                if (response.success)
                {
                    _instance.opponentUsername = response.json;
                    uiManager.playersWindow.GetSetPlayer2Name = response.json;
                    SetLoadingEvent(false);
                    SetTriviaState(new GameRunningState());
                }
                else
                {
                    SetLoadingEvent(true);
                    SetErrorMessage(response.errorMessage);
                }
            }
        }
        private class GameRunningState : StateAtTrivia
        {
            Question currentQuestion;
            Question GetSetCurrentQuestion
            {
                get => currentQuestion;
                set
                {
                    currentQuestion = value;
                    uiManager.gameWindow.UpdateQuestion(value.question, Answer.AnswersToStrings(value.answers));
                }
            }
            int currentQuestionNum = 1;
            int selectedAnswer;
            public override void OnEnter()
            {
                currentQuestionNum = 1;
                uiManager.gameWindow.mainGameobject.SetActive(true);
                uiManager.gameWindow.buttonClicked += AnsweredQuestion;
                _instance.totalTime = 0;
                uiManager.gameWindow.UpdateTimer(0);
                _instance.StartCoroutine(UpdateTimer());
                FetchQuestion();
            }
            public override void OnExit()
            {
                _instance.StopCoroutine(UpdateTimer());
                uiManager.gameWindow.mainGameobject.SetActive(false);
                uiManager.gameWindow.buttonClicked -= AnsweredQuestion;
            }
            IEnumerator UpdateTimer()
            {
                while (true)
                {
                    yield return null;
                    _instance.totalTime += Time.deltaTime;
                    uiManager.gameWindow.UpdateTimer(_instance.totalTime);
                }
            }
            public override void SetInputState(bool state)
            {

            }
            private void FetchQuestion()
            {
                SetLoadingEvent(true);
                WebFetch.HttpGet(WebFetch.GetQuestionURI(_instance.roomID, _instance.playerID), FetchQuestionResponse);
            }
            private void FetchQuestionResponse(HttpResponse response)
            {
                if (response.success)
                {
                    if (JsonParser.TryParseJson(response.json, out ResponseQuestion responseQuestion))
                    {
                        LoadQuestion(responseQuestion);
                    }
                    else
                        SetErrorMessage("Failed to parse question json!");

                }
                else
                    SetErrorMessage(response.errorMessage);
                SetLoadingEvent(false);

            }
            void LoadQuestion(ResponseQuestion responseQuestion)
            {
                Answer[] answers = Answer.StringsToAnswers(responseQuestion.answers);
                HelperFunc.ArrayShuffle(ref answers);
                currentQuestion = new Question(responseQuestion.question, answers);
            }
            void AnsweredQuestion(int answerNum)
            {
                selectedAnswer = answerNum;
                SetLoadingEvent(true);
                WebFetch.HttpGet(WebFetch.GetInsertAnswerURI(_instance.roomID, _instance.playerID, answerNum), InsertAnswerResponse);
            }
            void InsertAnswerResponse(HttpResponse response)
            {
                if (response.success)
                {
                    if (int.TryParse(response.json, out int correctAnswer))
                    {
                        uiManager.playersWindow.SetPlayerQuestionRecord(currentQuestionNum, (selectedAnswer == correctAnswer) ? QuestionRecord.Right : QuestionRecord.Wrong);
                        currentQuestionNum++;
                        CheckIfFinished();
                    }
                    else
                    {
                        SetErrorMessage("Failed to parse correct answer json");
                        SetLoadingEvent(false);

                    }
                }
                else
                {
                    SetErrorMessage(response.errorMessage);
                    SetLoadingEvent(false);
                }
            }

            private void CheckIfFinished() => WebFetch.HttpGet(WebFetch.GetPlayerFinishedURI(_instance.roomID, _instance.playerID), CheckIfFinishedResponse);
            void CheckIfFinishedResponse(HttpResponse response)
            {
                if (response.success)
                {
                    if (bool.TryParse(response.json, out bool finished))
                    {
                        if (finished)
                        {
                            SetLoadingEvent(false);
                            SetTriviaState(new ResultsState());
                        }
                        else
                            FetchQuestion();
                    }
                    else
                    {
                        SetLoadingEvent(false);
                        SetErrorMessage("Failed to parse whether player finished");
                    }
                }
                else
                {
                    SetLoadingEvent(false);
                    SetErrorMessage(response.errorMessage);
                }
            }

            class ResponseQuestion
            {
                public string question;
                public string Answer1;
                public string Answer2;
                public string Answer3;
                public string Answer4;
                public string[] answers => new string[4] { Answer1, Answer2, Answer3, Answer4 };
            }
            class Question
            {
                public string question;
                public Answer[] answers;

                public Question(string question, Answer[] answers)
                {
                    this.question = question;
                    this.answers = answers;
                }
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
                public static string[] AnswersToStrings(Answer[] answers)
                {
                    string[] answersText = new string[answers.Length];
                    for (int i = 0; i < answers.Length; i++)
                        answersText[i] = answers[i].answer;
                    return answersText;
                }
                public static Answer[] StringsToAnswers(string[] answersTexts)
                {
                    Answer[] answers = new Answer[answersTexts.Length];
                    for (int i = 0; i < answersTexts.Length; i++)
                    {
                        answers[i].answer = answersTexts[i];
                        answers[i].answerNum = i + 1;
                    }
                    return answers;
                }
            }
        }
        private class ResultsState : StateAtTrivia
        {
            public override void OnEnter()
            {
                uiManager.resultsWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEvent_Register(uiManager.resultsWindow.GetReturnToMainMenuButton, ReturnToMainMenu);
                UpdateGameRoomInformation();
            }

            public override void OnExit()
            {
                uiManager.resultsWindow.mainGameobject.SetActive(false);
                uiManager.ButtonEvent_Unregister(uiManager.resultsWindow.GetReturnToMainMenuButton, ReturnToMainMenu);
            }
            private void UpdateGameRoomInformation()
            {
                SetLoadingEvent(true);
                WebFetch.HttpGet(WebFetch.GetRoomURI(_instance.roomID), UpdateGameRoomResponse);
            }
            void UpdateGameRoomResponse(HttpResponse response)
            {
                if (response.success)
                {
                    HelperFunc.NotImplementedError();
                }
                else
                    SetErrorMessage(response.errorMessage);
                SetLoadingEvent(false);
            }
            public override void SetInputState(bool state)
            {
                uiManager.resultsWindow.GetReturnToMainMenuButton.interactable = state;
            }
            void ReturnToMainMenu() => _instance.GetSetGameState = new LoginState();
        }
    }
}