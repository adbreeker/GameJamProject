using System.Collections;
using FMODUnity;
using UnityEngine;

public class GumShieldBehavior : MonoBehaviour
{
    [SerializeField] ParticleSystem _explosionEffect;
    [SerializeField] EventReference _explosionSound;

    private void Start()
    {
        StartCoroutine(DestroyShieldAfterDeley(_explosionEffect.main.startDelay.constant));
    }

    IEnumerator DestroyShieldAfterDeley(float deley)
    {
        yield return new WaitForSeconds(deley);

        RuntimeManager.PlayOneShot(_explosionSound);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Shield collide with " + collision.gameObject.name);
    }
}
