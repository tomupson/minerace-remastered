using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void Logout()
    {
        UserInfo blankInfo = new UserInfo();
        if (File.Exists(Application.persistentDataPath + "/userdata/login.session") && Directory.Exists(Application.persistentDataPath + "/userdata"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/userdata/login.session", FileMode.Create);
            bf.Serialize(stream, blankInfo);
            stream.Close();
        }

        SceneManager.LoadScene("Login");
    }
}