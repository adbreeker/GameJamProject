using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image _hpImage;

    private void Update()
    {
        _hpImage.fillAmount = 0.75f;
    }
}
