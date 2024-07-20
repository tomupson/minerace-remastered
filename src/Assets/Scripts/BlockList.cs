using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockList", menuName = "MineRace/Block List")]
public class BlockList : ScriptableObject
{
    public List<Block> blocks;
}
