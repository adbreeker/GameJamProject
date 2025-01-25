using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    [ShowNativeProperty] public EnemyState CurrentState { get; private set; }

    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public float EnemySpeed { get; private set; }
    [field: SerializeField] public RawImage MainSprite { get; private set; }
    [field: SerializeField] public Canvas SpritesCanvas { get; private set; }
    [Space]
    [SerializeField] private EnemyState _startingState;
    [Space]
    [SerializeField] private DamagedEnemyState _damagedState;
    [SerializeField] private DyingEnemyState _dyingState;
    [SerializeField] private StunnedEnemyState _stunnedState;

    private bool _changingStateDisabled = false;
    private int _enemyHP = 3;

    private void Awake()
    {
        InitializeState();
    }

    private void Update()
    {
        CurrentState.StateUpdate();
    }

    public void ChangeState(EnemyState givenState)
    {
        if (_changingStateDisabled)
        {
            Debug.LogWarning("Trying to change state but this option is disabled!");
            return;
        }

        if (CurrentState == givenState)
        {
            //Debug.LogWarning("Trying to change state to the one that is already active!");
            return;
        }

        CurrentState.ExitState();
        CurrentState.gameObject.SetActive(false);

        CurrentState = givenState;

        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    public void DisableChangingStates()
    {
        _changingStateDisabled = true;
    }

    public void HitEnemy()
    {
        if (_enemyHP > 0)
        {
            _enemyHP--;

            if (_enemyHP > 0) ChangeState(_damagedState);
            else ChangeState(_dyingState);
        }
    }
    public void StunEnemy()
    {
        if (CurrentState != _stunnedState) ChangeState(_stunnedState);
    }

    private void InitializeState()
    {
        CurrentState = _startingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }
}