using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace MineRace.Authentication
{
    public class UserAccountManager : MonoBehaviour
    {
        public UserInfo UserInfo { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> Login(string username)
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
#if DEBUG
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

                AuthenticationService.Instance.SignedIn += () => Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
                AuthenticationService.Instance.SignedOut += () => Debug.Log("Signed out");

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                int id = Random.Range(1000, 10000);
                UserInfo = new UserInfo { UserId = id, Username = username };

                if (string.IsNullOrWhiteSpace(username))
                {
                    UserInfo.Username = $"User#{id}";
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
        }
    }
}