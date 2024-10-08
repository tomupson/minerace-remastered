using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ServerLevelGenerator : NetworkBehaviour
{
    private const int BlockSize = 1;

    [SerializeField] private LevelData levelData;
    [SerializeField] private BlockList blockList;
    [SerializeField] private GameObject borderBlockPrefab;
    [SerializeField] private GameObject endBlockPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        SpawnMap();
    }

    private void SpawnMap()
    {
        List<Block> resourceBlocks = blockList.blocks
            .Where(block => block.blockType == BlockType.Resource)
            .OrderBy(block => block.rarity)
            .ToList();

        foreach (Block block in resourceBlocks)
        {
            block.spawnPercentagesAtLevels = new float[levelData.mapHeight];
            for (int spawnLevel = 1; spawnLevel < block.spawnPercentagesAtLevels.Length; spawnLevel++)
            {
                block.spawnPercentagesAtLevels[spawnLevel] = block.basePercentage * Mathf.Pow(block.percentageMultiplier, block.spawnPercentagesAtLevels.Length - spawnLevel);
            }
        }

        for (int x = 0; x < levelData.mapWidth; x++)
        {
            for (int y = 0; y < levelData.mapHeight; y++)
            {
                if (y == levelData.mapHeight - 1)
                {
                    Block grassBlock = blockList.blocks.First(block => block.blockName == "Grass");
                    SpawnBlock(grassBlock, x, y);
                    continue;
                }

                bool resourceSpawned = false;
                for (int resourceBlockIdx = resourceBlocks.Count - 1; resourceBlockIdx >= 0; resourceBlockIdx--)
                {
                    Block resourceBlock = resourceBlocks[resourceBlockIdx];
                    float roll = Random.Range(0.0f, 100.0f);
                    if (roll <= resourceBlock.spawnPercentagesAtLevels[y])
                    {
                        SpawnBlock(resourceBlock, x, y);
                        resourceSpawned = true;
                        break;
                    }
                }

                if (!resourceSpawned)
                {
                    Block groundBlock = blockList.blocks.First(block => block.blockName == "Ground");
                    SpawnBlock(groundBlock, x, y);
                }
            }
        }

        for (int y = 0; y < levelData.mapHeight + 2; y++)
        {
            SpawnBorder(mapX: -1, mapY: y);
            SpawnBorder(mapX: levelData.mapWidth, mapY: y, isLeft: false);
        }

        for (int x = 0; x < levelData.mapWidth; x++)
        {
            SpawnEndBlock(mapX: x, mapY: -1);
        }
    }

    private void SpawnBlock(Block block, int mapX, int mapY)
    {
        GameObject blockInstance = SpawnOnNetwork(block.prefab);
        blockInstance.name = $"{block.blockName} => {mapX}, {mapY}";

        SetRandomBlockTexture(blockInstance, block);
        SetBlockPosition(blockInstance, mapX, mapY);
    }

    private void SpawnBorder(int mapX, int mapY, bool isLeft = true)
    {
        GameObject borderBlockInstance = SpawnOnNetwork(borderBlockPrefab);
        borderBlockInstance.name = $"BORDER_BLOCK_{(isLeft ? "LEFT" : "RIGHT")}";

        SetBlockPosition(borderBlockInstance, mapX, mapY);

        if (!isLeft)
        {
            Vector3 borderLocalScale = borderBlockInstance.transform.localScale;
            borderBlockInstance.transform.localScale = new Vector3(borderLocalScale.x * -1, borderLocalScale.y, borderLocalScale.z);
        }
    }

    private void SpawnEndBlock(int mapX, int mapY)
    {
        GameObject endBlockInstance = SpawnOnNetwork(endBlockPrefab);
        endBlockInstance.name = "END_GAME_BLOCK";

        SetBlockPosition(endBlockInstance, mapX, mapY);
    }

    private GameObject SpawnOnNetwork(GameObject gameObject)
    {
        GameObject spawnedObject = Instantiate(gameObject);
        spawnedObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        return spawnedObject;
    }

    private void SetRandomBlockTexture(GameObject blockInstance, Block block)
    {
        SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();

        int blockTextureIndex = Random.Range(0, block.textures.Count);
        blockRenderer.sprite = block.textures[blockTextureIndex];
        block.textureIndex = blockTextureIndex;
    }

    private void SetBlockPosition(GameObject blockInstance, int mapX, int mapY)
    {
        blockInstance.transform.position = new Vector3(mapX * BlockSize, mapY * BlockSize, 0);
        blockInstance.transform.SetParent(transform, worldPositionStays: false);
    }
}
