using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// Block.cs holds information about the different blocks in the game
/// </summary>

[System.Serializable]
public class Block : NetworkBehaviour
{
    public string blockName; // Block Name e.g Ground
    public string blockType; // Block Type e.g Resource
    public int rarity; // Block rarity, determines in what order the game tries to spawn (ideally should be set up so that rarer materials are attempted to spawn first)
    public int blockPointsValue; // How many points the user gains for destroying this block.
    public float basePercentage; // Base spawn % at the top y-level
    public float percentageMultiplier; // Multiplier as it goes down each y-level.
    [HideInInspector] public float[] spawnPercentagesAtLevels; // A list of all these spawn %s
    [HideInInspector] public int textureIndex; // For each block, this will hold its current texture 
    public List<Sprite> blockTextures; // A list of all the block textures (to add randomness)
    public List<Sprite> blockOutlineTextures; // A list of all the block outline textures.
    public string blockBreakSoundName; // The name of the sound that is played when the block is broken.

    [SyncVar] public NetworkInstanceId parentNetId; // The network id of the level generator object.

    void Start()
    {
        GameObject parent = ClientScene.FindLocalObject(parentNetId); // Find the level generator object in the local scene by using its network id
        transform.SetParent(parent.transform); // and parent ourselves to it.
        // We're doing this because for the player who joins, we're checking the server for if a block has been spawned however that information doesn't contain anything about what its parent is so for all remote clients we need to parent it back semi-manually.
    }

}