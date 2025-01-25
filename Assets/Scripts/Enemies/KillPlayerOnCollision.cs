using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class KillPlayerOnCollision : MonoBehaviour
{
    [SerializeField, Layer] private string _playerLayer;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer(_playerLayer))
        {
            collision.gameObject.GetComponent<PlayerController>().OnPlayerDeath?.Invoke();
        }
    }
}
