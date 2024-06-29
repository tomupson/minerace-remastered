using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    private Text timesUpText;

    [SerializeField] private GameObject timesUpPanel;
    [SerializeField] private GameObject finalScoresPanel;
    [SerializeField] private Text[] playerNameText;
    [SerializeField] private Text[] finalPointsText;
    [SerializeField] private Text[] finalMoneyText;

    [Header("Game Finished")]
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private Text gameFinishedText;

    void Start()
	{
        finalScoresPanel = GameObject.Find("FinalScoresPanel");
        timesUpText = timesUpPanel.GetComponentInChildren<Text>();
        timesUpPanel.SetActive(false);
        finalScoresPanel.SetActive(false);
        gameFinishedPanel.SetActive(false);
    }

    [ServerRpc]
    public void TimesUpServerRpc()
    {
        ShowTimesUpClientRpc();
    }

    [ClientRpc]
    public void ShowTimesUpClientRpc()
    {
        timesUpPanel.SetActive(true);
        timesUpText.GetComponent<Animation>().Play("timesUp");
        StartCoroutine(OpenScoresPanel());
    }

    public void GameFinished()
    {
        gameFinishedPanel.SetActive(true);
        Animation anim = gameFinishedText.GetComponent<Animation>();
        anim.Play("timesUp");
        StartCoroutine(OpenScoresPanel());
    }

    public IEnumerator OpenScoresPanel()
    {
        yield return new WaitForSeconds(3);

        Player[] players = FindObjectsOfType<Player>();
        for (var i = 0; i < players.Length; i++)
        {
            playerNameText[i].enabled = false;
            finalPointsText[i].enabled = false;
            finalMoneyText[i].enabled = false;
        }

        timesUpText.enabled = false;
        gameFinishedPanel.SetActive(false);
        finalScoresPanel.SetActive(true);

        finalScoresPanel.GetComponent<Animation>().Play("openFinalScoresPanel");
        yield return new WaitUntil(() => !finalScoresPanel.GetComponent<Animation>().isPlaying);

        for (int i = 0; i < players.Length; i++)
        {
            // Show player name things.
            playerNameText[i].enabled = true;
            playerNameText[i].text = "Player: " + players[i].username;
            playerNameText[i].GetComponent<Animation>().Play("dropIn");
            yield return new WaitUntil(() => !playerNameText[i].GetComponent<Animation>().isPlaying);
            finalPointsText[i].enabled = true;
            int counted = 0;

            while (counted < players[i].points.Value)
            {
                counted += (int)(players[i].points.Value / 1 * Time.deltaTime);
                finalPointsText[i].text = "Points: +" + counted;
                yield return null;
            }

            finalMoneyText[i].enabled = true;

            finalMoneyText[i].text = "Points: +" + counted;
            counted = 0;
            while (counted < Mathf.FloorToInt(players[i].points.Value / 4))
            {
                if (counted % 4 == 0) AudioManager.Instance.PlaySound("money_gain");
                counted += (int)((players[i].points.Value / 4) / 1 * Time.deltaTime);
                finalMoneyText[i].text = "Money: +$" + counted;
                yield return null;
            }

            finalMoneyText[i].text = "Money: +$" + counted;
        }
    }
}
