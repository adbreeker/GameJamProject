using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StunnedEnemyState : EnemyState
{
    [SerializeField] private ShootingWalkingEnemyState _walkingState;
    [SerializeField] private RawImage _stunnedOverlay;

    private float _animationDuration = 2f;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stunnedOverlay.gameObject.SetActive(true);
        StartCoroutine(StunAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        _stunnedOverlay.gameObject.SetActive(false);
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator StunAnimation()
    {
        yield return new WaitForSeconds(_animationDuration);
        _stateMachine.ChangeState(_walkingState);
    }
}
