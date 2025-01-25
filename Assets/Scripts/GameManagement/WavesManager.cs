using System.Collections;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }

    [ShowNativeProperty] public float WaveTimer { get; private set; }
    [ShowNativeProperty] public int WaveNumber { get; private set; }

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private EventReference _newWaveSound;
    [SerializeField] private EventReference _adaptiveMusic;
    [Space]
    [SerializeField] private float _addedTimePerWave = 10f;
    [SerializeField] private int _addedEnemiesPerWave = 3;
    [SerializeField] private float _firstWaveTime = 15f;
    [SerializeField] private int _firstWaveEnemies = 3;
    [SerializeField] private float _timeBeforFirstWave = 5f;

    private EventInstance _musicEvent;
    private float _currentStartingTimerValue;
    private int _currentEnemySpawnNumber;

    private void Awake()
    {
        CreateInstance();
        StartCoroutine(StartWaves());
    }

    private void OnDestroy()
    {
        _musicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicEvent.release();
    }

    public void SwapToDeathMusic()
    {
        _musicEvent.setParameterByName("death", 2);
    }

    private IEnumerator StartWaves()
    {
        _musicEvent = RuntimeManager.CreateInstance(_adaptiveMusic);
        _musicEvent.start();

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
        RuntimeManager.PlayOneShot(_newWaveSound);
        SpawnEnemies();

        WaveTimer = _currentStartingTimerValue;
        if (WaveNumber < 5) _musicEvent.setParameterByName("waves", WaveNumber);

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
