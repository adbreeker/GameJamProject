using UnityEngine;

public class EnemyFaceTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform _visuals;

    private bool _shouldLookAtPlayer = true;

    void Update()
    {
        if(_shouldLookAtPlayer) LookAtPlayer();
    }

    public void TurnOffLookingAtPlayer()
    {
        _shouldLookAtPlayer = false;
    }

    private void LookAtPlayer()
    {
        Vector3 lookVector = PlayerController.activePlayer.transform.position - _visuals.position;
        lookVector.y = 0;

        Quaternion rotation = Quaternion.LookRotation(lookVector);
        _visuals.rotation = rotation;
    }
}
