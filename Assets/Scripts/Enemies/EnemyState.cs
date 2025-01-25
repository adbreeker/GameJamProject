using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyState : MonoBehaviour
{
    [SerializeField] protected EnemyStateMachine _stateMachine;

    protected NavMeshAgent Agent => _stateMachine.Agent;
    protected RawImage MainSprite => _stateMachine.MainSprite;
    protected Canvas SpritesCanvas => _stateMachine.SpritesCanvas;

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();

    [Button]
    protected void ChangeToThisState()
    {
        _stateMachine.ChangeState(this);
    }
}
