using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOnTrigger : MonoBehaviour 
{

    // Properties
    // -----------------------------------------------------

    /** Whether to destroy this object on trigger. */
    public bool DestroySelf = false;

    /** Whether to destroy the other object on trigger. */
    public bool DestroyOther = true;

    /** Optional Damager to notify when destruction occurs. */
    public Damager Damager;


    // Members
    // -----------------------------------------------------

    /** Objects to ignore when they enter the trigger. */
    private List<GameObject> _ignored = new List<GameObject>();


    // Unity Methods
    // -----------------------------------------------------

    /** Called when object is enabled. */
    private void OnEnable()
    {
        // Reset ignored list.
        _ignored.Clear();
    }

    /** Handle an object entering the trigger. */
    private void OnTriggerEnter(Collider other)
	{
        // Check if we should ignore this object.
        if (_ignored.Contains(other.gameObject))
            return;

        if (DestroyOther)
            Cleanup(other.gameObject);
        if (DestroySelf)
            Cleanup(gameObject);
	}


    // Public Methods
    // -----------------------------------------------------

    /** Add an object to the list of ignored objects. */
    public void AddIgnored(GameObject go)
    { _ignored.Add(go); }

    /** Remove an object from the list of ignored objects. */
    public void RemoveIgnored(GameObject go)
    { _ignored.Remove(go); }


    // Private Methods
    // -----------------------------------------------------

    /** Cleans up the given object. */
    private void Cleanup(GameObject go)
    {
        var damageable = go.GetComponent<Damageable>();
        if (damageable != null)
            damageable.Kill(Damager);
        else
            ObjectPool.Cleanup(go);
    }

}
