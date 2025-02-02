using System;
using System.Collections;
using UnityEngine;

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

        if(currentItem == ItemType.NONE) { SpawnRandomItem(); }
        else { SpawnItem(currentItem); }
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

        SpawnItem(randomItem);
    }

    void SpawnItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.NONE:
                StartCoroutine(SpawnItemAfterDeley(UnityEngine.Random.Range(20f, 45f)));
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

    IEnumerator SpawnItemAfterDeley(float deley)
    {
        yield return new WaitForSeconds(deley);
        SpawnRandomItem();
    }

    public ItemType GetItem()
    {
        if(currentItem == ItemType.NONE) { return ItemType.NONE; }
        else
        {
            ItemType itemToReturn = currentItem;

            _currentIcon.SetActive(false);
            _currentIcon = null;
            currentItem = ItemType.NONE;
            StartCoroutine(SpawnItemAfterDeley(UnityEngine.Random.Range(60f, 90f)));

            return itemToReturn;
        }
    }

    private void LookAtPlayer(Transform iconToRotate)
    {
        Vector3 lookVector = PlayerController.activePlayer.transform.position - iconToRotate.position;
        lookVector.y = 0;

        Quaternion rotation = Quaternion.LookRotation(lookVector) * Quaternion.Euler(0f, 180f, 0f);
        iconToRotate.rotation = rotation;
    }
}
