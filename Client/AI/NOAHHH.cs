using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading;

public class NOAHHH : MonoBehaviour
{
    /*
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public TextMeshProUGUI waveText;
    public float waveInterval = 5f;
    float currentWaveTime;
    public GameObject explosionPrefab;
    public HealthScr playerHealth;

    private int currentWave = 1;
    private int enemyCount;

    public GameObject winScreen;
    private void Start()
    {
        enemyCount = 5;
        winScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }
    private void Update()
    {
        currentWaveTime += Time.deltaTime;
        if (currentWaveTime > waveInterval)
        {
            currentWave++;
            if (currentWave > 5) { winScreen.SetActive(true); }
            waveText.text = "Wave " + currentWave;
            enemyCount++;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }
            currentWaveTime = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        if (enemyObject != null)
        {
            FlyingEnemyAI enemyAI = enemyObject.GetComponent<FlyingEnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.player = playerHealth.transform;
                enemyAI.explosionPrefab = explosionPrefab;
                enemyAI.playerHealth = playerHealth;
            }
            else
            {
                Debug.LogError("FlyingEnemyAI component not found on instantiated enemy.");
                Destroy(enemyObject);
            }
        }
        else
        {
            Debug.LogError("Failed to instantiate enemy.");
        }
    }
    */
}