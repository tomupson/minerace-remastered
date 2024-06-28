// TODO: NETWORKING
using Unity.Netcode;
using UnityEngine;

public class GameSetup : NetworkBehaviour
{
    [SerializeField] private GameObject gameManPrefab;

    void Start()
    {
        GameObject go = Instantiate(gameManPrefab);
        //NetworkServer.Spawn(go);
    }
}
