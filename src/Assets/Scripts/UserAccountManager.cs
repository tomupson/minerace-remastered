using System;
using System.IO;
using UnityEngine;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager instance;
    public UserInfo userInfo;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1) // If one already exists
        {
            Destroy(gameObject); // Remove it
        }

        instance = this;

        int myId = new System.Random().Next(1000, 10000);

        userInfo = new UserInfo()
        {
            Experience = 0,
            HighScore = 0,
            Password = "1234",
            UserId = myId,
            Username = "User#" + myId
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\screenshot.png");
        }
    }
}
