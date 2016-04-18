using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{

    public LayerMask PickupMask;

    public GameObject PickupEffectPrefab;

    private bool _used;

    private void OnEnable()
    {
        _used = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((PickupMask.value & 1 << other.gameObject.layer) == 0)
            return;

        Use(other.gameObject);
    }

    public virtual bool Use(GameObject other)
    {
        if (_used)
            return false;

        _used = true;
        ObjectPool.GetAt(PickupEffectPrefab, transform, false);
        ObjectPool.Cleanup(gameObject);

        return true;
    }

}
