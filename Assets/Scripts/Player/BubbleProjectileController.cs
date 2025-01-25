using UnityEngine;

public class BubbleProjectileController : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] GameObject _impactPrefab;
    [SerializeField] LayerMask _collisionLayers;

    public void ShootInDirection(Vector3 direction)
    {
        _rigidbody.AddForce(direction * 10f, ForceMode.Impulse);
        Debug.Log(direction + "  " + _rigidbody.linearVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if((_collisionLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("bubble collides with - " + other.gameObject.name);
            Instantiate(_impactPrefab,
                (transform.position - _rigidbody.linearVelocity.normalized*0.3f),
                Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
