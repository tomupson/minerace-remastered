using MineRace.Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class ProfileUI : MonoBehaviour
{
    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button menuButton;

    private void Awake()
    {
        menuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }

    private void Start()
    {
        SetProfileText();
    }

    private void SetProfileText()
    {
        UserInfo userInfo = userAccountManager.UserInfo;
        titleText.text = userInfo.Username.ToUpper();
        nameText.text = userInfo.Username;
        highScoreText.text = userInfo.HighScore.ToString();
        xpText.text = $"{userInfo.Experience}xp";
        levelText.text = Mathf.RoundToInt(userInfo.Experience / 5000).ToString();
    }
}