using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace MineRace.Authentication
{
    public class UserAccountManager : MonoBehaviour
    {
        public event Action<string> OnUsernameChanged;
        public event Action OnLogout;

        public UserInfo UserInfo { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            AuthenticationService.Instance.SignedIn += () => Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
            AuthenticationService.Instance.SignedOut += () => Debug.Log("Signed out");
        }

        public async Task<bool> Login(string username)
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    InitializationOptions hostOptions = new InitializationOptions().SetProfile("host");
                    InitializationOptions clientOptions = new InitializationOptions().SetProfile("client");

                    await UnityServices.InitializeAsync(hostOptions);

                    if (AuthenticationService.Instance.IsAuthorized)
                    {
                        Debug.Log("Authorized");
                        AuthenticationService.Instance.SignOut();
                        await UnityServices.InitializeAsync(clientOptions);
                    }
#else
                    await UnityServices.InitializeAsync();
#endif
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    username = AuthenticationService.Instance.PlayerId;
                }

                if (string.IsNullOrWhiteSpace(UserInfo?.Username) || !UserInfo.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    UserInfo ??= new UserInfo();
                    SetUsername(username);
                }

                return true;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            return false;
        }

        public void Logout()
        {
            AuthenticationService.Instance.SignOut();
            OnLogout?.Invoke();
        }

        public void SetUsername(string username)
        {
            UserInfo.Username = username;
            OnUsernameChanged?.Invoke(username);
        }
    }
}