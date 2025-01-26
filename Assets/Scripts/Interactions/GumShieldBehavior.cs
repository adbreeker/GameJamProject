using System.Collections;
using UnityEngine;

public class GumShieldBehavior : MonoBehaviour
{
    [SerializeField] ParticleSystem _explosionEffect;

    private void Start()
    {
        StartCoroutine(DestroyShieldAfterDeley(_explosionEffect.main.startDelay.constant));
    }

    IEnumerator DestroyShieldAfterDeley(float deley)
    {
        yield return new WaitForSeconds(deley);
        Debug.Log("wybucham");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Shield collide with " + collision.gameObject.name);
    }
}
