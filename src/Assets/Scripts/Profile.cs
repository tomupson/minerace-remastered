using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    private UserInfo userInfo;

    [Header("User Info")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text highscoreText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text levelText;

    private void Start()
    {
        userInfo = UserAccountManager.Instance.UserInfo;
        SetProfileText();
    }

    private void SetProfileText()
    {
        titleText.text = userInfo.Username.ToUpper();
        nameText.text = userInfo.Username;
        highscoreText.text = userInfo.HighScore.ToString();
        xpText.text = $"{userInfo.Experience}xp";
        levelText.text = Mathf.RoundToInt(userInfo.Experience / 5000).ToString();
    }
}