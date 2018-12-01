using UnityEngine.Networking;
using UnityEngine;

public class GameSetup : NetworkBehaviour
{
    [SerializeField] private GameObject gameManPrefab;

    void Start()
    {
        GameObject go = Instantiate(gameManPrefab);
        NetworkServer.Spawn(go);
    }
}
