using System.Collections;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField, Scene] private string _gameplayScene;
    [SerializeField] private EventReference _buttonSound;

    private bool _scenIsChanging = false;

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
