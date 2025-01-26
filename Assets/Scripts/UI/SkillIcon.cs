using System.Collections.Generic;
using UnityEngine;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] GameObject _gumGrenadeIcon;
    [SerializeField] GameObject _gumShieldIcon;

    ItemType _currentIconType;
    GameObject _currentIconObject;
    List<GameObject> _allIcons;

    private void Start()
    {
        _currentIconType = ItemType.NONE;
        _currentIconObject = null;

        _allIcons = new()
        {
            _gumGrenadeIcon,
            _gumShieldIcon
        };

        TurnOffAllIconsExcept(null);

        PlayerController.activePlayer.OnPlayerPickUpItem += TurnOnIcon;
        PlayerController.activePlayer.OnPlayerUseItem += TurnOffIcon;
    }

    void TurnOnIcon(ItemType type)
    {
        switch (type) 
        {
            case ItemType.NONE:
                Debug.LogWarning("Player pick up item none?!");
                return;
            case ItemType.BUBBLE_TEA:
                return;
            case ItemType.GUM_GRENADE:
                _currentIconType = type;
                _currentIconObject = _gumGrenadeIcon;
                break;
            case ItemType.GUM_SHIELD:
                _currentIconType = type;
                _currentIconObject = _gumShieldIcon;
                break;
        }

        TurnOffAllIconsExcept(_currentIconObject);
        _currentIconObject.SetActive(true);
    }

    void TurnOffIcon(ItemType type)
    {
        if(_currentIconType == type)
        {
            _currentIconObject?.SetActive(false);
            _currentIconObject = null;
            _currentIconType = ItemType.NONE;
        }
    }

    void TurnOffAllIconsExcept(GameObject iconExcept)
    {
        foreach(GameObject icon in _allIcons) 
        {
            if(icon != iconExcept)
            {
                icon.SetActive(false);
            }
        }
    }
}
