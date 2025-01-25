using System.Collections;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class DyingEnemyState : EnemyState
{
    [SerializeField] private EnemyFaceTowardsPlayer _faceTowardsPlayer;
    [SerializeField] private Collider _collider;
    [SerializeField] private RawImage _dyingSprite;
    [SerializeField] private EventReference _dyingSound;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        AudioManager.Instance.PlayOneShotSpatialized(_dyingSound, _stateMachine.transform);

        MainSprite.gameObject.SetActive(false);
        _dyingSprite.gameObject.SetActive(true);

        _faceTowardsPlayer.TurnOffLookingAtPlayer();
        _collider.gameObject.SetActive(false);
        _stateMachine.DisableChangingStates();
        StartCoroutine(DyingAnimation());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator DyingAnimation()
    {
        Tween rotTween = SpritesCanvas.transform.DOLocalRotate(new Vector3(-90, 0, 0), 1.5f).SetEase(Ease.Linear);
        Tween colorTween = _dyingSprite.DOColor(Color.red, 0.75f);
        while (rotTween.IsPlaying() && colorTween.IsPlaying()) yield return null;


        Tween fadeTween = _dyingSprite.DOFade(0, 0.2f);
        while (fadeTween.IsPlaying()) yield return null;

        Destroy(_stateMachine.gameObject);
    }
}
