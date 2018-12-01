using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class LevelGenerator : NetworkBehaviour
{
    [SerializeField] private List<GameObject> blocks; // Blocks are pre-defined in the Unity Editor.
    [SerializeField] private int mapWidth; // How wide the map is.
    [SerializeField] private int mapHeight; // How deep the map is.
    public const int blockSize = 1;
    private List<GameObject> resourceBlocks = new List<GameObject>();
    [SerializeField] private GameObject borderBlockPrefab;
    [SerializeField] private GameObject endBlockPrefab;

    void Start()
    {
        if (isServer)
            CmdSpawnMap();
    }

    [Command]
    public void CmdSpawnMap()
    {
        resourceBlocks = blocks.Where(z => z.GetComponent<Block>().blockType == "Resource").ToList(); // List of all the blocks that are tagged as resources.
        
        resourceBlocks = resourceBlocks.OrderBy(z => z.GetComponent<Block>().rarity).ToList();

        for (int r = 0; r < resourceBlocks.Count; r++)
        {
            Block b = resourceBlocks[r].GetComponent<Block>();
            b.spawnPercentagesAtLevels = new float[mapHeight];
            for (int s = 1; s < b.spawnPercentagesAtLevels.Length; s++)
            {
                b.spawnPercentagesAtLevels[s] = b.basePercentage * Mathf.Pow(b.percentageMultiplier, b.spawnPercentagesAtLevels.Length - s);
            }
        }

        // For every block in the map.
        System.Random random = new System.Random();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (y == mapHeight - 1)
                {
                    GameObject grass = blocks.Where(z => z.GetComponent<Block>().blockName == "Grass").FirstOrDefault();
                    GameObject blockInstance = Instantiate(grass);
                    SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                    Block block = blockInstance.GetComponent<Block>();
                    int blockTextureIndex = random.Next(0, block.blockTextures.Count - 1);
                    blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                    block.textureIndex = blockTextureIndex;
                    blockInstance.transform.position = new Vector3(x * blockSize, y * blockSize, 0);
                    blockInstance.transform.SetParent(transform, false);
                    blockInstance.name = block.blockName + " => " + x + ", " + y;

                    block.parentNetId = GetComponent<NetworkIdentity>().netId;

                    NetworkServer.Spawn(blockInstance);
                    continue;
                }

                bool resourceSpawned = false;
                for (int i = resourceBlocks.Count - 1; i >= 0; i--)
                {
                    float roll = Random.Range(0.0f, 100.0f);
                    if (roll <= resourceBlocks[i].GetComponent<Block>().spawnPercentagesAtLevels[y])
                    {
                        GameObject blockInstance = Instantiate(resourceBlocks[i]);

                        SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                        Block block = blockInstance.GetComponent<Block>();
                        int blockTextureIndex = random.Next(0, block.blockTextures.Count - 1);
                        blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                        block.textureIndex = blockTextureIndex;

                        blockInstance.transform.position = new Vector3(x * blockSize, y * blockSize, 0);
                        blockInstance.transform.SetParent(transform, false);
                        blockInstance.name = block.blockName + " => " + x + ", " + y;
                        resourceSpawned = true;

                        block.parentNetId = GetComponent<NetworkIdentity>().netId;

                        NetworkServer.Spawn(blockInstance);
                        break;
                    }
                }

                if (!resourceSpawned)
                {
                    GameObject ground = blocks.Where(z => z.GetComponent<Block>().blockName == "Ground").FirstOrDefault();
                    GameObject blockInstance = Instantiate(ground);

                    SpriteRenderer blockRenderer = blockInstance.GetComponent<SpriteRenderer>();
                    Block block = blockInstance.GetComponent<Block>();
                    int blockTextureIndex = random.Next(0, block.blockTextures.Count - 1);
                    blockRenderer.sprite = block.blockTextures[blockTextureIndex];
                    block.textureIndex = blockTextureIndex;

                    blockInstance.transform.position = new Vector3(x * blockSize, y * blockSize, 0);
                    blockInstance.transform.SetParent(transform, false);
                    blockInstance.name = block.blockName + " => " + x + ", " + y;

                    block.parentNetId = GetComponent<NetworkIdentity>().netId;

                    NetworkServer.Spawn(blockInstance);
                }

                resourceSpawned = false;
            }
        }

        for (int y = 0; y < mapHeight + 1; y++)
        {
            GameObject leftBorder = Instantiate(borderBlockPrefab);
            GameObject rightBorder = Instantiate(borderBlockPrefab);
            leftBorder.transform.position = new Vector3(-1 * blockSize, y * blockSize, 0);
            leftBorder.transform.SetParent(transform, false);
            leftBorder.name = "BORDER_BLOCK_LEFT";
            NetworkServer.Spawn(leftBorder);
            rightBorder.transform.position = new Vector3(mapWidth * blockSize, y * blockSize, 0);
            rightBorder.transform.SetParent(transform, false);
            rightBorder.name = "BORDER_BLOCK_RIGHT";
            Vector3 s = rightBorder.transform.localScale;
            rightBorder.transform.localScale = new Vector3(s.x * -1, s.y, s.z);

            NetworkServer.Spawn(rightBorder);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            GameObject endBlock = Instantiate(endBlockPrefab);
            endBlock.transform.position = new Vector3(x * blockSize, -1, 0);
            endBlock.transform.SetParent(transform, false);
            endBlock.name = "END_GAME_BLOCK";

            NetworkServer.Spawn(endBlock);
        }
    }
}