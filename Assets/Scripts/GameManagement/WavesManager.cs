using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }

    public event Action OnNewWave;

    [ShowNativeProperty] public float WaveTimer { get; private set; }
    [ShowNativeProperty] public int WaveNumber { get; private set; }

    [SerializeField] private EventReference _newWaveSound;
    [SerializeField] private List<GameObject> _enemyPrefabs;
    [SerializeField] private float _maxSpawnDistanceFromPlayer;
    [SerializeField] private float _minSpawnDistanceFromPlayer;
    [Space]
    [SerializeField] private float _addedTimePerWave = 10f;
    [SerializeField] private int _addedEnemiesPerWave = 3;
    [SerializeField] private float _firstWaveTime = 15f;
    [SerializeField] private int _firstWaveEnemies = 3;
    [SerializeField] private float _timeBeforFirstWave = 5f;

    private float _currentStartingTimerValue;
    private int _currentEnemySpawnNumber;

    private void Awake()
    {
        CreateInstance();
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        WaveNumber = 0;
        WaveTimer = _timeBeforFirstWave;

        while (WaveTimer > 0)
        {
            WaveTimer -= Time.deltaTime;
            yield return null;
        }

        _currentEnemySpawnNumber = _firstWaveEnemies;
        _currentStartingTimerValue = _firstWaveTime;
        WaveNumber = 1;
        StartCoroutine(Waves());
    }

    private IEnumerator Waves()
    {
        OnNewWave?.Invoke();

        RuntimeManager.PlayOneShot(_newWaveSound);

        SpawnOneThirdEnemies();
        int timesSpawnedOneThird = 1;
        float spawnInterval = _currentStartingTimerValue / 3f;
        float currentSpawnInterval = spawnInterval;

        WaveTimer = _currentStartingTimerValue;

        while (WaveTimer > 0)
        {
            if (timesSpawnedOneThird < 3)
            {
                if (currentSpawnInterval > 0)
                {
                    currentSpawnInterval -= Time.deltaTime;
                }
                else
                {
                    timesSpawnedOneThird++;
                    currentSpawnInterval = spawnInterval;
                    SpawnOneThirdEnemies();
                }
            }

            WaveTimer -= Time.deltaTime;
            yield return null;
        }

        _currentStartingTimerValue += _addedTimePerWave;
        WaveNumber += 1;
        _currentEnemySpawnNumber += _addedEnemiesPerWave;
        StartCoroutine(Waves());
    }

    private void SpawnOneThirdEnemies()
    {
        for (int i = 0; i < _currentEnemySpawnNumber / 3; i++)
        {
            int randomChoice = UnityEngine.Random.Range(0, _enemyPrefabs.Count);
            GameObject spawnedEnemy = Instantiate(_enemyPrefabs[randomChoice]);

            float distanceToPlayer = 0f;
            Vector3 randomPos = Vector3.zero;
            Vector3 randomPosOnNavMesh = Vector3.zero;

            while (distanceToPlayer < _minSpawnDistanceFromPlayer)
            {
                randomPos = GetRandomPosition(
                    PlayerController.activePlayer.gameObject,
                    _minSpawnDistanceFromPlayer,
                    _maxSpawnDistanceFromPlayer);

                NavMesh.SamplePosition(randomPos, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
                randomPosOnNavMesh = hit.position;

                distanceToPlayer = Vector3.Distance(randomPosOnNavMesh, PlayerController.activePlayer.transform.position);
            }

            spawnedEnemy.transform.position = randomPosOnNavMesh;
        }
    }

    private Vector3 GetRandomPosition(GameObject origin, float minDistance, float maxDistance)
    {
        Vector3 originPos = origin.transform.position;
        Vector2 randomPoint2D = UnityEngine.Random.insideUnitCircle.normalized;

        float randomDistance = UnityEngine.Random.Range(minDistance, maxDistance);

        randomPoint2D *= randomDistance;
        return new Vector3(originPos.x + randomPoint2D.x, 0, originPos.z + randomPoint2D.y);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one WavesManager in the scene.");
        }
        Instance = this;
    }
}
