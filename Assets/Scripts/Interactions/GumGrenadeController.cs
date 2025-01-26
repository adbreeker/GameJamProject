using UnityEngine;

public class GumGrenadeController : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] LayerMask _collideMask;
    [SerializeField] GameObject _impactPrefab;

    float _explosionRadius;

    public void ThrowInDirection(Vector3 direction, float throwForce, float explosionRadius)
    {
        _rigidbody.AddForce(direction * throwForce, ForceMode.Impulse);
        _explosionRadius = explosionRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if((_collideMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            Instantiate(_impactPrefab, transform.position, Quaternion.identity).transform.localScale = Vector3.one * _explosionRadius;
            Collider[] enemies = Physics.OverlapSphere(transform.position, _explosionRadius, LayerMask.GetMask("Enemy"));
            foreach (Collider collider in enemies)
            {
                collider.transform.parent.parent.GetComponent<EnemyStateMachine>()?.StunEnemy();
            }
            Destroy(gameObject);
        }
    }
}
