using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LevelGenerator : NetworkBehaviour
{
    private const int BlockSize = 1;

    public static LevelGenerator Instance { get; private set; }

    [SerializeField] private List<GameObject> blocks;
    [SerializeField] private GameObject borderBlockPrefab;
    [SerializeField] private GameObject endBlockPrefab;

    public int mapWidth;
    public int mapHeight;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (IsServer)
        {
            SpawnMap();
        }
    }

    private void SpawnMap()
    {
        List<BlockData> blockDatas = blocks.Select(go => new BlockData(go)).ToList();
        List<BlockData> resourceBlockDatas = blockDatas
            .Where(data => data.block.blockType == BlockType.Resource)
            .OrderBy(data => data.block.rarity)
            .ToList();

        foreach (BlockData blockData in resourceBlockDatas)
        {
            blockData.block.spawnPercentagesAtLevels = new float[mapHeight];
            for (int spawnLevel = 1; spawnLevel < blockData.block.spawnPercentagesAtLevels.Length; spawnLevel++)
            {
                blockData.block.spawnPercentagesAtLevels[spawnLevel] = blockData.block.basePercentage * Mathf.Pow(blockData.block.percentageMultiplier, blockData.block.spawnPercentagesAtLevels.Length - spawnLevel);
            }
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (y == mapHeight - 1)
                {
                    BlockData grassBlockData = blockDatas.First(data => data.block.blockName == "Grass");
                    SpawnBlock(grassBlockData, x, y);
                    continue;
                }

                bool resourceSpawned = false;
                for (int i = resourceBlockDatas.Count - 1; i >= 0; i--)
                {
                    BlockData resourceBlockData = resourceBlockDatas[i];
                    float roll = Random.Range(0.0f, 100.0f);
                    if (roll <= resourceBlockData.block.spawnPercentagesAtLevels[y])
                    {
                        SpawnBlock(resourceBlockData, x, y);
                        resourceSpawned = true;
                        break;
                    }
                }

                if (!resourceSpawned)
                {
                    BlockData groundBlockData = blockDatas.First(data => data.block.blockName == "Ground");
                    SpawnBlock(groundBlockData, x, y);
                }
            }
        }

        for (int y = 0; y < mapHeight + 1; y++)
        {
            SpawnBorder(mapX: -1, mapY: y);
            SpawnBorder(mapX: mapWidth, mapY: y, isLeft: false);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            SpawnEndBlock(mapX: x, mapY: -1);
        }
    }

    private void SpawnBlock(BlockData blockData, int mapX, int mapY)
    {
        GameObject blockInstance = SpawnOnNetwork(blockData.prefab);
        blockInstance.name = $"{blockData.block.blockName} => {mapX}, {mapY}";

        SetRandomBlockTexture(blockInstance, blockData.block);
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

        int blockTextureIndex = Random.Range(0, block.blockTextures.Count);
        blockRenderer.sprite = block.blockTextures[blockTextureIndex];
        block.textureIndex = blockTextureIndex;
    }

    private void SetBlockPosition(GameObject blockInstance, int mapX, int mapY)
    {
        blockInstance.transform.position = new Vector3(mapX * BlockSize, mapY * BlockSize, 0);
        blockInstance.transform.SetParent(transform, worldPositionStays: false);
    }

    private sealed class BlockData
    {
        public GameObject prefab;
        public Block block;

        public BlockData(GameObject prefab)
        {
            this.prefab = prefab;
            this.block = prefab.GetComponent<Block>();
        }
    }
}
