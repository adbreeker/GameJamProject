using UnityEngine;
using UnityEngine.Events;

public class EntityBehavior : MonoBehaviour
{
    [SerializeField] UnityEvent _OnHitEvent;

    public void OnHit()
    {
        _OnHitEvent?.Invoke();
    }
}
