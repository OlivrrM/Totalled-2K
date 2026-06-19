using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public GameObject currentAmbiencePrefab; // Dust particle prefab
    public float chunkSize = 20f; // Size of each chunk
    public int renderDistance = 2; // How many chunks around the player are active
    public int poolSize = 25; // Maximum number of pooled objects

    private Dictionary<Vector2Int, AmbienceChunk> activeChunks = new Dictionary<Vector2Int, AmbienceChunk>();
    private Queue<AmbienceChunk> chunkPool = new Queue<AmbienceChunk>();

    class AmbienceChunk
    {
        public GameObject go;
        public ParticleSystem particleSystem;
    }
    private void Awake()
    {
        Cache.ambienceManager = this;
    }
    void Start()
    {
        // Initialize the object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject chunk = Instantiate(currentAmbiencePrefab);
            AmbienceChunk aChunk = new AmbienceChunk
            {
                go = chunk,
                particleSystem = chunk.GetComponent<ParticleSystem>()
            };
            chunk.SetActive(false);
            chunkPool.Enqueue(aChunk);
        }
    }

    void Update()
    {
        Vector2Int playerChunk = GetChunkPosition(Cache.surfCharacter.transform.position);
        LoadChunks(playerChunk);
        UnloadDistantChunks(playerChunk);
    }

    Vector2Int GetChunkPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.z / chunkSize)
        );
    }

    void LoadChunks(Vector2Int playerChunk)
    {
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(playerChunk.x + x, playerChunk.y + z);

                if (!activeChunks.ContainsKey(chunkPos))
                {
                    Vector3 spawnPosition = new Vector3(chunkPos.x * chunkSize, Cache.surfCharacter.transform.position.y, chunkPos.y * chunkSize);
                    AmbienceChunk chunk = GetPooledChunk();
                    chunk.go.transform.position = spawnPosition;
                    chunk.go.SetActive(true);
                    activeChunks.Add(chunkPos, chunk);
                }
            }
        }
    }

    void UnloadDistantChunks(Vector2Int playerChunk)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var chunk in activeChunks)
        {
            if (Mathf.Abs(chunk.Key.x - playerChunk.x) > renderDistance || Mathf.Abs(chunk.Key.y - playerChunk.y) > renderDistance)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (var chunkPos in chunksToRemove)
        {
            AmbienceChunk chunk = activeChunks[chunkPos];
            chunk.go.SetActive(false); // Disable instead of destroying
            chunkPool.Enqueue(chunk); // Reuse it later
            activeChunks.Remove(chunkPos);
        }
    }
    public void DisableChunkParticleSystems() // These dont work probably the controller is at fault. It looks fine with dust indoors
    {
        /*foreach (AmbienceChunk aChunk in chunkPool){
            aChunk.particleSystem.Stop();
        }*/
    }
    public void EnableChunkParticleSystems()
    {
        /*foreach (AmbienceChunk aChunk in chunkPool){
            aChunk.particleSystem.Play();
        }*/
    }

    AmbienceChunk GetPooledChunk()
    {
        if (chunkPool.Count > 0)
        {
            return chunkPool.Dequeue();
        }
        else
        {
            // If pool is empty, create a new chunk (failsafe)
            GameObject chunk = Instantiate(currentAmbiencePrefab);
            AmbienceChunk aChunk = new AmbienceChunk{
                go = chunk,
                particleSystem = chunk.GetComponent<ParticleSystem>()
            };
            return aChunk;
        }
    }
}