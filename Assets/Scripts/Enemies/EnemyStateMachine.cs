using NaughtyAttributes;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    //facing player
    //shooting
    //state machine


    //spawning state
    //afk state
    //stunned state
    //shooting state
    //coming state

    [ShowNativeProperty] public EnemyState CurrentState { get; private set; }

    [SerializeField] private EnemyState _startingState;

    private bool _changingStateDisabled = false;
    private int _enemyHP = 3;

    private void Update()
    {
        CurrentState.StateUpdate();
    }

    public void DisableChangingStates()
    {
        _changingStateDisabled = true;
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
            Debug.LogWarning("Trying to change state to the one that is already active!");
            return;
        }

        CurrentState.ExitState();
        CurrentState.gameObject.SetActive(false);

        CurrentState = givenState;

        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }

    private void InitializeState()
    {
        CurrentState = _startingState;
        CurrentState.gameObject.SetActive(true);
        CurrentState.EnterState();
    }
}