using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LevelGenerator : NetworkBehaviour
{
    private const int BlockSize = 1;

    private List<GameObject> resourceBlocks = new List<GameObject>();

    [SerializeField] private List<GameObject> blocks;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private GameObject borderBlockPrefab;
    [SerializeField] private GameObject endBlockPrefab;

    private void Start()
    {
        if (IsServer)
        {
            SpawnMapServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnMapServerRpc()
    {
        resourceBlocks = blocks
            .Select(o => new { gameObject = o, block = o.GetComponent<Block>() })
            .Where(x => x.block.blockType == "Resource")
            .OrderBy(x => x.block.rarity)
            .Select(x => x.gameObject)
            .ToList();
        
        foreach (GameObject blockObject in resourceBlocks)
        {
            Block block = blockObject.GetComponent<Block>();
            block.spawnPercentagesAtLevels = new float[mapHeight];
            for (int spawnLevel = 1; spawnLevel < block.spawnPercentagesAtLevels.Length; spawnLevel++)
            {
                block.spawnPercentagesAtLevels[spawnLevel] = block.basePercentage * Mathf.Pow(block.percentageMultiplier, block.spawnPercentagesAtLevels.Length - spawnLevel);
            }
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (y == mapHeight - 1)
                {
                    GameObject grass = blocks.FirstOrDefault(b => b.GetComponent<Block>().blockName == "Grass");
                    GameObject blockInstance = Instantiate(grass);
                    blockInstance.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
                    SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                    Block block = blockInstance.GetComponent<Block>();
                    int blockTextureIndex = Random.Range(0, block.blockTextures.Count);
                    blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                    block.textureIndex = blockTextureIndex;
                    blockInstance.transform.position = new Vector3(x * BlockSize, y * BlockSize, 0);
                    blockInstance.transform.SetParent(transform, false);
                    blockInstance.name = $"{block.blockName} => {x}, {y}";
                    continue;
                }

                bool resourceSpawned = false;
                for (int i = resourceBlocks.Count - 1; i >= 0; i--)
                {
                    float roll = Random.Range(0.0f, 100.0f);
                    if (roll <= resourceBlocks[i].GetComponent<Block>().spawnPercentagesAtLevels[y])
                    {
                        GameObject blockInstance = Instantiate(resourceBlocks[i]);
                        blockInstance.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

                        SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                        Block block = blockInstance.GetComponent<Block>();
                        int blockTextureIndex = Random.Range(0, block.blockTextures.Count);
                        blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                        block.textureIndex = blockTextureIndex;

                        blockInstance.transform.position = new Vector3(x * BlockSize, y * BlockSize, 0);
                        blockInstance.transform.SetParent(transform, false);
                        blockInstance.name = $"{block.blockName} => {x}, {y}";
                        resourceSpawned = true;
                        break;
                    }
                }

                if (!resourceSpawned)
                {
                    GameObject ground = blocks.FirstOrDefault(b => b.GetComponent<Block>().blockName == "Ground");
                    GameObject blockInstance = Instantiate(ground);
                    blockInstance.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

                    SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                    Block block = blockInstance.GetComponent<Block>();
                    int blockTextureIndex = Random.Range(0, block.blockTextures.Count);
                    blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                    block.textureIndex = blockTextureIndex;

                    blockInstance.transform.position = new Vector3(x * BlockSize, y * BlockSize, 0);
                    blockInstance.transform.SetParent(transform, false);
                    blockInstance.name = $"{block.blockName} => {x}, {y}";
                }

                resourceSpawned = false;
            }
        }

        for (int y = 0; y < mapHeight + 1; y++)
        {
            GameObject leftBorder = Instantiate(borderBlockPrefab);
            leftBorder.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
            leftBorder.transform.position = new Vector3(-1 * BlockSize, y * BlockSize, 0);
            leftBorder.transform.SetParent(transform, false);
            leftBorder.name = "BORDER_BLOCK_LEFT";

            GameObject rightBorder = Instantiate(borderBlockPrefab);
            rightBorder.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
            rightBorder.transform.position = new Vector3(mapWidth * BlockSize, y * BlockSize, 0);
            rightBorder.transform.SetParent(transform, false);
            rightBorder.name = "BORDER_BLOCK_RIGHT";
            Vector3 s = rightBorder.transform.localScale;
            rightBorder.transform.localScale = new Vector3(s.x * -1, s.y, s.z);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            GameObject endBlock = Instantiate(endBlockPrefab);
            endBlock.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
            endBlock.transform.position = new Vector3(x * BlockSize, -1, 0);
            endBlock.transform.SetParent(transform, false);
            endBlock.name = "END_GAME_BLOCK";
        }
    }
}
