using NaughtyAttributes;
using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    [SerializeField] protected EnemyStateMachine _stateMachine;

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();

    [Button]
    protected void ChangeToThisState()
    {
        _stateMachine.ChangeState(this);
    }
}
