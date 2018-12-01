using UnityEngine;
using UnityEngine.Networking;

public class Player_SyncScale : NetworkBehaviour
{
    [SyncVar(hook = "FacingCallback")] public bool netFacingRight = true;

    [Command]
    public void CmdFlipSprite(bool facing)
    {
        netFacingRight = facing;
        if (netFacingRight)
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

    void FacingCallback(bool facing)
    {
        netFacingRight = facing;
        if (netFacingRight)
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