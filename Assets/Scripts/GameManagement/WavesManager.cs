using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }

    public event Action OnNewWave;

    [ShowNativeProperty] public float WaveTimer { get; private set; }
    [ShowNativeProperty] public int WaveNumber { get; private set; }

    [SerializeField] private List<GameObject> _enemyPrefabs;
    [SerializeField] private EventReference _newWaveSound;
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
        SpawnEnemies();

        WaveTimer = _currentStartingTimerValue;

        while (WaveTimer > 0)
        {
            WaveTimer -= Time.deltaTime;
            yield return null;
        }

        _currentStartingTimerValue += _addedTimePerWave;
        WaveNumber += 1;
        _currentEnemySpawnNumber += _addedEnemiesPerWave;
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
