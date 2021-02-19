using UnityEngine;
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
            _instance.setLoadingEvent += SetInputState;
        }

        public override void OnExit() {
            _instance.setLoadingEvent -= SetInputState;
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
            public override void OnEnter() {
                uiManager.OpenSignupUI();
                uiManager.InputFieldEvent_Register(uiManager.signupWindow.GetInput, Signup);
            }

            public override void OnExit() {
                uiManager.InputFieldEvent_Unregister(uiManager.signupWindow.GetInput, Signup);
                uiManager.CloseSignupUI();
            }
            public override void SetInputState(bool value) => uiManager.signupWindow.SetInputState(value);
            private void Signup(string value) {
                _instance.GetSetUsername = value;
                ExitToMainWindow();
                HelperFunc.NotImplementedError();
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