// TODO: NETWORKING
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Holds information about a block
/// </summary>
[Serializable]
public class Block : NetworkBehaviour
{
    public string blockName;
    public string blockType;
    [Tooltip("Determines in what order the game tries to spawn (ideally should be set up so that rarer materials are attempted to spawn first)")]
    public int rarity;
    public int blockPointsValue;
    [Tooltip("Base spawn % at the top y-level")]
    public float basePercentage;
    [Tooltip("Multiplier as the level generator goes down each y-level")]
    public float percentageMultiplier;
    [HideInInspector] public float[] spawnPercentagesAtLevels;
    [HideInInspector] public int textureIndex;
    [Tooltip("Set of block textures, to allow variance between blocks of the same type")]
    public List<Sprite> blockTextures;
    [Tooltip("Set of block outline textures, to allow variance between blocks of the same type")]
    public List<Sprite> blockOutlineTextures;
    public string blockBreakSoundName;

    //[SyncVar] public NetworkInstanceId parentNetId; // The network id of the level generator object.

    //void Start()
    //{
    //    GameObject parent = ClientScene.FindLocalObject(parentNetId); // Find the level generator object in the local scene by using its network id
    //    transform.SetParent(parent.transform); // and parent ourselves to it.
    //    // We're doing this because for the player who joins, we're checking the server for if a block has been spawned however that information doesn't contain anything about what its parent is so for all remote clients we need to parent it back semi-manually.
    //}
}