using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class BubbleProjectileController : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] GameObject _impactPrefab;
    [SerializeField] LayerMask _collisionLayers;
    [SerializeField, Layer] string _entityTarget;
    [SerializeField] EventReference _bubbleHitSound;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void ShootInDirection(Vector3 direction, float force = 7.5f)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        //Debug.Log(direction + "  " + _rigidbody.linearVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if((_collisionLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            //Debug.Log("bubble collides with - " + other.gameObject.name);
            GameObject spawnedEffect = Instantiate(_impactPrefab,
                (transform.position - _rigidbody.linearVelocity.normalized*0.3f),
                Quaternion.identity);

            if (other.gameObject.layer == LayerMask.NameToLayer(_entityTarget))
            {
                other.gameObject.GetComponent<EntityBehavior>().HitEntity();
            }
            else
            {
                AudioManager.Instance.PlayOneShotSpatialized(_bubbleHitSound, spawnedEffect.transform);
            }

            Destroy(gameObject);
        }
    }
}
