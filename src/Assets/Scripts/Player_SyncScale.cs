using Unity.Netcode;
using UnityEngine;

public class Player_SyncScale : NetworkBehaviour
{
    public NetworkVariable<bool> netFacingRight = new NetworkVariable<bool>(true);

    private void Awake()
    {
        netFacingRight.OnValueChanged += FacingCallback;
    }

    [ServerRpc]
    public void FlipSpriteServerRpc(bool facing)
    {
        netFacingRight.Value = facing;
        if (facing)
        {
            Vector3 s = transform.localScale;
            s.x = 1;
            transform.localScale = s;
        }
        else
        {
            Vector3 SpriteScale = transform.localScale;
            SpriteScale.x = -1;
            transform.localScale = SpriteScale;
        }
    }

    void FacingCallback(bool oldFacingRight, bool newFacingRight)
    {
        if (newFacingRight)
        {
            Vector3 s = transform.localScale;
            s.x = 1;
            transform.localScale = s;
        }
        else
        {
            Vector3 SpriteScale = transform.localScale;
            SpriteScale.x = -1;
            transform.localScale = SpriteScale;
        }
    }
}