using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text levelText;

    private void Start()
    {
        SetProfileText();
    }

    private void SetProfileText()
    {
        UserInfo userInfo = UserAccountManager.Instance.UserInfo;
        titleText.text = userInfo.Username.ToUpper();
        nameText.text = userInfo.Username;
        highScoreText.text = userInfo.HighScore.ToString();
        xpText.text = $"{userInfo.Experience}xp";
        levelText.text = Mathf.RoundToInt(userInfo.Experience / 5000).ToString();
    }
}