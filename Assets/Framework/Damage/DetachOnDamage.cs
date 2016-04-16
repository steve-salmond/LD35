using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetachOnDamage : MonoBehaviour
{
    // Properties
    // -----------------------------------------------------

    /** Damageable object. */
    public Damageable Damageable;

    /** Set of objects that can be detached. */
    public GameObject[] Detachable;


    // Members
    // -----------------------------------------------------

    /** The set of undetached objects. */
    private List<GameObject> _detachable;

    /** Damage per object. */
    private float _damagePerObject;

    /** Next damage detach threshold. */
    private float _detachThreshold;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        _detachable = new List<GameObject>(Detachable);
        if (_detachable.Count > 0)
            _damagePerObject = Damageable.StartingHealth / _detachable.Count;

        _detachThreshold = Damageable.StartingHealth - _damagePerObject;

        Damageable.Damaged += OnDamaged;
	}


    // Private Methods
    // -----------------------------------------------------

    /** Handle damage event. */
    private void OnDamaged(Damageable d, Damager damager, float damage)
    {
        while (d.Health < _detachThreshold && CanDetach())
            Detach();
    }

    /** Determines if an object can be detached. */
    private bool CanDetach()
    {
        return _detachable.Count > 0;
    }

    /** Detaches an object. */
    private void Detach()
    {
        var n = _detachable.Count;
        if (n <= 0)
            return;

        // Debug.Log("Detaching a part!");

        var index = Random.Range(0, n);
        var go = _detachable[index];
        _detachable.RemoveAt(index);
        go.SetActive(false);

        _detachThreshold -= _damagePerObject;
    }

}
