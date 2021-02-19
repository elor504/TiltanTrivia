using UnityEngine;
using System;
public partial class TriviaManager
{

    class LoginState : GameState
    {
        public static LoginState loginState;
        StateAtLogin stateAtLogin;
        public StateAtLogin GetSetStateAtLogin {
            get => stateAtLogin;
            set {
                if (stateAtLogin != null)
                    stateAtLogin.OnExit();
                stateAtLogin = value;
                stateAtLogin.OnEnter();
            }
        }
        public override void OnEnter() {
            loginState = this;
            uiManager.SetMainLoginWindow(true);
            GetSetStateAtLogin = new SignupState();
            _instance.SetLoadingEvent += SetInputState;
        }

        public override void OnExit() {
            _instance.SetLoadingEvent -= SetInputState;
            stateAtLogin.OnExit();
            uiManager.SetMainLoginWindow(false);
        }
        public override void SetErrorMessage(string value) => uiManager.SetErrorMessage(value);

        public override void SetInputState(bool value) => stateAtLogin.SetInputState(value);
        //States
        public abstract class StateAtLogin : State
        {
            protected void SetLoginState(StateAtLogin stateAtLogin) => loginState.GetSetStateAtLogin = stateAtLogin;
            protected void ExitToMainWindow() => SetLoginState(new MainLoginWindowState());
            protected void SetLoadingEvent(bool state) => _instance.SetLoadingEvent(state);
            protected void SetErrorMessage(string error) => TriviaUIManager._instance.SetErrorMessage(error);
        }
        class MainLoginWindowState : StateAtLogin
        {
            public override void OnEnter() {
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetFindMatchButton, MatchWithOpponent);
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetCreateRoomButton, CreateRoomWindow);
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetJoinRoomButton, JoinRoomWindow);
            }
            public override void OnExit() {
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetFindMatchButton, MatchWithOpponent);
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetCreateRoomButton, CreateRoomWindow);
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetJoinRoomButton, JoinRoomWindow);
            }
            private void CreateRoomWindow() => SetLoginState(new CreateRoomState());
            private void JoinRoomWindow() => SetLoginState(new JoinRoomState());
            public override void SetInputState(bool value) => uiManager.mainLoginWindow.SetInputState(value);
            private void MatchWithOpponent() {
                _instance.GetSetGameState = new TriviaState();
                HelperFunc.NotImplementedError();
            }

        }
        class SignupState : StateAtLogin
        {
            string username;
            public override void OnEnter() {
                uiManager.OpenSignupUI();
                uiManager.InputFieldEvent_Register(uiManager.signupWindow.GetInput, Signup);
            }
            public override void OnExit() {
                uiManager.InputFieldEvent_Unregister(uiManager.signupWindow.GetInput, Signup);
                uiManager.CloseSignupUI();
            }
            public override void SetInputState(bool value) => uiManager.signupWindow.SetInputState(value);
            private void Signup(string username) {
                this.username = username;
                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(WebFetch.SignupURI(username), SignUpResponse));
            }
            private void SignUpResponse(bool success, string json, string errorMessage)
            {
                if (success)
                {
                    _instance.GetSetUsername = username;
                    Response response = JsonUtility.FromJson<Response>(json);
                    _instance.playerID = response.playerID;
                }
                else
                    SetErrorMessage(errorMessage);
                Debug.Log(_instance.playerID + " , " + _instance.GetSetUsername);
                SetLoadingEvent(false);
            }
            [Serializable]
            class Response
            {
                public int playerID;
            }
        }
        class CreateRoomState : StateAtLogin
        {
            public override void OnEnter() {
                uiManager.createRoomWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEventRegister(uiManager.createRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEventRegister(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
            }
            public override void OnExit() {
                uiManager.createRoomWindow.mainGameobject.SetActive(false);
                uiManager.ButtonEventUnregister(uiManager.createRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEventUnregister(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
            }
            public override void SetInputState(bool value) => uiManager.createRoomWindow.SetInputState(value);
            private void CreateRoom() {
                HelperFunc.NotImplementedError();
            }
        }
        class JoinRoomState : StateAtLogin
        {
            public override void OnEnter() {
                uiManager.joinRoomWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEventRegister(uiManager.joinRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEventRegister(uiManager.joinRoomWindow.GetConfirmButton, JoinRoom);
            }
            public override void OnExit() {
                uiManager.joinRoomWindow.mainGameobject.SetActive(false);
                uiManager.ButtonEventUnregister(uiManager.joinRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEventUnregister(uiManager.joinRoomWindow.GetConfirmButton, JoinRoom);
            }
            public override void SetInputState(bool value) => uiManager.joinRoomWindow.SetInputState(value);
            private void JoinRoom() {
                HelperFunc.NotImplementedError();
            }
        }

    }
}