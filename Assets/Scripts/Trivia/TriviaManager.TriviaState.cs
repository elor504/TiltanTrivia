using System;
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
                    stateAtLogin.Exit();
                stateAtLogin = value;
                if (stateAtLogin != null)
                    stateAtLogin.Enter();
            }
        }
        public override void OnEnter()
        {
            triviaState = this;
            uiManager.playersWindow.mainGameobject.SetActive(true);
            uiManager.playersWindow.SetPlayer2State(false);
            uiManager.playersWindow.ResetPlayerProgress();
            uiManager.playersWindow.GetSetPlayer1Name = _instance.GetSetUsername;
            uiManager.playersWindow.GetSetPlayer2Name = "NOT LOGGED IN";
            GetSetStateAtTrivia = new WaitingRoomState();
        }
        public override void OnExit()
        {
            GetSetStateAtTrivia = null;
            uiManager.playersWindow.mainGameobject.SetActive(false);
        }
        public override void SetErrorMessage(string value)
        {
        }
        public override void SetInputState(bool state) => GetSetStateAtTrivia.SetInputState(state);
        public abstract class StateAtTrivia : State
        {
            protected void SetTriviaState(StateAtTrivia stateAtTrivia) => triviaState.GetSetStateAtTrivia = stateAtTrivia;

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
                _instance.StartCoroutine(WebFetch.HttpGet(
                WebFetch.GetIsPlayer2LoggedInURI(_instance.roomID),
                CheckForOpponentSuccess,
                FailureResponse
                ));
            }
            void CheckForOpponentSuccess(HttpResponse<bool> response)
            {
                Debug.Log("Opponent logged in: " + response.body);
                if (response.body)
                {
                    uiManager.playersWindow.SetPlayer2State(true);
                    UpdateOpponentUsername();
                }
                else
                    _instance.StartCoroutine(CheckForOpponent(updateInterval));
            }
            void UpdateOpponentUsername()
            {
                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(
                WebFetch.GetOpponentUsernameURI(_instance.roomID, _instance.playerID),
                UpdateOpponentUsernameSuccess,
                FailureResponse
                ));
            }
            void UpdateOpponentUsernameSuccess(HttpResponse<string> response)
            {
                response.body = response.body.Replace('"', ' ');
                _instance.opponentUsername = response.body;
                uiManager.playersWindow.GetSetPlayer2Name = response.body;
                SetLoadingEvent(false);
                SetTriviaState(new GameRunningState());
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
                _instance.StartCoroutine(WebFetch.HttpGet<ResponseQuestion>(
                WebFetch.GetQuestionURI(_instance.roomID, _instance.playerID),
                FetchQuestionSuccess,
                FailureResponse
                ));
            }
            private void FetchQuestionSuccess(HttpResponse<ResponseQuestion> response)
            {
                LoadQuestion(response.body);
                SetLoadingEvent(false);

            }
            void LoadQuestion(ResponseQuestion responseQuestion)
            {
                Answer[] answers = Answer.StringsToAnswers(responseQuestion.answers);
                HelperFunc.ArrayShuffle(ref answers);
                GetSetCurrentQuestion = new Question(responseQuestion.Question, answers);
                Debug.Log("Question: " + GetSetCurrentQuestion.question);
            }
            void AnsweredQuestion(int answerButtonNum)
            {
                selectedAnswer = GetSetCurrentQuestion.answers[answerButtonNum - 1].answerNum;
                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(
                WebFetch.GetInsertAnswerURI(_instance.roomID, _instance.playerID, selectedAnswer),
                InsertAnswerSuccess,
                FailureResponse
                ));
            }
            void InsertAnswerSuccess(HttpResponse<int> response)
            {
                uiManager.playersWindow.SetPlayerQuestionRecord(currentQuestionNum, (selectedAnswer == response.body) ? QuestionRecord.Right : QuestionRecord.Wrong);
                currentQuestionNum++;
                CheckIfFinished();
            }


            private void CheckIfFinished()
            {
                _instance.StartCoroutine(WebFetch.HttpGet(
                    WebFetch.GetPlayerFinishedURI(_instance.roomID, _instance.playerID),
                    CheckIfFinishedSuccess,
                    FailureResponse
                    ));
            }
            void CheckIfFinishedSuccess(HttpResponse<bool> response)
            {
                if (response.body)
                {
                    SetLoadingEvent(false);
                    SetTriviaState(new ResultsState());
                }
                else
                    FetchQuestion();
            }


            class ResponseQuestion
            {
                public string Question;
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
            const float updateInterval = 1.5f;
            GameRoomData gameRoom;
            Coroutine gameroomUpdater;
            Coroutine apiRequest;
            bool windowOpen;
            public override void OnEnter()
            {
                uiManager.resultsWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEvent_Register(uiManager.resultsWindow.GetReturnToMainMenuButton, ReturnToMainMenu);
                gameroomUpdater = _instance.StartCoroutine(UpdateGameRoomInformation(0));
                gameRoom = new GameRoomData() { Player1Id = _instance.playerID };
                UpdateResultsUI(true);
                windowOpen = true;
            }

            public override void OnExit()
            {
                windowOpen = false;
                uiManager.resultsWindow.mainGameobject.SetActive(false);
                if (gameroomUpdater != null)
                    _instance.StopCoroutine(gameroomUpdater);
                if (apiRequest != null)
                    _instance.StopCoroutine(apiRequest);
                uiManager.ButtonEvent_Unregister(uiManager.resultsWindow.GetReturnToMainMenuButton, ReturnToMainMenu);
            }
            IEnumerator UpdateGameRoomInformation(float delay)
            {
                if (delay > 0)
                    yield return new WaitForSeconds(delay);
                if (windowOpen)
                {
                    SetLoadingEvent(true);
                    apiRequest = _instance.StartCoroutine(WebFetch.HttpGet<GameRoomData>(
                        WebFetch.GetRoomURI(_instance.roomID),
                        UpdateGameRoomSuccess,
                        FailureResponse
                        ));
                }
            }
            void UpdateGameRoomSuccess(HttpResponse<GameRoomData> response)
            {
                gameRoom = response.body;
                if (gameRoom.Player1Id == _instance.playerID || gameRoom.Player2Id == _instance.playerID)
                {
                    UpdateResultsUI(gameRoom.Player1Id == _instance.playerID);
                }
                else
                {
                    SetErrorMessage("Id was not found in room...");
                    SetLoadingEvent(false);
                }

            }
            void UpdateResultsUI(bool player1)
            {
                uiManager.resultsWindow.GetSetYourTime = "Your Time: " + (player1 ? gameRoom.Player1Time : gameRoom.Player2Time);
                Debug.Log("Score: " + (player1 ? gameRoom.Player1Score : gameRoom.Player2Score).ToString());
                uiManager.resultsWindow.GetSetYourScore = "Your Score: " + (player1 ? gameRoom.Player1Score : gameRoom.Player2Score) + "/16";
                if ((player1 && gameRoom.CurPlayer2Q == 16) || (!player1 && gameRoom.CurPlayer1Q == 16))
                {
                    uiManager.resultsWindow.GetSetOpponentText = "Opponent Time: " + (player1 ? gameRoom.Player2Time : gameRoom.Player1Time);
                    uiManager.resultsWindow.GetSetOpponentScore = "Answered: " + (player1 ? gameRoom.Player2Score : gameRoom.Player1Score) + "/16";
                    if ((player1 && (gameRoom.Player1Score > gameRoom.Player2Score || (gameRoom.Player1Score == gameRoom.Player2Score && gameRoom.Player1Time < gameRoom.Player2Score)))
                        || (!player1 && (gameRoom.Player2Score > gameRoom.Player1Score || (gameRoom.Player2Score == gameRoom.Player1Score && gameRoom.Player2Time < gameRoom.Player1Score))))
                        uiManager.resultsWindow.SetResult(ResultWindowState.Won);
                    else
                        uiManager.resultsWindow.SetResult(ResultWindowState.Lost);
                    SetLoadingEvent(false);
                }
                else
                {
                    uiManager.resultsWindow.GetSetOpponentText = "Opponent Time: ";
                    uiManager.resultsWindow.GetSetOpponentScore = "Answered: ";
                    uiManager.resultsWindow.SetResult(ResultWindowState.Waiting);
                    if (windowOpen)
                        gameroomUpdater = _instance.StartCoroutine(UpdateGameRoomInformation(updateInterval));
                    SetLoadingEvent(false);
                }
            }
            public override void SetInputState(bool state) => uiManager.resultsWindow.GetReturnToMainMenuButton.interactable = state;
            void ReturnToMainMenu() => _instance.GetSetGameState = new LoginState();
            public class GameRoomData
            {
                public int GameRoomId;
                public string RoomPassword;
                public int Player1Id;
                public int Player2Id;
                public int Player1Score;
                public int Player2Score;
                public float Player1Time;
                public float Player2Time;
                public DateTime GameStartTime;
                public int CurPlayer1Q;
                public int CurPlayer2Q;
            }
        }
    }
}