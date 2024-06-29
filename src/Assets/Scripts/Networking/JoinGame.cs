// TODO: NETWORKING
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{
    [Header("Game List References")]
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject gameListItemPrefab;
    [SerializeField] private Transform gameListTransform;
    [SerializeField] private InputField filterMatchField;

    [Header("Joining Game with password")]
    [SerializeField] private GameObject passwordCanvas;
    [SerializeField] private InputField matchPasswordInputField;
    [SerializeField] private Text passwordStatusText;
    [SerializeField] private Button joinGamePasswordButton;

    [Header("Database Info")]
    [SerializeField] private Text loggedInAsText;

    List<GameObject> gameList = new List<GameObject>();

    //MatchInfoSnapshot matchToJoin;

    void Start()
    {
        //if (NetworkManager.Singleton.matchMaker == null)
        //    NetworkManager.Singleton.StartMatchMaker();

        passwordCanvas.SetActive(false);

        loggedInAsText.text = "LOGGED IN AS: " + UserAccountManager.Instance.userInfo.Username;

        RefreshGameList();
    }

    public void RefreshGameList()
    {
        #if UNITY_WEBGL
        statusText.text = "Matchmaking not available on WebGL. Download to play.";
        #else
        
        ClearGameList();

        statusText.text = "Searching for open games.";

        //if (NetworkManager.Singleton.matchMaker == null)
        //    NetworkManager.Singleton.StartMatchMaker();

        //NetworkManager.Singleton.matchMaker.ListMatches(0, 20, filterMatchField.text, false, 0, 0, OnMatchList);
        #endif
    }

    public void RefreshButtonPressed()
    {
        AudioManager.Instance.PlaySound("button_press");
        RefreshGameList();
    }

    //public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    //{
    //    statusText.text = "";
    //    if (matchList == null)
    //    {
    //        statusText.text = "Failed to fetch games.";
    //        return;
    //    }

    //    foreach (MatchInfoSnapshot match in matchList)
    //    {
    //        GameObject gameListItemGO = Instantiate(gameListItemPrefab);
    //        gameListItemGO.transform.SetParent(gameListTransform);

    //        GameListItem gameListItem = gameListItemGO.GetComponent<GameListItem>();
    //        if (gameListItem != null)
    //            gameListItem.Setup(match, JoinRoom);

    //        gameList.Add(gameListItemGO);
    //    }

    //    if (gameList.Count == 0)
    //        statusText.text = "No Games available.";
    //}

    void ClearGameList()
    {
        for (int i = 0; i < gameList.Count; i++)
        {
            Destroy(gameList[i]);
        }

        gameList.Clear();
    }

    //public void JoinRoom(MatchInfoSnapshot _match)
    //{
    //    matchToJoin = _match;

    //    if (_match.isPrivate)
    //    {
    //        joinGamePasswordButton.enabled = true;
    //        passwordCanvas.SetActive(true);
    //        passwordStatusText.text = "";
    //        Animation anim = passwordCanvas.transform.GetChild(0).GetComponent<Animation>();
    //        anim["grow"].speed = 1;
    //        anim["grow"].time = 0;
    //        anim.Play("grow");
    //        return;
    //    } else
    //    {
    //        NetworkManager.Singleton.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, NetworkManager.Singleton.OnMatchJoined);
    //    }
    //}

    //public void JoinRoomPassword()
    //{
    //    if (matchPasswordInputField.text == null || matchPasswordInputField.text == "")
    //    {
    //        AudioManager.instance.PlaySound("error_message");
    //        passwordStatusText.text = "The field 'Match Password' is required.";
    //    } else
    //    {
    //        joinGamePasswordButton.enabled = false;

    //        AudioManager.instance.PlaySound("button_press");
    //        ClosePasswordCanvas();

    //        if (matchToJoin == null)
    //            return;

    //        Debug.Log("Joining: " + matchToJoin.name);
    //        NetworkManager.Singleton.matchMaker.JoinMatch(matchToJoin.networkId, matchPasswordInputField.text, "", "", 0, 0, NetworkManager.Singleton.OnMatchJoined);
    //        StartCoroutine(WaitForSuccessfulJoin());
    //    }
    //}

    public void ClosePasswordCanvas()
    {
        StartCoroutine(WaitForAnimation(passwordCanvas, passwordCanvas.transform.GetChild(0).GetComponent<Animation>(), "grow"));
    }

    IEnumerator WaitForAnimation(GameObject canvas, Animation anim, string animationName)
    {
        anim[animationName].speed = -1;
        anim[animationName].time = anim[animationName].length;
        anim.Play(animationName);

        yield return new WaitWhile(() => anim.isPlaying);

        canvas.SetActive(false);

        yield return null;
    }

    IEnumerator WaitForSuccessfulJoin()
    {
        ClearGameList();

        int countdown = 10;
        while (countdown > 0)
        {
            statusText.text = "Joining Game...";

            yield return new WaitForSeconds(1);

            countdown--;
        }

        // If you're still on the same scene when the countdown is over.
        AudioManager.Instance.PlaySound("connection_error");
        statusText.text = "Failed to connect.";
        yield return new WaitForSeconds(2.5f);
        //MatchInfo matchInfo = NetworkManager.Singleton.matchInfo;
        //if (matchInfo != null)
        //{
        //    NetworkManager.Singleton.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, NetworkManager.Singleton.OnDropConnection);
        //    NetworkManager.Singleton.StopHost();
        //}

        //matchToJoin = null;

        RefreshGameList();
    }

    public void TEMP_LAN_HOST()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void TEMP_LAN_CLIENT()
    {
        NetworkManager.Singleton.StartClient();
    }
}
