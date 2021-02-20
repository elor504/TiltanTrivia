public partial class TriviaManager
{

    class LoginState : GameState
    {
        public static LoginState loginState;
        StateAtLogin stateAtLogin;
        public StateAtLogin GetSetStateAtLogin
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
            loginState = this;
            uiManager.SetMainLoginWindow(true);
            if (_instance.playerID == 0 || _instance.username == "")
                GetSetStateAtLogin = new SignupState();
            else
                GetSetStateAtLogin = new MainLoginWindowState();
            _instance.SetLoadingEvent += SetInputState;

        }

        public override void OnExit()
        {
            GetSetStateAtLogin = null;
            _instance.SetLoadingEvent -= SetInputState;
            uiManager.SetMainLoginWindow(false);
        }
        public override void SetErrorMessage(string value) => uiManager.SetErrorMessage(value);

        public override void SetInputState(bool value) => GetSetStateAtLogin.SetInputState(value);
        //States
        public abstract class StateAtLogin : State
        {
            protected void SetLoginState(StateAtLogin stateAtLogin) => loginState.GetSetStateAtLogin = stateAtLogin;
            protected void ExitToMainWindow() => SetLoginState(new MainLoginWindowState());
        }
        class MainLoginWindowState : StateAtLogin
        {
            public override void OnEnter()
            {
                uiManager.ButtonEvent_Register(uiManager.mainLoginWindow.GetFindMatchButton, FindMatch);
                uiManager.ButtonEvent_Register(uiManager.mainLoginWindow.GetCreateRoomButton, CreateRoomWindow);
                uiManager.ButtonEvent_Register(uiManager.mainLoginWindow.GetJoinRoomButton, JoinRoomWindow);
            }
            public override void OnExit()
            {
                uiManager.ButtonEvent_Unregister(uiManager.mainLoginWindow.GetFindMatchButton, FindMatch);
                uiManager.ButtonEvent_Unregister(uiManager.mainLoginWindow.GetCreateRoomButton, CreateRoomWindow);
                uiManager.ButtonEvent_Unregister(uiManager.mainLoginWindow.GetJoinRoomButton, JoinRoomWindow);
            }
            private void CreateRoomWindow() => SetLoginState(new CreateRoomState());
            private void JoinRoomWindow() => SetLoginState(new JoinRoomState());
            public override void SetInputState(bool value) => uiManager.mainLoginWindow.SetInputState(value);
            private void FindMatch()
            {

                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(
                    WebFetch.FindMatchURI(_instance.playerID),
                    FindMatchSuccess,
                    FailureResponse
                    ));
            }
            private void FindMatchSuccess(HttpResponse<int> response)
            {
                _instance.roomID = response.body;
                SetLoadingEvent(false);
                _instance.GetSetGameState = new TriviaState();
            }



        }
        class SignupState : StateAtLogin
        {
            string username;
            public override void OnEnter()
            {
                uiManager.SetSignupWindow(true);
                uiManager.InputFieldEvent_Register(uiManager.signupWindow.GetInput, Signup);
            }
            public override void OnExit()
            {
                uiManager.InputFieldEvent_Unregister(uiManager.signupWindow.GetInput, Signup);
                uiManager.SetSignupWindow(false);
            }
            public override void SetInputState(bool value) => uiManager.signupWindow.SetInputState(value);
            private void Signup(string username)
            {
                this.username = username;
                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(
                WebFetch.SignupURI(username),
                SignUpSuccess,
                FailureResponse
                ));
            }
            private void SignUpSuccess(HttpResponse<int> response)
            {
                _instance.GetSetUsername = username;
                _instance.playerID = response.body;
                ExitToMainWindow();
                SetLoadingEvent(false);
            }
        }
        class CreateRoomState : StateAtLogin
        {
            string password;
            public override void OnEnter()
            {
                uiManager.createRoomWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEvent_Register(uiManager.createRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEvent_Register(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
                uiManager.InputFieldEvent_Register(uiManager.createRoomWindow.GetInput, SetPassword);
            }
            public override void OnExit()
            {
                uiManager.createRoomWindow.mainGameobject.SetActive(false);
                uiManager.InputFieldEvent_Unregister(uiManager.createRoomWindow.GetInput, SetPassword);
                uiManager.ButtonEvent_Unregister(uiManager.createRoomWindow.GetConfirmButton, CreateRoom);
                uiManager.ButtonEvent_Unregister(uiManager.createRoomWindow.GetExitButton, ExitToMainWindow);
            }
            public void SetPassword(string value) => password = value;
            public override void SetInputState(bool value) => uiManager.createRoomWindow.SetInputState(value);
            private void CreateRoom()
            {
                if (_instance.roomPassword != null)
                {
                    SetLoadingEvent(true);
                    _instance.StartCoroutine(WebFetch.HttpGet(
                    WebFetch.CreateRooomURI(_instance.playerID, password),
                    CreateRoomSuccess,
                    FailureResponse
                    ));
                }
                else
                    SetErrorMessage("Please fill the password field.");
            }
            private void CreateRoomSuccess(HttpResponse<bool> response)
            {
                if (response.body)
                {
                    _instance.roomPassword = password;
                    UpdateRoomId();
                }
                else
                {
                    if (response.errorMessage == "")
                        response.errorMessage = "Server error";
                    FailureResponse(response);
                }
            }
            private void UpdateRoomId()
            {
                SetLoadingEvent(true);
                _instance.StartCoroutine(WebFetch.HttpGet(
                    WebFetch.GetRoomIdURI(_instance.playerID),
                    GetRoomIdResponse,
                    FailureResponse
                    ));
            }
            private void GetRoomIdResponse(HttpResponse<int> response)
            {
                _instance.roomID = response.body;
                _instance.GetSetGameState = new TriviaState();
                SetLoadingEvent(false);
            }

        }
        class JoinRoomState : StateAtLogin
        {
            string password;
            int roomID;
            public override void OnEnter()
            {
                uiManager.joinRoomWindow.mainGameobject.SetActive(true);
                uiManager.ButtonEvent_Register(uiManager.joinRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEvent_Register(uiManager.joinRoomWindow.GetConfirmButton, JoinRoom);
                uiManager.InputFieldEvent_Register(uiManager.joinRoomWindow.GetPwInput, SetPassword);
                uiManager.InputFieldEvent_Register(uiManager.joinRoomWindow.GetIdInput, SetRoomID);
            }
            public override void OnExit()
            {
                uiManager.joinRoomWindow.mainGameobject.SetActive(false);
                uiManager.InputFieldEvent_Unregister(uiManager.joinRoomWindow.GetPwInput, SetPassword);
                uiManager.InputFieldEvent_Unregister(uiManager.joinRoomWindow.GetIdInput, SetRoomID);
                uiManager.ButtonEvent_Unregister(uiManager.joinRoomWindow.GetExitButton, ExitToMainWindow);
                uiManager.ButtonEvent_Unregister(uiManager.joinRoomWindow.GetConfirmButton, JoinRoom);
            }
            public void SetPassword(string value) => password = value;
            public void SetRoomID(string value)
            {
                if (int.TryParse(value, out int roomID))
                    this.roomID = roomID;
                else
                    SetErrorMessage("Room ID must contain only numbers.");
            }
            public override void SetInputState(bool value) => uiManager.joinRoomWindow.SetInputState(value);
            private void JoinRoom()
            {
                if (password != null && roomID != 0)
                {
                    SetLoadingEvent(true);
                    _instance.StartCoroutine(WebFetch.HttpGet(
                    WebFetch.JoinRooomURI(_instance.playerID, roomID, password),
                    JoinRoomSuccess,
                    FailureResponse
                    ));
                }
                else
                {
                    SetErrorMessage("Password or/and room ID not valid.");
                    SetLoadingEvent(false);
                }

            }
            private void JoinRoomSuccess(HttpResponse<bool> response)
            {
                if (response.body)
                {
                    _instance.roomPassword = password;
                    _instance.roomID = roomID;
                    SetLoadingEvent(false);
                    _instance.GetSetGameState = new TriviaState();
                }
                else
                {
                    if (response.errorMessage == "")
                        response.errorMessage = "Server error!";
                    FailureResponse(response);
                }


            }
        }

    }
}