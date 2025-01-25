using System;
using UnityEngine;

public class PlayerBehavior : EntityBehavior
{
    [field: Header("Statistics:")]
    [field: SerializeField] public int maxHealth { get; private set; }
    [field: SerializeField] public int currentHealth { get; private set; }
    [field: SerializeField] public bool isPlayerDead { get; private set; } = false;

    [Header("Special effects:")]
    [SerializeField] GameObject _healingPrefab;

    //listeners
    public Action OnPlayerDeath;
    public Action OnPlayerHited;
    public Action OnPlayerHealed;

    public static PlayerBehavior activePlayer;

    private void Awake()
    {
        if (activePlayer != null && activePlayer != this)
        {
            Destroy(gameObject);
        }
        activePlayer = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void HitPlayer(int damage)
    {
        if(isPlayerDead) { return; }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isPlayerDead = true;
            OnPlayerDeath?.Invoke();
        }
        OnPlayerHited?.Invoke();
    }

    public void HealPlayer(int heal)
    {
        if (isPlayerDead) { return; }

        currentHealth += heal;
        Instantiate(_healingPrefab, transform);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnPlayerHealed?.Invoke();
    }
}
