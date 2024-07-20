using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance { get; private set; }
    public UserInfo UserInfo { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;

        PlayerInputActions inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Screenshot.performed += OnScreenshotPerformed;
    }

    public async Task<bool> Login(string username)
    {
        try
        {
#if DEBUG
            InitializationOptions hostOptions = new InitializationOptions().SetProfile("host");
            InitializationOptions clientOptions = new InitializationOptions().SetProfile("client");

            await UnityServices.InitializeAsync(hostOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
            };

            if (AuthenticationService.Instance.IsAuthorized)
            {
                Debug.Log("Authorized");
                AuthenticationService.Instance.SignOut();
                await UnityServices.InitializeAsync(clientOptions);
            }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
#else
            //await UnityServices.InitializeAsync();
            //await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif

            int id = UnityEngine.Random.Range(1000, 10000);
            UserInfo user = new UserInfo();
            user.UserId = id;
            user.Username = username ?? $"User#{id}";
            return true;
        }
        catch (AuthenticationException) { }
        return false;
    }

    private void OnScreenshotPerformed(InputAction.CallbackContext context)
    {
        ScreenCapture.CaptureScreenshot($"MineRace_Screenshot_{DateTime.Now:s}.png");
    }
}
