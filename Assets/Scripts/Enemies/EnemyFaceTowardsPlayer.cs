using UnityEngine;

public class EnemyFaceTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform _visuals;
    [SerializeField] private Transform _player; //later will be swapped for player singleton

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
        Vector3 lookVector = _player.position - _visuals.position;
        lookVector.y = 0;

        Quaternion rotation = Quaternion.LookRotation(lookVector);
        _visuals.rotation = rotation;
    }
}
