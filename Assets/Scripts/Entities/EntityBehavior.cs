using UnityEngine;
using UnityEngine.Events;

public class EntityBehavior : MonoBehaviour
{
    [SerializeField] UnityEvent _OnHit;

    public virtual void HitEntity()
    {
        _OnHit?.Invoke();
    }
}
