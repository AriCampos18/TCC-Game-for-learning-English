using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class AuthenticationManagerTests
{
    [Test]
    public async Task LoginReturnsTokensAndUsesConfiguredBackendUrl()
    {
        string capturedUri = null;
        HttpMethod capturedMethod = null;
        string capturedBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            capturedUri = request.RequestUri.AbsoluteUri;
            capturedMethod = request.Method;
            capturedBody = await request.Content.ReadAsStringAsync();
            return JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900));
        });

        using (var manager = new AuthenticationManager("https://example.invalid/custom/", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.LoginAsync("test-user", "test-password");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.AccessToken, Is.EqualTo("access-1"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refresh-1"));
            Assert.That(result.Value.ExpiresIn, Is.EqualTo(900));
            Assert.That(result.Value.User.Id, Is.EqualTo(42));
            Assert.That(capturedMethod, Is.EqualTo(HttpMethod.Post));
            Assert.That(capturedUri,
                Is.EqualTo("https://example.invalid/custom/api/auth/login"));

            JObject requestBody = JObject.Parse(capturedBody);
            Assert.That(requestBody.Value<string>("email_or_username"), Is.EqualTo("test-user"));
            Assert.That(requestBody.Value<string>("password"), Is.EqualTo("test-password"));
        }
    }

    [Test]
    public async Task CurrentUserUsesBearerTokenAndParsesBackendUserId()
    {
        string capturedPath = null;
        HttpMethod capturedMethod = null;
        string capturedAuthorization = null;
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
        {
            capturedPath = request.RequestUri.AbsolutePath;
            capturedMethod = request.Method;
            capturedAuthorization = request.Headers.Authorization == null
                ? null
                : request.Headers.Authorization.ToString();
            return Task.FromResult(JsonResponse(HttpStatusCode.OK, CurrentUserResponse(73)));
        });

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticatedUser> result =
                await manager.GetCurrentUserAsync("opaque-token-containing-no-user-id");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Id, Is.EqualTo(73));
            Assert.That(capturedPath, Is.EqualTo("/api/users/me"));
            Assert.That(capturedMethod, Is.EqualTo(HttpMethod.Get));
            Assert.That(capturedAuthorization, Is.EqualTo("Bearer opaque-token-containing-no-user-id"));
        }
    }

    [Test]
    public async Task LoginRequiresBackendUserAndDoesNotInferIdFromToken()
    {
        const string response =
            "{\"data\":{\"access_token\":\"user-id-999-token\",\"refresh_token\":\"refresh-1\",\"expires_in\":900}}";
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, response)));

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.LoginAsync("test-user", "test-password");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.UnexpectedResponse));
        }
    }

    public sealed class VirtualKeyboardTests
    {
        private GameObject root;

        [TearDown]
        public void TearDown()
        {
            if (root != null)
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void KeyboardEditsOnlyConfiguredAuthenticationFields()
        {
            root = new GameObject("AuthenticationKeyboardTest", typeof(RectTransform));
            GameObject panel = new GameObject("VirtualKeyboardPanel", typeof(RectTransform));
            panel.transform.SetParent(root.transform, false);
            panel.SetActive(false);

            TMP_InputField allowedField = CreateInputField("AllowedField");
            allowedField.transform.SetParent(root.transform, false);

            TMP_InputField unrelatedField = CreateInputField("ExerciseField");
            unrelatedField.transform.SetParent(root.transform, false);
            unrelatedField.text = "exercise";

            VRVirtualKeyboard keyboard = root.AddComponent<VRVirtualKeyboard>();
            var serializedKeyboard = new SerializedObject(keyboard);
            serializedKeyboard.FindProperty("keyboardPanel").objectReferenceValue = panel;
            serializedKeyboard.ApplyModifiedPropertiesWithoutUndo();
            keyboard.SetAllowedFields(allowedField);
            Assert.That(keyboard.FocusField(unrelatedField), Is.False);
            Assert.That(keyboard.FocusField(allowedField), Is.True);
            Assert.That(panel.activeSelf, Is.True);

            keyboard.PressKey("b");
            Assert.That(allowedField.text, Is.EqualTo("b"));

            keyboard.PressKey("SHIFT");
            keyboard.PressKey("z");
            Assert.That(allowedField.text, Is.EqualTo("Zb"));

            keyboard.PressKey("SPACE");
            Assert.That(allowedField.text, Is.EqualTo(" Zb"));

            keyboard.PressKey("CLEAR");
            Assert.That(allowedField.text, Is.Empty);

            keyboard.PressKey("DONE");
            Assert.That(panel.activeSelf, Is.False);
            Assert.That(keyboard.FocusField(unrelatedField), Is.False);
            keyboard.PressKey("x");
            Assert.That(unrelatedField.text, Is.EqualTo("exercise"));
        }

        private static TMP_InputField CreateInputField(string name)
        {
            GameObject inputObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(TMP_InputField));
            TMP_InputField input = inputObject.GetComponent<TMP_InputField>();
            input.targetGraphic = inputObject.GetComponent<Image>();

            GameObject textObject = new GameObject(
                "Text",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            textObject.transform.SetParent(inputObject.transform, false);
            input.textViewport = (RectTransform)inputObject.transform;
            input.textComponent = textObject.GetComponent<TextMeshProUGUI>();
            input.lineType = TMP_InputField.LineType.SingleLine;
            return input;
        }
    }

    [Test]
    public async Task RegisterPostsAccountAndRevokesTemporaryRegistrationSession()
    {
        int requestIndex = 0;
        string registrationBody = null;
        string logoutBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            requestIndex++;
            if (requestIndex == 1)
            {
                Assert.That(request.RequestUri.AbsolutePath, Is.EqualTo("/api/auth/register"));
                registrationBody = await request.Content.ReadAsStringAsync();
                return JsonResponse(HttpStatusCode.Created, TokenResponse("registration-access", "registration-refresh", 900));
            }

            Assert.That(request.RequestUri.AbsolutePath, Is.EqualTo("/api/auth/logout"));
            logoutBody = await request.Content.ReadAsStringAsync();
            return JsonResponse(HttpStatusCode.OK, "{\"data\":{\"message\":\"Sessão encerrada.\"}}");
        });

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<bool> result =
                await manager.RegisterAsync("new@example.invalid", "new-user", "dummy-password");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(requestIndex, Is.EqualTo(2));

            JObject createBody = JObject.Parse(registrationBody);
            Assert.That(createBody.Value<string>("email"), Is.EqualTo("new@example.invalid"));
            Assert.That(createBody.Value<string>("username"), Is.EqualTo("new-user"));
            Assert.That(createBody.Value<string>("password"), Is.EqualTo("dummy-password"));

            JObject revokeBody = JObject.Parse(logoutBody);
            Assert.That(revokeBody.Count, Is.EqualTo(1));
            Assert.That(revokeBody.Value<string>("refresh_token"), Is.EqualTo("registration-refresh"));
        }
    }

    [Test]
    public async Task LoginControllerDelegatesAndHonorsRememberMe()
    {
        var store = new ControllerRefreshTokenStore();
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900))));

        using (var authentication = new AuthenticationManager("https://example.invalid", handler))
        {
            var session = new SessionManager(authentication, store);
            var controller = new LoginController(session);
            AuthenticationControllerResult result =
                await controller.LoginAsync("  test-user  ", "dummy-password", true);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsWarning, Is.False);
            Assert.That(session.AccessToken, Is.EqualTo("access-1"));
            Assert.That(session.AuthenticatedUserId, Is.EqualTo(42));
            Assert.That(session.RememberMe, Is.True);
            Assert.That(store.Token, Is.EqualTo("refresh-1"));
        }
    }

    [Test]
    public async Task LoginControllerValidatesRequiredFieldsBeforeCallingBackend()
    {
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            throw new AssertionException("Backend must not be called for empty fields."));

        using (var authentication = new AuthenticationManager("https://example.invalid", handler))
        {
            var session = new SessionManager(
                authentication,
                new ControllerRefreshTokenStore());
            var controller = new LoginController(session);

            AuthenticationControllerResult missingEmail =
                await controller.LoginAsync(" ", "dummy-password", false);
            AuthenticationControllerResult missingPassword =
                await controller.LoginAsync("test-user", "", false);

            Assert.That(missingEmail.IsSuccess, Is.False);
            Assert.That(missingEmail.Error.Type, Is.EqualTo(AuthenticationFailureType.Validation));
            Assert.That(missingPassword.IsSuccess, Is.False);
            Assert.That(missingPassword.Error.Type, Is.EqualTo(AuthenticationFailureType.Validation));
            Assert.That(handler.RequestCount, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task RegisterControllerValidatesFieldsWithoutCallingBackend()
    {
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            throw new AssertionException("Backend must not be called for invalid registration fields."));

        using (var authentication = new AuthenticationManager("https://example.invalid", handler))
        {
            var controller = new RegisterController(authentication);
            AuthenticationControllerResult missingEmail =
                await controller.RegisterAsync("", "test-user", "dummy-password");
            AuthenticationControllerResult malformedEmail =
                await controller.RegisterAsync("not-an-email", "test-user", "dummy-password");
            AuthenticationControllerResult shortPassword =
                await controller.RegisterAsync("test@example.invalid", "test-user", "short");

            Assert.That(missingEmail.Error.Type, Is.EqualTo(AuthenticationFailureType.Validation));
            Assert.That(malformedEmail.Error.Type, Is.EqualTo(AuthenticationFailureType.Validation));
            Assert.That(shortPassword.Error.Type, Is.EqualTo(AuthenticationFailureType.Validation));
            Assert.That(handler.RequestCount, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task RegisterControllerReportsSuccessWithoutStartingSession()
    {
        int requestIndex = 0;
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
        {
            requestIndex++;
            return Task.FromResult(requestIndex == 1
                ? JsonResponse(HttpStatusCode.Created, TokenResponse("registration-access", "registration-refresh", 900))
                : JsonResponse(HttpStatusCode.OK, "{\"data\":{\"message\":\"Sessão encerrada.\"}}"));
        });

        using (var authentication = new AuthenticationManager("https://example.invalid", handler))
        {
            var controller = new RegisterController(authentication);
            AuthenticationControllerResult result =
                await controller.RegisterAsync("new@example.invalid", "new-user", "dummy-password");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsWarning, Is.False);
            Assert.That(result.Message, Does.Contain("Você já pode entrar"));
            Assert.That(handler.RequestCount, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task RegisterControllerWarnsWhenTemporarySessionCleanupFails()
    {
        int requestIndex = 0;
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
        {
            requestIndex++;
            return Task.FromResult(requestIndex == 1
                ? JsonResponse(HttpStatusCode.Created, TokenResponse("registration-access", "registration-refresh", 900))
                : JsonResponse(HttpStatusCode.ServiceUnavailable, "{\"error\":{\"code\":\"unavailable\",\"message\":\"Indisponível.\"}}"));
        });

        using (var authentication = new AuthenticationManager("https://example.invalid", handler))
        {
            var controller = new RegisterController(authentication);
            AuthenticationControllerResult result =
                await controller.RegisterAsync("new@example.invalid", "new-user", "dummy-password");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsWarning, Is.True);
            Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.RegistrationCleanup));
            Assert.That(result.Message, Does.Contain("Não envie o cadastro novamente"));
        }
    }

    [Test]
    public void AuthenticationUIRejectsMismatchedPasswordsBeforeCallingBackend()
    {
        const string prefabPath = "Assets/Prefabs/Authentication/AuthenticationUI.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.That(prefab, Is.Not.Null);

        GameObject uiObject = UnityEngine.Object.Instantiate(prefab);
        var errorModal = new GameObject("AuthenticationErrorModal");
        var portugueseErrorText = new GameObject("PortugueseErrorText").AddComponent<TextMeshProUGUI>();
        var englishErrorText = new GameObject("EnglishErrorText").AddComponent<TextMeshProUGUI>();
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            throw new AssertionException("Backend must not be called when passwords do not match."));

        try
        {
            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var store = new ControllerRefreshTokenStore();
                var session = new SessionManager(authentication, store);
                var ui = uiObject.GetComponent<AuthenticationUIController>();
                Assert.That(ui, Is.Not.Null);

                ui.SetErrorModal(errorModal, portugueseErrorText, englishErrorText);
                ui.Initialize(
                    new LoginController(session),
                    new RegisterController(authentication),
                    session,
                    null);

                TMP_InputField password = FindInputField(uiObject, "RegisterPasswordInput");
                TMP_InputField confirmation = FindInputField(uiObject, "RegisterConfirmPasswordInput");
                password.text = "dummy-password";
                confirmation.text = "different-password";

                FindButton(uiObject, "RegisterButton").onClick.Invoke();

                Assert.That(errorModal.activeSelf, Is.True);
                Assert.That(
                    portugueseErrorText.text,
                    Is.EqualTo("As senhas informadas não coincidem."));
                Assert.That(
                    englishErrorText.text,
                    Is.EqualTo("The passwords do not match."));
                Assert.That(
                    FindText(uiObject, "RegisterStatusText").text,
                    Is.EqualTo("As senhas informadas não coincidem."));
                Assert.That(handler.RequestCount, Is.EqualTo(0));
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(uiObject);
            UnityEngine.Object.DestroyImmediate(errorModal);
            UnityEngine.Object.DestroyImmediate(portugueseErrorText.gameObject);
            UnityEngine.Object.DestroyImmediate(englishErrorText.gameObject);
        }
    }

    [Test]
    public void AuthenticationUIInitializesDependenciesOnlyOnceAcrossActivationChanges()
    {
        const string prefabPath = "Assets/Prefabs/Authentication/AuthenticationUI.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.That(prefab, Is.Not.Null);

        GameObject uiObject = UnityEngine.Object.Instantiate(prefab);
        try
        {
            AuthenticationUIController ui = uiObject.GetComponent<AuthenticationUIController>();
            Assert.That(ui, Is.Not.Null);
            Assert.That(
                ReadPrivateField<string>(ui, "backendBaseUrl"),
                Is.EqualTo("http://localhost:5000/"));

            InvokePrivateMethod(ui, "InitializeOwnedDependencies");
            AuthenticationManager authentication =
                ReadPrivateField<AuthenticationManager>(ui, "ownedAuthenticationManager");
            SessionManager session = ReadPrivateField<SessionManager>(ui, "sessionManager");
            LoginController login = ReadPrivateField<LoginController>(ui, "loginController");
            RegisterController register = ReadPrivateField<RegisterController>(ui, "registerController");

            Assert.That(authentication, Is.Not.Null);
            Assert.That(session, Is.Not.Null);
            Assert.That(login, Is.Not.Null);
            Assert.That(register, Is.Not.Null);

            uiObject.SetActive(false);
            uiObject.SetActive(true);
            InvokePrivateMethod(ui, "InitializeOwnedDependencies");

            Assert.That(
                ReadPrivateField<AuthenticationManager>(ui, "ownedAuthenticationManager"),
                Is.SameAs(authentication));
            Assert.That(ReadPrivateField<SessionManager>(ui, "sessionManager"), Is.SameAs(session));
            Assert.That(ReadPrivateField<LoginController>(ui, "loginController"), Is.SameAs(login));
            Assert.That(ReadPrivateField<RegisterController>(ui, "registerController"), Is.SameAs(register));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(uiObject);
        }
    }

    public sealed class SessionManagerTests
    {
        [Test]
        public async Task RememberedLoginPersistsOnlyRefreshToken()
        {
            var store = new MemoryRefreshTokenStore { Token = "previous-token" };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
                Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900))));

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> result =
                    await session.LoginAsync("test-user", "test-password", true);

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(session.AccessToken, Is.EqualTo("access-1"));
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(42));
                Assert.That(session.RememberMe, Is.True);
                Assert.That(store.Token, Is.EqualTo("refresh-1"));
                Assert.That(store.SaveCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task LoginWithoutRememberDoesNotPersistRefreshToken()
        {
            var store = new MemoryRefreshTokenStore { Token = "previous-token" };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
                Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900))));

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> result =
                    await session.LoginAsync("test-user", "test-password", false);

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(session.AccessToken, Is.EqualTo("access-1"));
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(42));
                Assert.That(session.RememberMe, Is.False);
                Assert.That(store.Token, Is.Null);
                Assert.That(store.SaveCount, Is.EqualTo(0));
                Assert.That(store.ClearCount, Is.GreaterThan(0));
            }
        }

        [Test]
        public async Task RestoreSessionRotatesAndPersistsRefreshToken()
        {
            var store = new MemoryRefreshTokenStore { Token = "saved-refresh" };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath == "/api/auth/refresh")
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-new", "refresh-new", 900)));
                }

                Assert.That(request.RequestUri.AbsolutePath, Is.EqualTo("/api/users/me"));
                Assert.That(request.Headers.Authorization.ToString(), Is.EqualTo("Bearer access-new"));
                return Task.FromResult(JsonResponse(HttpStatusCode.OK, CurrentUserResponse(73)));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> result = await session.RestoreSessionAsync();

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Is.True);
                Assert.That(session.AccessToken, Is.EqualTo("access-new"));
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(73));
                Assert.That(session.RememberMe, Is.True);
                Assert.That(store.Token, Is.EqualTo("refresh-new"));
                Assert.That(store.SaveCount, Is.EqualTo(1));
                Assert.That(handler.RequestCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task TemporaryCurrentUserFailureClearsIdentityButKeepsRefreshedSession()
        {
            var store = new MemoryRefreshTokenStore { Token = "saved-refresh" };
            int requestIndex = 0;
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                requestIndex++;
                if (requestIndex == 1)
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-old", "refresh-old", 900, 12)));
                }

                if (requestIndex == 2)
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-new", "refresh-new", 900, 99)));
                }

                throw new HttpRequestException("temporary network failure");
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> loginResult =
                    await session.LoginAsync("test-user", "test-password", true);
                Assert.That(loginResult.IsSuccess, Is.True);
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(12));

                AuthenticationResult<bool> restoreResult = await session.RestoreSessionAsync();

                Assert.That(restoreResult.IsSuccess, Is.False);
                Assert.That(restoreResult.Error.Type, Is.EqualTo(AuthenticationFailureType.Connection));
                Assert.That(session.AuthenticatedUserId, Is.Null);
                Assert.That(session.AccessToken, Is.EqualTo("access-new"));
                Assert.That(store.Token, Is.EqualTo("refresh-new"));
            }
        }

        [Test]
        public async Task RejectedCurrentUserClearsIdentityAndInvalidatesLocalSession()
        {
            var store = new MemoryRefreshTokenStore();
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath == "/api/auth/login")
                {
                    return Task.FromResult(JsonResponse(
                        HttpStatusCode.OK,
                        TokenResponse("access-login", "refresh-login", 900, 12)));
                }

                if (request.RequestUri.AbsolutePath == "/api/auth/refresh")
                {
                    return Task.FromResult(JsonResponse(
                        HttpStatusCode.OK,
                        TokenResponse("access-renewed", "refresh-renewed", 900, 12)));
                }

                return Task.FromResult(JsonResponse(
                    HttpStatusCode.Unauthorized,
                    "{\"error\":{\"code\":\"unauthorized\",\"message\":\"Sessão inválida.\"}}"));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                await session.LoginAsync("test-user", "test-password", true);
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(12));

                AuthenticationResult<bool> restoreResult = await session.RestoreSessionAsync();

                Assert.That(restoreResult.IsSuccess, Is.False);
                Assert.That(restoreResult.Error.Type, Is.EqualTo(AuthenticationFailureType.InvalidSession));
                Assert.That(session.AuthenticatedUserId, Is.Null);
                Assert.That(session.AccessToken, Is.Null);
                Assert.That(store.Token, Is.Null);
                Assert.That(handler.RequestCount, Is.EqualTo(3));
            }
        }

        [Test]
        public async Task LoginSwitchesIdentityAndFailedLoginDoesNotKeepPreviousIdentity()
        {
            int loginCount = 0;
            var store = new MemoryRefreshTokenStore();
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                loginCount++;
                return Task.FromResult(loginCount == 1
                    ? JsonResponse(HttpStatusCode.OK, TokenResponse("access-first", "refresh-first", 900, 12))
                    : JsonResponse(
                        HttpStatusCode.Unauthorized,
                        "{\"error\":{\"code\":\"unauthorized\",\"message\":\"Credenciais inválidas.\"}}"));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> firstLogin =
                    await session.LoginAsync("first-user", "test-password", false);
                Assert.That(firstLogin.IsSuccess, Is.True);
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(12));

                AuthenticationResult<bool> failedLogin =
                    await session.LoginAsync("second-user", "wrong-password", false);

                Assert.That(failedLogin.IsSuccess, Is.False);
                Assert.That(session.AuthenticatedUserId, Is.Null);
                Assert.That(session.AccessToken, Is.Null);
            }
        }

        [Test]
        public async Task SuccessfulAccountSwitchReplacesUserId()
        {
            int loginCount = 0;
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                loginCount++;
                return Task.FromResult(JsonResponse(
                    HttpStatusCode.OK,
                    TokenResponse(
                        "access-" + loginCount,
                        "refresh-" + loginCount,
                        900,
                        loginCount == 1 ? 12 : 73)));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, new MemoryRefreshTokenStore());
                await session.LoginAsync("first-user", "test-password", false);
                await session.LoginAsync("second-user", "test-password", false);

                Assert.That(session.AuthenticatedUserId, Is.EqualTo(73));
                Assert.That(session.AccessToken, Is.EqualTo("access-2"));
            }
        }

        [Test]
        public async Task RefreshReconfirmsIdentityFromCurrentUserEndpoint()
        {
            int requestIndex = 0;
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                requestIndex++;
                if (request.RequestUri.AbsolutePath == "/api/auth/login")
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900, 12)));
                }

                if (request.RequestUri.AbsolutePath == "/api/auth/refresh")
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-2", "refresh-2", 900, 99)));
                }

                Assert.That(request.RequestUri.AbsolutePath, Is.EqualTo("/api/users/me"));
                return Task.FromResult(JsonResponse(HttpStatusCode.OK, CurrentUserResponse(73)));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, new MemoryRefreshTokenStore());
                await session.LoginAsync("test-user", "test-password", false);
                AuthenticationResult<bool> refreshResult = await session.RefreshAsync();

                Assert.That(refreshResult.IsSuccess, Is.True);
                Assert.That(session.AuthenticatedUserId, Is.EqualTo(73));
                Assert.That(session.AccessToken, Is.EqualTo("access-2"));
                Assert.That(handler.RequestCount, Is.EqualTo(3));
            }
        }

        [Test]
        public async Task ExpiredRefreshTokenIsClearedAndNotReused()
        {
            var store = new MemoryRefreshTokenStore { Token = "expired-refresh" };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
                Task.FromResult(JsonResponse(
                    HttpStatusCode.Unauthorized,
                    "{\"error\":{\"code\":\"unauthorized\",\"message\":\"Refresh token inválido ou expirado.\"}}")));

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> restoreResult = await session.RestoreSessionAsync();

                Assert.That(restoreResult.IsSuccess, Is.False);
                Assert.That(restoreResult.Error.Type, Is.EqualTo(AuthenticationFailureType.InvalidSession));
                Assert.That(session.AccessToken, Is.Null);
                Assert.That(store.Token, Is.Null);

                AuthenticationResult<bool> retryResult = await session.RefreshAsync();
                Assert.That(retryResult.IsSuccess, Is.False);
                Assert.That(retryResult.Error.Type, Is.EqualTo(AuthenticationFailureType.InvalidSession));
                Assert.That(handler.RequestCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task ConnectionFailureDuringRenewalDoesNotReuseSavedToken()
        {
            var store = new MemoryRefreshTokenStore { Token = "possibly-rotated-refresh" };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
                throw new HttpRequestException("connection interrupted"));

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> restoreResult = await session.RestoreSessionAsync();

                Assert.That(restoreResult.IsSuccess, Is.False);
                Assert.That(restoreResult.Error.Type, Is.EqualTo(AuthenticationFailureType.Connection));
                Assert.That(store.Token, Is.Null);
                Assert.That(session.AccessToken, Is.Null);
                Assert.That(handler.RequestCount, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task FailedPersistenceOfRotatedTokenClearsSessionAndRevokesIt()
        {
            var store = new MemoryRefreshTokenStore { Token = "saved-refresh", FailSave = true };
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath == "/api/auth/refresh")
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-new", "refresh-new", 900)));
                }

                return Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"data\":{\"message\":\"Sessão encerrada.\"}}"));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                AuthenticationResult<bool> result = await session.RestoreSessionAsync();

                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.Storage));
                Assert.That(session.AccessToken, Is.Null);
                Assert.That(store.Token, Is.Null);
                Assert.That(handler.RequestCount, Is.EqualTo(2));
            }
        }

        [Test]
        public async Task LogoutClearsMemoryAndStoredRefreshToken()
        {
            var store = new MemoryRefreshTokenStore();
            var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            {
                if (request.RequestUri.AbsolutePath == "/api/auth/login")
                {
                    return Task.FromResult(JsonResponse(HttpStatusCode.OK, TokenResponse("access-1", "refresh-1", 900)));
                }

                return Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"data\":{\"message\":\"Sessão encerrada.\"}}"));
            });

            using (var authentication = new AuthenticationManager("https://example.invalid", handler))
            {
                var session = new SessionManager(authentication, store);
                await session.LoginAsync("test-user", "test-password", true);

                AuthenticationResult<bool> logoutResult = await session.LogoutAsync();

                Assert.That(logoutResult.IsSuccess, Is.True);
                Assert.That(session.AccessToken, Is.Null);
                Assert.That(session.AuthenticatedUserId, Is.Null);
                Assert.That(session.IsAuthenticated, Is.False);
                Assert.That(store.Token, Is.Null);
            }
        }

        [Test]
        public void WindowsDpapiStoreProtectsAndRemovesTestToken()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor &&
                Application.platform != RuntimePlatform.WindowsPlayer)
            {
                Assert.Ignore("DPAPI é testado somente no Windows.");
            }

            string directory = Path.Combine(Path.GetTempPath(), "unity-auth-" + Guid.NewGuid().ToString("N"));
            string path = Path.Combine(directory, "refresh.dpapi");
            string token = "dummy-refresh-token-" + Guid.NewGuid().ToString("N");
            try
            {
                var store = new WindowsDpapiRefreshTokenStore(path);
                store.Save(token);

                Assert.That(store.Load(), Is.EqualTo(token));
                Assert.That(File.Exists(path), Is.True);
                Assert.That(Encoding.UTF8.GetString(File.ReadAllBytes(path)), Does.Not.Contain(token));

                store.Clear();
                Assert.That(File.Exists(path), Is.False);
            }
            finally
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        private static string TokenResponse(
            string accessToken,
            string refreshToken,
            int expiresIn,
            int userId = 42)
        {
            return "{\"data\":{\"user\":{\"id\":" + userId +
                   "},\"access_token\":\"" + accessToken +
                   "\",\"refresh_token\":\"" + refreshToken +
                   "\",\"expires_in\":" + expiresIn + "}}";
        }

        private static string CurrentUserResponse(int userId)
        {
            return "{\"data\":{\"id\":" + userId +
                   ",\"email\":\"test@example.invalid\",\"username\":\"test-user\",\"status\":true}}";
        }

        private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private sealed class MemoryRefreshTokenStore : IRefreshTokenStore
        {
            public string Token { get; set; }
            public bool FailSave { get; set; }
            public int SaveCount { get; private set; }
            public int ClearCount { get; private set; }

            public string Load()
            {
                return Token;
            }

            public void Save(string refreshToken)
            {
                if (FailSave)
                {
                    throw new RefreshTokenStoreException("simulated storage failure");
                }

                Token = refreshToken;
                SaveCount++;
            }

            public void Clear()
            {
                Token = null;
                ClearCount++;
            }
        }

        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory;

            public int RequestCount { get; private set; }

            public StubHttpMessageHandler(
                Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory)
            {
                this.responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                RequestCount++;
                return responseFactory(request, cancellationToken);
            }
        }
    }

    [Test]
    public async Task LoginReportsInvalidCredentials()
    {
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            Task.FromResult(JsonResponse(
                HttpStatusCode.Unauthorized,
                "{\"error\":{\"code\":\"unauthorized\",\"message\":\"Credenciais inválidas.\"}}")));

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.LoginAsync("test-user", "wrong-password");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.InvalidCredentials));
            Assert.That(result.Error.Code, Is.EqualTo("unauthorized"));
        }
    }

    [Test]
    public async Task RefreshParsesRotatedTokens()
    {
        string capturedPath = null;
        string capturedBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            capturedPath = request.RequestUri.AbsolutePath;
            capturedBody = await request.Content.ReadAsStringAsync();
            return JsonResponse(HttpStatusCode.OK, TokenResponse("access-2", "refresh-2", 900));
        });

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.RefreshAsync("refresh-1");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.AccessToken, Is.EqualTo("access-2"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refresh-2"));
            Assert.That(capturedPath, Is.EqualTo("/api/auth/refresh"));
            JObject requestBody = JObject.Parse(capturedBody);
            Assert.That(requestBody.Count, Is.EqualTo(1));
            Assert.That(requestBody.Value<string>("refresh_token"), Is.EqualTo("refresh-1"));
        }
    }

    [Test]
    public async Task LogoutPostsOnlyRefreshToken()
    {
        string capturedPath = null;
        string capturedBody = null;
        var handler = new StubHttpMessageHandler(async (request, cancellationToken) =>
        {
            capturedPath = request.RequestUri.AbsolutePath;
            capturedBody = await request.Content.ReadAsStringAsync();
            return JsonResponse(HttpStatusCode.OK, "{\"data\":{\"message\":\"Sessão encerrada.\"}}");
        });

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<bool> result = await manager.LogoutAsync("refresh-1");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(capturedPath, Is.EqualTo("/api/auth/logout"));
            JObject requestBody = JObject.Parse(capturedBody);
            Assert.That(requestBody.Count, Is.EqualTo(1));
            Assert.That(requestBody.Value<string>("refresh_token"), Is.EqualTo("refresh-1"));
        }
    }

    [Test]
    public async Task ConnectionFailureIsReported()
    {
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            throw new HttpRequestException("offline"));

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.LoginAsync("test-user", "test-password");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.Connection));
        }
    }

    [Test]
    public async Task MalformedSuccessResponseIsReported()
    {
        var handler = new StubHttpMessageHandler((request, cancellationToken) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"data\":{\"access_token\":\"access-only\"}}")));

        using (var manager = new AuthenticationManager("https://example.invalid", handler))
        {
            AuthenticationResult<AuthenticationTokens> result =
                await manager.LoginAsync("test-user", "test-password");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Type, Is.EqualTo(AuthenticationFailureType.UnexpectedResponse));
        }
    }

    private static string TokenResponse(
        string accessToken,
        string refreshToken,
        int expiresIn,
        int userId = 42)
    {
        return "{\"data\":{\"user\":{\"id\":" + userId +
               "},\"access_token\":\"" + accessToken +
               "\",\"refresh_token\":\"" + refreshToken +
               "\",\"expires_in\":" + expiresIn + "}}";
    }

    private static string CurrentUserResponse(int userId)
    {
        return "{\"data\":{\"id\":" + userId +
               ",\"email\":\"test@example.invalid\",\"username\":\"test-user\",\"status\":true}}";
    }

    private static TMP_InputField FindInputField(GameObject root, string objectName)
    {
        return FindComponent<TMP_InputField>(root, objectName);
    }

    private static Button FindButton(GameObject root, string objectName)
    {
        return FindComponent<Button>(root, objectName);
    }

    private static TMP_Text FindText(GameObject root, string objectName)
    {
        return FindComponent<TMP_Text>(root, objectName);
    }

    private static T FindComponent<T>(GameObject root, string objectName)
        where T : Component
    {
        T[] components = root.GetComponentsInChildren<T>(true);
        foreach (T component in components)
        {
            if (component != null &&
                string.Equals(component.gameObject.name, objectName, StringComparison.Ordinal))
            {
                return component;
            }
        }

        Assert.Fail("Não foi encontrado " + objectName + " no prefab de autenticação.");
        return null;
    }

    private static T ReadPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, "Campo privado não encontrado: " + fieldName);
        return (T)field.GetValue(instance);
    }

    private static void InvokePrivateMethod(object instance, string methodName)
    {
        MethodInfo method = instance.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "Método privado não encontrado: " + methodName);
        method.Invoke(instance, null);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory;

        public int RequestCount { get; private set; }

        public StubHttpMessageHandler(
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            return responseFactory(request, cancellationToken);
        }
    }

    private sealed class ControllerRefreshTokenStore : IRefreshTokenStore
    {
        public string Token { get; private set; }

        public string Load()
        {
            return Token;
        }

        public void Save(string refreshToken)
        {
            Token = refreshToken;
        }

        public void Clear()
        {
            Token = null;
        }
    }
}
