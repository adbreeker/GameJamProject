using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class SpawningEnemyState : EnemyState
{
    [SerializeField] private ShootingWalkingEnemyState _walkingState;
    [SerializeField] private EventReference _spawningSound;

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

        yield return new WaitForSeconds(0.5f);
        MainSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        _stateMachine.ChangeState(_walkingState);
    }
}
