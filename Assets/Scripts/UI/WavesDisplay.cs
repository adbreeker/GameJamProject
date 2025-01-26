using TMPro;
using UnityEngine;

public class WavesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TMP;

    private void Update()
    {
        if (WavesManager.Instance != null)
        {
            if (WavesManager.Instance.WaveNumber > 0)
            {
                _TMP.text = "Wave " + WavesManager.Instance.WaveNumber + "\n" +
                            Mathf.RoundToInt(WavesManager.Instance.WaveTimer);
            }
            else
            {
                _TMP.text = Mathf.RoundToInt(WavesManager.Instance.WaveTimer).ToString();
            }
        }
    }
}
