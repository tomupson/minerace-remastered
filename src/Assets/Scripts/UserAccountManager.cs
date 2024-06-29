using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance;
    public UserInfo userInfo;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        Instance = this;

        int id = Random.Range(1000, 10000);

        userInfo = new UserInfo
        {
            UserId = id,
            Username = $"User#{id}"
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot($"MineRace_Screenshot_{DateTime.Now:s}.png");
        }
    }
}
