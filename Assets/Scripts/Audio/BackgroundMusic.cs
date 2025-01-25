using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private EventReference _adaptiveMusic;

    private EventInstance _musicEvent;

    private void Start()
    {
        WavesManager.Instance.OnNewWave += UpdateMusic;
        PlayerController.activePlayer.OnPlayerDeath += SwapToDeathMusic;

        _musicEvent.setParameterByName("death", 0);
        _musicEvent.setParameterByName("waves", 0);
        _musicEvent = RuntimeManager.CreateInstance(_adaptiveMusic);
        _musicEvent.start();
    }

    private void OnDestroy()
    {
        _musicEvent.release();
    }

    private void UpdateMusic()
    {
        int waveNumber = WavesManager.Instance.WaveNumber;
        if (waveNumber < 5) _musicEvent.setParameterByName("waves", waveNumber);
    }

    public void SwapToDeathMusic()
    {
        _musicEvent.setParameterByName("death", 1);
    }

}
