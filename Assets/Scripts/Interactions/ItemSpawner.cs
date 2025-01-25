using System;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    NONE,
    BUBBLE_TEA,
    GUM_GRENADE,
    GUM_SHIELD
}

public class ItemSpawner : MonoBehaviour
{
    [field: Header("Spawned item:")]
    [field: SerializeField] public ItemType currentItem { get; private set; }

    [Header("Items icons:")]
    [SerializeField] GameObject _bubbleTeaIcon;
    [SerializeField] GameObject _gumGrenadeIcon;
    [SerializeField] GameObject _gumShieldIcon;

    GameObject _currentIcon;

    private void Start()
    {
        _bubbleTeaIcon.SetActive(false);
        _gumGrenadeIcon.SetActive(false);
        _gumShieldIcon.SetActive(false);

        SpawnRandomItem();
    }

    private void FixedUpdate()
    {
        if(_currentIcon != null)
        {
            LookAtPlayer(_currentIcon.transform);
        }
    }

    void SpawnRandomItem()
    {
        Array items = Enum.GetValues(typeof(ItemType));
        ItemType randomItem = (ItemType)items.GetValue(UnityEngine.Random.Range(0, items.Length));

        currentItem = randomItem;

        switch(currentItem)
        {
            case ItemType.NONE:
                break;
            case ItemType.BUBBLE_TEA:
                _bubbleTeaIcon.SetActive(true);
                _currentIcon = _bubbleTeaIcon;
                break;
            case ItemType.GUM_GRENADE:
                _gumGrenadeIcon.SetActive(true);
                _currentIcon = _gumGrenadeIcon;
                break;
            case ItemType.GUM_SHIELD:
                _gumShieldIcon.SetActive(true);
                _currentIcon = _gumShieldIcon;
                break;
        }
    }

    public void GetItem()
    {
        if(currentItem == ItemType.NONE) { return; }
        else
        {
            if (currentItem == ItemType.BUBBLE_TEA) { PlayerBehavior.activePlayer.HealPlayer(4); }
            if (currentItem == ItemType.GUM_GRENADE) { }
            if (currentItem == ItemType.GUM_SHIELD) { }

            _currentIcon.SetActive(false);
            _currentIcon = null;
            currentItem = ItemType.NONE;
        }
    }

    private void LookAtPlayer(Transform iconToRotate)
    {
        Vector3 lookVector = PlayerController.activePlayer.transform.position - iconToRotate.position;
        lookVector.y = 0;

        Quaternion rotation = Quaternion.LookRotation(lookVector);
        iconToRotate.rotation = rotation;
    }
}
