using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance { get; private set; }
    public UserInfo UserInfo { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);

        // TODO: Look at removing this once Login scene is properly used
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        Instance = this;

        int id = UnityEngine.Random.Range(1000, 10000);

        UserInfo = new UserInfo
        {
            UserId = id,
            Username = $"User#{id}"
        };
    }

    private async void Start()
    {
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

        //await UnityServices.InitializeAsync();
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot($"MineRace_Screenshot_{DateTime.Now:s}.png");
        }
    }
}
