using System.Collections;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class ShootingWalkingEnemyState : EnemyState
{
    [SerializeField] private EventReference _shootingSound;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private BubbleProjectileController _bubbleProjectilePrefab;

    private Tween _rotTween;
    private float _shootingCooldown = 0f;

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
        ManageShootingCooldown();
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
        if (_shootingCooldown > 0) return;

        Vector3 playerPos = PlayerController.activePlayer.transform.position;
        Vector3 direction = (playerPos - _gunPoint.position).normalized;
        float distance = Vector3.Distance(_gunPoint.position, playerPos);

        if (Physics.Raycast(_gunPoint.position, direction, out RaycastHit hit, distance, _obstacleLayer))
        {
            return;
        }

        GameObject bubble = Instantiate(_bubbleProjectilePrefab.gameObject, _gunPoint.position, Quaternion.identity);
        bubble.GetComponent<BubbleProjectileController>().ShootInDirection(_gunPoint.forward, 7.5f);

        _shootingCooldown = 0.5f;
    }

    private void ManageShootingCooldown()
    {
        if (_shootingCooldown > 0) _shootingCooldown -= Time.deltaTime;
    }

    private void FollowPlayer()
    {
        Agent.SetDestination(PlayerController.activePlayer.transform.position);
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
