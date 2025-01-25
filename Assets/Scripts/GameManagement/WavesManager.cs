using System.Collections;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }

    [ShowNativeProperty] public float WaveTimer { get; private set; }
    [ShowNativeProperty] public int WaveNumber { get; private set; }

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private EventReference _newWaveSound;

    private float _currentStartingTimerValue = 5f;
    private int _currentEnemySpawnNumber = 3;

    private void Awake()
    {
        CreateInstance();
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        WaveNumber = 0;
        WaveTimer = _currentStartingTimerValue;

        while (WaveTimer > 0)
        {
            WaveTimer -= Time.deltaTime;
            yield return null;
        }

        _currentStartingTimerValue = 45f;
        WaveNumber = 1;
        StartCoroutine(Waves());
    }

    private IEnumerator Waves()
    {
        RuntimeManager.PlayOneShot(_newWaveSound);
        SpawnEnemies();

        WaveTimer = _currentStartingTimerValue;

        while (WaveTimer > 0)
        {
            WaveTimer -= Time.deltaTime;
            yield return null;
        }

        _currentStartingTimerValue += 15f;
        WaveNumber += 1;
        _currentEnemySpawnNumber += 3;
        StartCoroutine(Waves());
    }

    private void SpawnEnemies()
    {
        for(int i = 0; i < _currentEnemySpawnNumber; i++)
        {
            //samplujemy nav mesh
            //spawnujemy tylko w odpowiedniej odleglosci od gracza
        }
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
