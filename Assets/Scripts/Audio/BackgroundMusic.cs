using System.Collections;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private EventReference _adaptiveMusic;
    [SerializeField] private EventReference _failSound;

    private EventInstance _musicEvent;

    private void Start()
    {
        WavesManager.Instance.OnNewWave += UpdateMusic;
        
        PlayerBehavior.activePlayer.OnPlayerDeath += () => StartCoroutine(FadeMusicAndPlayFailSound());

        _musicEvent = RuntimeManager.CreateInstance(_adaptiveMusic);
        _musicEvent.setParameterByName("waves", 1);
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

    private IEnumerator FadeMusicAndPlayFailSound()
    {
        RuntimeManager.PlayOneShot(_failSound);
        float timeToFadeOutMusic = 0.5f;

        _musicEvent.getVolume(out float volume);
        while (volume > 0)
        {
            AudioManager.Instance.SetInstanceVolume(_musicEvent, volume - (Time.unscaledDeltaTime / timeToFadeOutMusic));
            _musicEvent.getVolume(out volume);
            yield return null;
        }
    }

}
