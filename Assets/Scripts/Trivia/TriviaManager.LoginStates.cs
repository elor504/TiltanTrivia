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

        public override void Update() {
            stateAtLogin.Update();
        }
        //States
        public abstract class StateAtLogin : State
        {
            protected void SetLoginState(StateAtLogin stateAtLogin) => loginState.GetSetStateAtLogin = stateAtLogin;
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
            public override void SetInputState(bool value) => uiManager.SetSignupInputState(value);

            public override void Update() { }
            private void Signup(string value) => _instance.GetSetUsername = value;
        }
        class MainLoginWindowState : StateAtLogin
        {
            public override void OnEnter() {
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetFindMatchButton, MatchWithOpponent);
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetCreateRoomButton, () => { SetLoginState(new CreateRoomState()); });
                uiManager.ButtonEventRegister(uiManager.mainLoginWindow.GetJoinRoomButton, () => { SetLoginState(new JoinRoomState()); });
            }

            public override void OnExit() {
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetFindMatchButton, MatchWithOpponent);
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetCreateRoomButton, () => { SetLoginState(new CreateRoomState()); });
                uiManager.ButtonEventUnregister(uiManager.mainLoginWindow.GetJoinRoomButton, () => { SetLoginState(new JoinRoomState()); });
            }
            public override void SetInputState(bool value) => uiManager.SetSignupInputState(value);

            public override void Update() { }
            private void MatchWithOpponent() { Debug.Log("Matching!"); }

        }
        class CreateRoomState : StateAtLogin
        {
            public override void OnEnter() {
                uiManager.ButtonEventRegister(uiManager.createRoomWindow.GetExitButton, () => { SetLoginState(new MainLoginWindowState()); });
                uiManager.ButtonEventRegister(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
            }
            public override void OnExit() {
                uiManager.ButtonEventUnregister(uiManager.createRoomWindow.GetExitButton, () => { SetLoginState(new MainLoginWindowState()); });
                uiManager.ButtonEventUnregister(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
            }
            public override void SetInputState(bool state) {
                uiManager.createRoomWindow.GetConfirmButton.interactable = state;
                uiManager.createRoomWindow.GetExitButton.interactable = state;
                uiManager.createRoomWindow.GetInput.interactable = state;
            }
            public override void Update() {
            }
            private void CreateRoom() { }
        }
        class JoinRoomState : StateAtLogin
        {
            public override void OnEnter() {
                throw new System.NotImplementedException();
            }

            public override void OnExit() {
                throw new System.NotImplementedException();
            }

            public override void SetInputState(bool state) {
                throw new System.NotImplementedException();
            }

            public override void Update() {
                throw new System.NotImplementedException();
            }
        }

    }
}