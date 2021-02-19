using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class TriviaManager
{
    class TriviaState : GameState
    {
        Question currentQuestion;
        Question GetSetCurrentQuestion {
            get => currentQuestion;
            set {

            }
        }
        public override void OnEnter() {
            _instance.StartCoroutine(StartPlayer2UpdateCoroutine());
            LoadQuestion();
        }

        private void LoadQuestion() {
            currentQuestion = new Question() {
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

        public override void OnExit() { 

        }

        public override void SetErrorMessage(string value) {
        }

        public override void SetInputState(bool state) {
        }
        IEnumerator StartPlayer2UpdateCoroutine() {
            while (true) {
                yield return new WaitForSecondsRealtime(5);
                UpdateOpponentQuestion();
            }
        }
        void UpdateOpponentQuestion() => HelperFunc.NotImplementedError();
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
            int answerNum;
            string answer;

            public Answer(int answerNum, string answer) {
                this.answerNum = answerNum;
                this.answer = answer;
            }
        } 
    }

}