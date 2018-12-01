using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;

public class LoginManager : MonoBehaviour
{
    private string conn;

    [Header("Input Fields")]
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;

    [Header("Buttons")]
    [SerializeField] private Button loginButton;

    [Header("Status Texts")]
    [SerializeField] private Text loginStatusText;

    [Header("Toggles")]
    [SerializeField] private Toggle rememberMeToggle;

    void Start()
    {
        conn = "URI=file:" + Application.streamingAssetsPath + "/MineRace.db";
        loginButton.enabled = true;
        loginStatusText.text = "";

        #if UNITY_WEBGL
        rememberMeToggle.isOn = false;
        rememberMeToggle.enabled = false;
        rememberMeToggle.transform.Find("Background").GetComponent<Image>().color = Color.grey;
        #endif

        if (File.Exists(Application.persistentDataPath + "/userdata/login.session"))
        {
            loginStatusText.text = "Loading you in from previous session...";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/userdata/login.session", FileMode.Open);
            UserInfo userInfo = (UserInfo)bf.Deserialize(stream);
            stream.Close();

            if (userInfo.UserId != 0)
            {
                UserAccountManager.instance.userInfo = userInfo;
                SceneManager.LoadScene("Menu");
            }
        }
    }

    public void Login()
    {
        Crypto crypto = new Crypto();

        IDbConnection dbConnection;
        dbConnection = (IDbConnection)new SqliteConnection(conn);
        dbConnection.Open();

        IDbCommand command = dbConnection.CreateCommand();
        string sqlQuery = string.Format("SELECT id, password FROM USERS WHERE username = '{0}'", usernameInputField.text);

        command.CommandText = sqlQuery;

        IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (crypto.DecryptString(reader["password"].ToString()) == passwordInputField.text)
            {
                UserInfo info = FetchUserInfo((long)reader["id"]);
                UserAccountManager.instance.userInfo = info;
                if (rememberMeToggle.isOn) WriteSessionInfo(info);
                SceneManager.LoadScene("Menu");
            }
        }

        // Cleanup
        reader.Close();
        reader = null;

        command.Dispose();
        command = null;

        dbConnection.Close();
        dbConnection = null;
    }

    void WriteSessionInfo(UserInfo userInfo)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/userdata/login.session", FileMode.Create);
        bf.Serialize(stream, userInfo);
        stream.Close();
    }

    UserInfo FetchUserInfo(long userId)
    {
        UserInfo userInfo = new UserInfo();

        IDbConnection dbConnection;
        dbConnection = (IDbConnection)new SqliteConnection(conn);
        dbConnection.Open();

        IDbCommand command = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM USERS WHERE id = " + userId;

        command.CommandText = sqlQuery;
       
        IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            userInfo.UserId = userId;
            userInfo.Username = reader["username"].ToString();
            userInfo.Password = reader["password"].ToString();
            userInfo.Experience = Convert.ToInt32((long)reader["experience"]);
            userInfo.HighScore = Convert.ToInt32((long)reader["highScore"]);
            foreach (var p in userInfo.GetType().GetProperties())
            {
                Debug.Log(p.Name + ": " + p.GetValue(userInfo, null));
            }
        }

        // Cleanup
        reader.Close();
        reader = null;

        command.Dispose();
        command = null;

        dbConnection.Close();
        dbConnection = null;

        return userInfo;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
