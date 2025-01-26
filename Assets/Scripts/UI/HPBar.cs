using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image _hpImage;

    private void Start()
    {
        _hpImage.fillAmount = 1;
        PlayerBehavior.activePlayer.OnPlayerHited += UpdateHealthBar;
        PlayerBehavior.activePlayer.OnPlayerHealed += UpdateHealthBar;
    }

    void UpdateHealthBar()
    {
        _hpImage.fillAmount = (float)PlayerBehavior.activePlayer.currentHealth / (float)PlayerBehavior.activePlayer.maxHealth;
    }
}
