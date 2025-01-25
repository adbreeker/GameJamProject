using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TMP;
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private WavesDisplay _wavesDisplay;

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
        _wavesDisplay.gameObject.SetActive(false);
        _gameOverCanvas.gameObject.SetActive(true);
        _TMP.text = "You survived to Wave " +  WavesManager.Instance.WaveNumber + "!";
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
