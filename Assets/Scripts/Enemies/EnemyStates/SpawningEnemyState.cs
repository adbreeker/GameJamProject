using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class SpawningEnemyState : EnemyState
{
    [SerializeField] private ShootingWalkingEnemyState _walkingState;
    [SerializeField] private EventReference _spawningSound;

    private float _animationDuration = 1f;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        StartCoroutine(SpawningAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator SpawningAnimation()
    {
        AudioManager.Instance.PlayOneShotSpatialized(_spawningSound, _stateMachine.transform);
        yield return new WaitForSeconds(_animationDuration);
        //some kind of particle
        _stateMachine.ChangeState(_walkingState);
    }
}
