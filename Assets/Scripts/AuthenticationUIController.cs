using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class AuthenticationUIController : MonoBehaviour
{
    [SerializeField] private string backendBaseUrl = "http://localhost:5000/";
    [SerializeField] private GameObject errorModal;
    [Tooltip("Texto do modal de erro em português.")]
    [SerializeField] private TMP_Text errorModalMessage;
    [Tooltip("Texto do modal de erro em inglês.")]
    [SerializeField] private TMP_Text errorModalMessageEnglish;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject loginLoading;
    [SerializeField] private GameObject registerLoading;
    [SerializeField] private TMP_InputField loginIdentityInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerConfirmPasswordInput;
    [SerializeField] private TMP_Text loginStatusText;
    [SerializeField] private TMP_Text registerStatusText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button openRegisterButton;
    [SerializeField] private Button openLoginButton;
    [SerializeField] private VRVirtualKeyboard virtualKeyboard;

    private AuthenticationManager ownedAuthenticationManager;
    private LoginController loginController;
    private RegisterController registerController;
    private SessionManager sessionManager;
    private ModoJogoManager modeManager;
    private bool referencesReady;
    private bool referencesBound;
    private bool initialized;
    private bool dependenciesInitializationAttempted;
    private bool requestInProgress;
    private bool isOpen;
    private bool previousUiMode;
    private string missingReferenceDescription;

    private UnityAction loginClickHandler;
    private UnityAction registerClickHandler;
    private UnityAction openRegisterClickHandler;
    private UnityAction openLoginClickHandler;

    private void Awake()
    {
        BindUiReferences();
        BindButtonHandlers();
    }

    private async void Start()
    {
        await InitializeAuthenticationAsync();
    }

    private void OnDestroy()
    {
        RemoveButtonHandlers();
        ownedAuthenticationManager?.Dispose();
    }

    public void Initialize(
        LoginController loginController,
        RegisterController registerController,
        SessionManager sessionManager,
        ModoJogoManager modeManager)
    {
        BindUiReferences();
        BindButtonHandlers();

        if (!referencesReady)
        {
            throw new InvalidOperationException(
                "A hierarquia do prefab de autenticação está incompleta: " +
                missingReferenceDescription + ".");
        }

        if (initialized)
        {
            if (ReferenceEquals(this.loginController, loginController) &&
                ReferenceEquals(this.registerController, registerController) &&
                ReferenceEquals(this.sessionManager, sessionManager) &&
                ReferenceEquals(this.modeManager, modeManager))
            {
                return;
            }

            throw new InvalidOperationException(
                "AuthenticationUIController já foi inicializado com outras instâncias.");
        }

        this.loginController = loginController ??
                               throw new ArgumentNullException(nameof(loginController));
        this.registerController = registerController ??
                                  throw new ArgumentNullException(nameof(registerController));
        this.sessionManager = sessionManager ??
                              throw new ArgumentNullException(nameof(sessionManager));
        this.modeManager = modeManager;
        initialized = true;
    }

    public void OpenLogin()
    {
        if (!EnsureOpen())
        {
            return;
        }

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        virtualKeyboard.HideKeyboard();
        ClearStatusMessages();
        HideErrorModal();
    }

    public void OpenRegister()
    {
        if (!EnsureOpen())
        {
            return;
        }

        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        virtualKeyboard.HideKeyboard();
        ClearStatusMessages();
        HideErrorModal();
    }

    public void Close()
    {
        if (virtualKeyboard != null)
        {
            virtualKeyboard.HideKeyboard();
        }

        if (isOpen && modeManager != null)
        {
            modeManager.DefinirModoUI(previousUiMode);
        }

        isOpen = false;
        gameObject.SetActive(false);
    }

    public async Task<AuthenticationResult<bool>> RestoreSessionAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        EnsureInitialized();

        AuthenticationResult<bool> result =
            await sessionManager.RestoreSessionAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            OpenLogin();
            ShowAuthenticationError(
                loginStatusText,
                result.Error == null ? "Não foi possível restaurar a sessão." : result.Error.Message,
                GetEnglishErrorMessage(result.Error));
        }
        else if (result.Value)
        {
            Close();
        }
        else
        {
            OpenLogin();
        }

        return result;
    }

    private async Task InitializeAuthenticationAsync()
    {
        try
        {
            if (!initialized)
            {
                InitializeOwnedDependencies();
            }

            await RestoreSessionAsync();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            if (referencesReady)
            {
                OpenLogin();
                ShowAuthenticationError(
                    loginStatusText,
                    "Não foi possível restaurar a sessão. Faça login novamente.",
                    "The session could not be restored. Please sign in again.");
            }
        }
    }

    private void InitializeOwnedDependencies()
    {
        if (dependenciesInitializationAttempted)
        {
            if (initialized)
            {
                return;
            }

            throw new InvalidOperationException(
                "A inicialização das dependências de autenticação já foi tentada.");
        }

        dependenciesInitializationAttempted = true;
        if (string.IsNullOrWhiteSpace(backendBaseUrl))
        {
            throw new InvalidOperationException(
                "Configure uma URL HTTP ou HTTPS válida no campo Backend Base Url.");
        }

        AuthenticationManager authentication = new AuthenticationManager(backendBaseUrl);
        try
        {
            var session = new SessionManager(
                authentication,
                new WindowsDpapiRefreshTokenStore());
            Initialize(
                new LoginController(session),
                new RegisterController(authentication),
                session,
                modeManager != null ? modeManager : ModoJogoManager.Instance);
            ownedAuthenticationManager = authentication;
        }
        catch
        {
            authentication.Dispose();
            throw;
        }
    }

    public void SetErrorModal(
        GameObject modal,
        TMP_Text portugueseMessageText = null,
        TMP_Text englishMessageText = null)
    {
        errorModal = modal;
        errorModalMessage = portugueseMessageText;
        errorModalMessageEnglish = englishMessageText;
    }

    private void LoginClicked()
    {
        if (requestInProgress)
        {
            return;
        }

        ObserveRequest(LoginAsync(), loginStatusText);
    }

    private async Task LoginAsync()
    {
        if (!EnsureInitializedFromButton())
        {
            return;
        }

        HideErrorModal();
        SetStatus(loginStatusText, "Entrando...");
        SetRequestInProgress(true, loginLoading);

        AuthenticationControllerResult result;
        try
        {
            result = await loginController.LoginAsync(
                loginIdentityInput.text,
                loginPasswordInput.text,
                rememberMeToggle.isOn);
        }
        finally
        {
            SetRequestInProgress(false, loginLoading);
        }

        SetStatus(loginStatusText, result.Message);
        if (!result.IsSuccess || result.IsWarning)
        {
            ShowAuthenticationError(
                loginStatusText,
                result.Message,
                GetEnglishErrorMessage(result.Error));
            return;
        }

        Close();
    }

    private void RegisterClicked()
    {
        if (requestInProgress)
        {
            return;
        }

        ObserveRequest(RegisterAsync(), registerStatusText);
    }

    private async Task RegisterAsync()
    {
        if (!EnsureInitializedFromButton())
        {
            return;
        }

        HideErrorModal();
        if (!string.Equals(
                registerPasswordInput.text,
                registerConfirmPasswordInput.text,
                StringComparison.Ordinal))
        {
            const string mismatchMessage = "As senhas informadas não coincidem.";
            SetStatus(registerStatusText, mismatchMessage);
            ShowAuthenticationError(
                registerStatusText,
                mismatchMessage,
                "The passwords do not match.");
            return;
        }

        SetStatus(registerStatusText, "Cadastrando...");
        SetRequestInProgress(true, registerLoading);

        AuthenticationControllerResult result;
        try
        {
            result = await registerController.RegisterAsync(
                registerEmailInput.text,
                registerUsernameInput.text,
                registerPasswordInput.text);
        }
        finally
        {
            SetRequestInProgress(false, registerLoading);
        }

        SetStatus(registerStatusText, result.Message);
        if (!result.IsSuccess || result.IsWarning)
        {
            ShowAuthenticationError(
                registerStatusText,
                result.Message,
                GetEnglishErrorMessage(result.Error));
        }
    }

    private bool EnsureOpen()
    {
        if (!referencesReady)
        {
            Debug.LogError(
                "Não foi possível abrir a autenticação: a hierarquia do prefab está incompleta.",
                this);
            return false;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!isOpen)
        {
            if (modeManager == null)
            {
                Debug.LogError(
                    "AuthenticationUIController precisa receber o ModoJogoManager para alternar o modo de interação.",
                    this);
            }
            else
            {
                previousUiMode = modeManager.uiMode;
                modeManager.DefinirModoUI(true);
            }

            isOpen = true;
        }

        return true;
    }

    private bool EnsureInitializedFromButton()
    {
        if (EnsureInitialized())
        {
            return true;
        }

        Debug.LogError(
            "AuthenticationUIController ainda não recebeu LoginController, RegisterController e SessionManager.",
            this);
        return false;
    }

    private bool EnsureInitialized()
    {
        return initialized;
    }

    private void ObserveRequest(Task request, TMP_Text statusText)
    {
        request.ContinueWith(
            completed =>
            {
                if (completed.IsFaulted)
                {
                    Exception exception = completed.Exception.Flatten().InnerExceptions[0];
                    Debug.LogException(exception, this);
                    ShowAuthenticationError(
                        statusText,
                        "Ocorreu um erro inesperado. Tente novamente.",
                        "An unexpected error occurred. Please try again.");
                }
                else if (completed.IsCanceled)
                {
                    ShowAuthenticationError(
                        statusText,
                        "A operação foi cancelada.",
                        "The operation was canceled.");
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void BindUiReferences()
    {
        if (referencesBound)
        {
            return;
        }

        referencesBound = true;
        var missingReferences = new List<string>();
        AddMissing(missingReferences, loginPanel, "LoginPanel");
        AddMissing(missingReferences, registerPanel, "RegisterPanel");
        AddMissing(missingReferences, loginLoading, "LoginLoading");
        AddMissing(missingReferences, registerLoading, "RegisterLoading");
        AddMissing(missingReferences, loginIdentityInput, "LoginIdentityInput");
        AddMissing(missingReferences, loginPasswordInput, "LoginPasswordInput");
        AddMissing(missingReferences, rememberMeToggle, "RememberMeToggle");
        AddMissing(missingReferences, registerUsernameInput, "RegisterUsernameInput");
        AddMissing(missingReferences, registerEmailInput, "RegisterEmailInput");
        AddMissing(missingReferences, registerPasswordInput, "RegisterPasswordInput");
        AddMissing(missingReferences, registerConfirmPasswordInput, "RegisterConfirmPasswordInput");
        AddMissing(missingReferences, loginStatusText, "LoginStatusText");
        AddMissing(missingReferences, registerStatusText, "RegisterStatusText");
        AddMissing(missingReferences, loginButton, "LoginButton");
        AddMissing(missingReferences, registerButton, "RegisterButton");
        AddMissing(missingReferences, openRegisterButton, "OpenRegisterButton");
        AddMissing(missingReferences, openLoginButton, "OpenLoginButton");
        AddMissing(missingReferences, virtualKeyboard, "VRVirtualKeyboard");

        referencesReady = missingReferences.Count == 0;
        missingReferenceDescription = string.Join(", ", missingReferences);
        if (!referencesReady)
        {
            Debug.LogError(
                "Atribua estas referências no Inspector do AuthenticationUIController: " +
                missingReferenceDescription + ".",
                this);
            return;
        }

        virtualKeyboard.SetAllowedFields(
            loginIdentityInput,
            loginPasswordInput,
            registerUsernameInput,
            registerEmailInput,
            registerPasswordInput,
            registerConfirmPasswordInput);
    }

    private void BindButtonHandlers()
    {
        if (!referencesReady || loginClickHandler != null)
        {
            return;
        }

        loginClickHandler = LoginClicked;
        registerClickHandler = RegisterClicked;
        openRegisterClickHandler = OpenRegister;
        openLoginClickHandler = OpenLogin;
        loginButton.onClick.AddListener(loginClickHandler);
        registerButton.onClick.AddListener(registerClickHandler);
        openRegisterButton.onClick.AddListener(openRegisterClickHandler);
        openLoginButton.onClick.AddListener(openLoginClickHandler);
    }

    private void RemoveButtonHandlers()
    {
        if (loginButton != null && loginClickHandler != null)
        {
            loginButton.onClick.RemoveListener(loginClickHandler);
        }

        if (registerButton != null && registerClickHandler != null)
        {
            registerButton.onClick.RemoveListener(registerClickHandler);
        }

        if (openRegisterButton != null && openRegisterClickHandler != null)
        {
            openRegisterButton.onClick.RemoveListener(openRegisterClickHandler);
        }

        if (openLoginButton != null && openLoginClickHandler != null)
        {
            openLoginButton.onClick.RemoveListener(openLoginClickHandler);
        }
    }

    private void SetRequestInProgress(bool value, GameObject loadingIndicator)
    {
        requestInProgress = value;
        loadingIndicator.SetActive(value);
        loginButton.interactable = !value;
        registerButton.interactable = !value;
        openRegisterButton.interactable = !value;
        openLoginButton.interactable = !value;
    }

    private void ShowAuthenticationError(
        TMP_Text statusText,
        string portugueseMessage,
        string englishMessage)
    {
        string visiblePortugueseMessage = string.IsNullOrWhiteSpace(portugueseMessage)
            ? "Ocorreu um erro inesperado. Tente novamente."
            : portugueseMessage;
        string visibleEnglishMessage = string.IsNullOrWhiteSpace(englishMessage)
            ? "An unexpected error occurred. Please try again."
            : englishMessage;
        SetStatus(statusText, visiblePortugueseMessage);

        if (errorModal == null)
        {
            Debug.LogWarning(
                "Nenhum modal de erro foi atribuído ao AuthenticationUIController; a mensagem aparece no status do formulário.",
                this);
            return;
        }

        if (errorModalMessage != null)
        {
            errorModalMessage.text = visiblePortugueseMessage;
        }

        if (errorModalMessageEnglish != null)
        {
            errorModalMessageEnglish.text = visibleEnglishMessage;
        }

        errorModal.SetActive(true);
    }

    private static string GetEnglishErrorMessage(AuthenticationError error)
    {
        if (error == null)
        {
            return "An unexpected error occurred. Please try again.";
        }

        switch (error.Type)
        {
            case AuthenticationFailureType.Validation:
                switch (error.Message)
                {
                    case "Informe seu e-mail ou nome de usuário.":
                        return "Please enter your email or username.";
                    case "Informe sua senha.":
                        return "Please enter your password.";
                    case "Informe seu e-mail.":
                        return "Please enter your email.";
                    case "Informe seu nome de usuário.":
                        return "Please enter your username.";
                    case "Informe um e-mail válido.":
                        return "Please enter a valid email address.";
                    case "A senha deve conter pelo menos 8 caracteres.":
                        return "Your password must be at least 8 characters long.";
                    default:
                        return "Please check the information you entered.";
                }
            case AuthenticationFailureType.InvalidCredentials:
                return "The email/username or password is incorrect.";
            case AuthenticationFailureType.InvalidSession:
                return "Your session has expired. Please sign in again.";
            case AuthenticationFailureType.Connection:
                return "Could not connect to the server. Check your connection and try again.";
            case AuthenticationFailureType.Http:
                return error.StatusCode == System.Net.HttpStatusCode.Conflict
                    ? "This email or username is already registered."
                    : "The request could not be completed.";
            case AuthenticationFailureType.Server:
                return "The server is temporarily unavailable. Please try again later.";
            case AuthenticationFailureType.UnexpectedResponse:
                return "The server returned an unexpected response.";
            case AuthenticationFailureType.Storage:
                return "The session could not be securely stored or restored on this device.";
            case AuthenticationFailureType.RegistrationCleanup:
                return "Your account was created, but the temporary session could not be closed. Do not submit the registration again; try signing in.";
            default:
                return "An unexpected error occurred. Please try again.";
        }
    }

    private void HideErrorModal()
    {
        if (errorModal != null)
        {
            errorModal.SetActive(false);
        }
    }

    private void ClearStatusMessages()
    {
        SetStatus(loginStatusText, string.Empty);
        SetStatus(registerStatusText, string.Empty);
    }

    private static void SetStatus(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value ?? string.Empty;
        }
    }

    private static void AddMissing<T>(ICollection<string> missing, T reference, string name)
        where T : UnityEngine.Object
    {
        if (reference == null)
        {
            missing.Add(name);
        }
    }

}
