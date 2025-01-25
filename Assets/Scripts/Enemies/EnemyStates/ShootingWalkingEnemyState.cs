using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShootingWalkingEnemyState : EnemyState
{
    [SerializeField] private Transform _player; //later will be swapped for player singleton

    private Tween _rotTween;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        Agent.speed = _stateMachine.EnemySpeed;
        Agent.enabled = true;
        Agent.isStopped = false;
        StartCoroutine(WalkingAnimation());
    }

    public override void StateUpdate()
    {
        ShootPlayerIfSeen();
        FollowPlayer();
    }

    public override void ExitState()
    {
        Agent.isStopped = true;
        Agent.enabled = false;
        _rotTween.Kill();
        SpritesCanvas.transform.localRotation = Quaternion.identity;
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void ShootPlayerIfSeen()
    {
        //if player seen shoot
    }

    private void FollowPlayer()
    {
        Agent.SetDestination(_player.position);
    }

    private IEnumerator WalkingAnimation()
    {
        while (true)
        {
            _rotTween = SpritesCanvas.transform.DOLocalRotate(new Vector3(0, 0, 7f), 0.4f).SetEase(Ease.Linear);
            while (_rotTween.IsPlaying()) yield return null;

            _rotTween = SpritesCanvas.transform.DOLocalRotate(new Vector3(0, 0, -7f), 0.4f).SetEase(Ease.Linear);
            while (_rotTween.IsPlaying()) yield return null;
        }
    }
}
