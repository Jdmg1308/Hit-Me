using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameEnemyManager : MonoBehaviour
{
    public GameManager GM;
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public int EnemiesLeftInWave = 0;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints;  // List of possible spawn points
    public GameObject enemyPrefab;
    public float SpawnDelay = 3f; // first wave spawn delay

    [Header("Wave Settings")]
    public List<int> waveConfigurations = new List<int> { 3, 4, 6 }; // List of enemy count per wave
    public int currentWave = 0;         // Current wave number
    private bool _WaveInProgress = false;

    public struct EnemyStats
    {
        public int MaxHealth;
        public float ChaseSpeed;
        public int PunchDamage;
    }
    [Header("Enemy Effects")]
    public EnemyStats enemyStats;
    public bool isDoubleDamage = false;
    public int extraEnemySpawns = 0;

    [Header("FX")]
    // death fx
    public GameObject EnemyOnScreenDeathPrefab;
    public GameObject EnemyOffScreenDeathPrefab;
    private SpriteRenderer _OffScreenDeathRend;
    private float _OffScreenSpriteWidth;
    private float _OffScreenSpriteHeight;

    public bool hasSpawned = false;
    public bool shouldSpawn = false;

    void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _OffScreenDeathRend = EnemyOffScreenDeathPrefab.GetComponent<SpriteRenderer>();

        var bounds = _OffScreenDeathRend.bounds;
        _OffScreenSpriteWidth = bounds.size.x / 2f;
        _OffScreenSpriteHeight = bounds.size.y / 2f;

        var enemyScript = enemyPrefab.GetComponent<Enemy>();
        enemyStats = new EnemyStats();
        enemyStats.MaxHealth = enemyScript.MaxHealth;
        enemyStats.ChaseSpeed = enemyScript.ChaseSpeed;
        enemyStats.PunchDamage = enemyScript.PunchDamage;
    }

    void Update()
    {
        // Check if there are more waves left
        if (currentWave <= waveConfigurations.Count && !_WaveInProgress && shouldSpawn)
        {
            int enemiesToSpawn = 0;
            if (currentWave < waveConfigurations.Count)
                enemiesToSpawn = waveConfigurations[currentWave];
            // If no enemies are left, spawn a new wave
            if (EnemiesLeftInWave == 0 && !hasSpawned)
            {
                if (currentWave < waveConfigurations.Count)
                    StartCoroutine(StartWaves(enemiesToSpawn));
                ++currentWave;
            }
        }

        // testing methods
        if (Input.GetKeyDown(KeyCode.T))
        {
            // SpawnExtraEnemies(5);
            // DestroyExtraEnemies(5);
            // HealEnemies(15);
            // DoubleDamageTimer(10f);
            // SpawnHallucinationClones(1);
        }
    }

    public void ResetWaves()
    {
        currentWave = 0;
        EnemiesLeftInWave = 0;
        hasSpawned = false;
        _WaveInProgress = false;
    }

    public IEnumerator StartWaves(int enemiesToSpawn)
    {
        _WaveInProgress = true;
        yield return new WaitForSeconds(SpawnDelay);
        SpawnWave(enemiesToSpawn);
    }

    // this exists to handle damage mods
    public void Damage(Enemy enemy, int damage, int hitstun)
    {
        if (isDoubleDamage)
            enemy.DamageHelper(2 * damage, hitstun);
        else
            enemy.DamageHelper(damage, hitstun);
    }

    public void Death(GameObject enemy)
    {
        if (enemy != null)
        {
            EnemiesLeftInWave -= 1;
            spawnedEnemies.Remove(enemy);
            StartCoroutine(WaitForSpawn(enemy));

            // Check if all enemies are dead
            if (EnemiesLeftInWave == 0)
                _WaveInProgress = false; // Allow the next wave to start
        }
    }

    // difficulty = more enemies + enemy hp + dmg + speed
    public void SetDifficulty(int level)
    {

        // level 1, 2, 3
        if (level == 2)
        {
            enemyStats.MaxHealth += 50; // 150
            enemyStats.ChaseSpeed += 1f; // 3
            enemyStats.PunchDamage += 3; // 8
            extraEnemySpawns = 1;
        }
        else if (level == 3)
        {
            enemyStats.MaxHealth += 80; // 180
            enemyStats.ChaseSpeed += 1.5f; // 3.5
            enemyStats.PunchDamage += 5; // 10
            extraEnemySpawns = 2;
        }
    }

    #region Card Effects
    public void BuffEnemies(int ExtraHealth, int ExtraDamage, float ExtraSpeed)
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Enemy enemyRef = enemy.GetComponent<Enemy>();
                // Code to execute for each item
                enemyRef.MaxHealth += ExtraHealth;
                enemyRef.CurrentHealth += ExtraHealth;
                enemyRef.ChaseSpeed += ExtraSpeed;
                enemyRef.PunchDamage += ExtraDamage;
            }
        }

        enemyStats.MaxHealth += ExtraHealth;
        enemyStats.ChaseSpeed += ExtraSpeed;
        enemyStats.PunchDamage += ExtraDamage;
    }

    // spawn x enemies, chooses random spawn points from list
    public void SpawnExtraEnemies(int enemiesToSpawn)
    {
        // Choose a random number of spawn points, capped by the total spawn points available
        int randomSpawnPoints = Mathf.Min(enemiesToSpawn, spawnPoints.Count);
        List<Transform> selectedSpawnPoints = GetRandomSpawnPoints(randomSpawnPoints);

        // Spawn enemies equally spread out among the selected spawn points
        SpawnEnemies(enemiesToSpawn, selectedSpawnPoints);
    }

    // destroy x num of enemies from existing list
    public void DestroyExtraEnemies(int enemiesToDestroy)
    {
        // Make sure we do not try to destroy more enemies than we have
        enemiesToDestroy = Mathf.Min(enemiesToDestroy, spawnedEnemies.Count);

        // Loop through the specified number of enemies to destroy
        for (int i = 0; i < enemiesToDestroy; i++)
        {
            GameObject enemy = spawnedEnemies[0];
            Death(enemy);
        }
    }

    // heal all enemies
    public void HealEnemies(int healAmount)
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            Enemy enemy = spawnedEnemies[i].GetComponent<Enemy>();
            enemy.CurrentHealth += healAmount;
            //added cap for HP at max HP - angela 
            if (enemy.CurrentHealth > enemy.MaxHealth)
                enemy.CurrentHealth = enemy.MaxHealth;
        }
    }

    // temporarily all enemies take double damage
    public void DoubleDamageTimer(float timeAmount)
    {
        StartCoroutine(DoubleDamageTimerHelper(timeAmount));
    }

    private IEnumerator DoubleDamageTimerHelper(float timeAmount)
    {
        float currentTime = 0f;
        isDoubleDamage = true;
        while (currentTime < timeAmount)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        isDoubleDamage = false;
    }

    // spawns clones of each enemy beside them
    public void SpawnHallucinationClones(int health)
    {
        foreach (GameObject t in spawnedEnemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, t.transform.position, Quaternion.identity);
            Enemy enemyRef = newEnemy.GetComponent<Enemy>();
            enemyRef.Player = GM.Player;
            enemyRef.GameEnemyManager = this;

            // modded stats for hallucination clones
            enemyRef.PunchDamage = 0;
            enemyRef.MaxHealth = health;
            enemyRef.CurrentHealth = health;
            enemyRef.initialSpawnDelay = 0f;
        }
    }
    #endregion

    #region Spawn Helpers
    private void SpawnWave(int enemiesToSpawn)
    {
        hasSpawned = true;
        int amount = enemiesToSpawn + extraEnemySpawns;
        SpawnExtraEnemies(amount);
        hasSpawned = false;
    }

    private List<Transform> GetRandomSpawnPoints(int count)
    {
        // Create a temporary list to avoid modifying the original spawn points list
        List<Transform> tempSpawnPoints = new List<Transform>(spawnPoints);

        // List to hold the selected spawn points
        List<Transform> selectedPoints = new List<Transform>();

        // Randomly select the requested number of spawn points
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempSpawnPoints.Count);
            selectedPoints.Add(tempSpawnPoints[randomIndex]);
            tempSpawnPoints.RemoveAt(randomIndex); // Remove to avoid duplicates
        }

        return selectedPoints;
    }

    // spawn x number of enemies using a list of spawn points
    private void SpawnEnemies(int totalEnemies, List<Transform> spawnPoints)
    {
        int enemiesPerPoint = totalEnemies / spawnPoints.Count;   // Divide enemies equally
        int remainingEnemies = totalEnemies % spawnPoints.Count;  // Handle any leftover enemies

        foreach (Transform spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerPoint; i++)
            {
                SpawnEnemy(spawnPoint);
            }

            // Spawn one extra enemy if there are remaining enemies
            if (remainingEnemies > 0)
            {
                SpawnEnemy(spawnPoint);
                remainingEnemies--;
            }
        }

        // Update total number of enemies after spawning
        EnemiesLeftInWave += totalEnemies;
    }

    private void SpawnEnemy(Transform spawnPoint)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        Enemy enemyRef = newEnemy.GetComponent<Enemy>();
        enemyRef.Player = GM.Player;
        enemyRef.GameEnemyManager = this;

        // adding current stats
        enemyRef.MaxHealth = enemyStats.MaxHealth;
        enemyRef.ChaseSpeed = enemyStats.ChaseSpeed;
        enemyRef.PunchDamage = enemyStats.PunchDamage;

        Debug.Log(newEnemy.name);
        Debug.Log(enemyPrefab);
        Debug.Log(spawnPoint.position);
        spawnedEnemies.Add(newEnemy);
    }
    #endregion

    #region FX
    IEnumerator WaitForSpawn(GameObject enemy)
    {
        while (Time.timeScale != 1.0f)
        { // wait until after hit stop fx
            yield return null;
        }
        Destroy(enemy);
        DeathIndication(enemy);
    }

    // if enemy is offscreen, then spawn offscreen arrow pointing at them, otherwise normal death fx
    private void DeathIndication(GameObject enemy)
    {
        Camera _Camera = GM.Camera.GetComponent<Camera>();
        Vector3 screenPos = _Camera.WorldToViewportPoint(enemy.transform.position);
        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= 1 || screenPos.y <= 0 || screenPos.y >= 1;

        if (isOffScreen)
        {
            GameObject indicator = Instantiate(EnemyOffScreenDeathPrefab);
            Vector3 spriteSizeInViewPort = _Camera.WorldToViewportPoint(new Vector3(_OffScreenSpriteWidth, _OffScreenSpriteHeight, 0))
                - _Camera.WorldToViewportPoint(Vector3.zero);

            screenPos.x = Mathf.Clamp(screenPos.x, spriteSizeInViewPort.x, 1 - spriteSizeInViewPort.x);
            screenPos.y = Mathf.Clamp(screenPos.y, spriteSizeInViewPort.y, 1 - spriteSizeInViewPort.y);

            Vector3 worldPosition = _Camera.ViewportToWorldPoint(screenPos);
            worldPosition.z = 0;
            indicator.transform.position = worldPosition;

            Vector3 direction = enemy.transform.position - indicator.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (direction.x < 0)
            {
                Vector3 scale = indicator.transform.localScale;
                scale.y = scale.y * -1;
                indicator.transform.localScale = scale;
            }
        }
        else
        {
            Instantiate(EnemyOnScreenDeathPrefab, enemy.transform.position, Quaternion.identity);
        }
    }
    #endregion
}
