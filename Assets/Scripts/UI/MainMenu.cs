using System.Collections;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField, Scene] private string _gameplayScene;
    [SerializeField] private EventReference _buttonSound;
    [SerializeField] private EventReference _menuMusic;

    private bool _scenIsChanging = false;
    private EventInstance _musicEvent;

    private void Start()
    {
        _musicEvent = RuntimeManager.CreateInstance(_menuMusic);
        _musicEvent.start();
    }

    private void OnDestroy()
    {
        _musicEvent.release();
        FmodBuses.Master.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ChangeScene()
    {
        if (_scenIsChanging) return;

        _scenIsChanging = true;
        RuntimeManager.PlayOneShot(_buttonSound);
        StartCoroutine(ChangeAfterSound());
    }

    private IEnumerator ChangeAfterSound()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(_gameplayScene);
    }
}
