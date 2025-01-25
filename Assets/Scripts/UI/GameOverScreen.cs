using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TMP;
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private List<GameObject> _gameObjectsToTurnOff;
    [SerializeField] private EventReference _buttonSound;

    private bool _sceneIsReseting = false;

    private void Start()
    {
        PlayerController.activePlayer.OnPlayerDeath += ActivateGameOverScreen;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void ActivateGameOverScreen()
    {
        Time.timeScale = 0f;

        foreach (GameObject gameObject in _gameObjectsToTurnOff) gameObject.SetActive(false);
        _gameOverCanvas.gameObject.SetActive(true);
        _TMP.text = "You survived to Wave " +  WavesManager.Instance.WaveNumber + "!";
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResetScene()
    {
        if(_sceneIsReseting) return;

        _sceneIsReseting = true;
        RuntimeManager.PlayOneShot(_buttonSound);
        StartCoroutine(ResetAfterSound());
    }

    private IEnumerator ResetAfterSound()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
