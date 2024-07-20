using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "MineRace/Block")]
public class Block : ScriptableObject
{
    public string blockName;
    public BlockType blockType;
    public GameObject prefab;
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
}
