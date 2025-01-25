using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DamagedEnemyState : EnemyState
{
    [SerializeField] private ShootingWalkingEnemyState _walkingState;
    [SerializeField] private RawImage _damagedSprite;
    [SerializeField] private EventReference _damagedSound;

    private float _animationDuration = 0.5f;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        RuntimeManager.PlayOneShotAttached(_damagedSound, _stateMachine.gameObject);

        MainSprite.gameObject.SetActive(false);
        _damagedSprite.gameObject.SetActive(true);

        StartCoroutine(DamagedAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        MainSprite.gameObject.SetActive(true);
        _damagedSprite.gameObject.SetActive(false);

        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator DamagedAnimation()
    {
        yield return new WaitForSeconds(_animationDuration);
        _stateMachine.ChangeState(_walkingState);
    }
}
