// TODO: NETWORKING
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    private NetworkManager netMan;

    [Header("Match Settings")]
    [SerializeField] private uint gameSize = 2;

    [Header("Create Match References")]
    [SerializeField] private GameObject createMatchCanvas;
    [SerializeField] private InputField gameNameField;
    [SerializeField] private InputField gamePasswordField;
    [SerializeField] private Text createGameStatusText;
    [SerializeField] private Button createGameButton;

    void Start()
    {
        createMatchCanvas.SetActive(false);
        netMan = NetworkManager.Singleton;
        //if (netMan.matchMaker == null)
        //    netMan.StartMatchMaker();
    }

    public void CreateRoom()
    {
        string gameName = gameNameField.text;
        string gamePassword = gamePasswordField.text ?? string.Empty;

        if (gamePassword == null)
        {
            gamePassword = "";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(gameName))
            {
                createGameButton.enabled = false;
                AudioManager.Instance.PlaySound("button_press");
                Debug.Log("Creating Game");
                //netMan.matchMaker.CreateMatch(gameName, gameSize, true, gamePassword, "", "", 0, 0, netMan.OnMatchCreate);
            }
            else
            {
                AudioManager.Instance.PlaySound("error_message");
                createGameStatusText.text = "The field 'Game Name' is required.";
            }
        }
    }

    public void OpenCreateMatch()
    {
        AudioManager.Instance.PlaySound("button_press");
        createMatchCanvas.SetActive(true);
        Animation anim = createMatchCanvas.transform.GetChild(0).GetComponent<Animation>();
        anim["grow"].speed = 1;
        anim["grow"].time = 0;
        anim.Play("grow");
        createGameStatusText.text = "";
    }

    public void CloseCreateMatch()
    {
        StartCoroutine(WaitForAnimation());
    }

    IEnumerator WaitForAnimation()
    {
        Animation anim = createMatchCanvas.transform.GetChild(0).GetComponent<Animation>();
        FindObjectOfType<ButtonHighlight>().GetComponentInChildren<Text>().color = Color.white;
        anim["grow"].speed = -1;
        anim["grow"].time = anim["grow"].length;
        anim.Play("grow");

        yield return new WaitWhile(() => anim.isPlaying);

        createMatchCanvas.SetActive(false);

        yield return null;
    }
}
