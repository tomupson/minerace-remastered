using Unity.Netcode;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }

            return;
        }

        // TODO: Fetch from somewhere
        const int mapWidth = 48;
        transform.position = new Vector3(mapWidth / 2 + 12 * (OwnerClientId * 2 - 1), 100, 0);
    }
}
