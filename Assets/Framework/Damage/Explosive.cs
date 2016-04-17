using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosive : MonoBehaviour 
{

    // Properties
    // -----------------------------------------------------

    /** Optional Damager to notify when explosive damage is applied. */
    public Damager Damager;

    /** Maximum damage to apply. */
    public float Damage;

    /** Layer mask governing what objects can be damaged. */
    public LayerMask DamageMask;

    /** Layer mask governing what objects block explosions. */
    public LayerMask BlockerMask;

    /** Explosive radius. */
    public float Radius;

    /** Damage falloff curve (normalized: 0 = at origin, 1 = at max. radius). */
    public AnimationCurve DamageFalloffCurve;

    /** Explosive force to apply. */
    public float Force;

    /** Optional upwards modifier to lift objects. */
    public float ForceUpwardsModifier = 0;

    /** Explosive delay. */
    public float Delay;


    // Members
    // -----------------------------------------------------

    /** Objects to ignore collisions. */
    private List<GameObject> _ignored = new List<GameObject>();



    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void OnEnable() 
	{
        if (!Damager)
            Damager = GetComponent<Damager>();

        StartCoroutine(ExplodeRoutine());
	}

    /** Disabling. */
    private void OnDisable()
    {
        _ignored.Clear();
        StopAllCoroutines();
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

    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(Delay);

        // Look for nearby objects to damage.
        var origin = transform.position;
        var colliders = Physics.OverlapSphere(origin, Radius, DamageMask);
        foreach (var collider in colliders)
        {
            // Perform a ray cast to see if explosion is blocked.
            var p = collider.transform.position;

            /*
            Debug.Log("Trying to damage: " + collider.gameObject.name);
            if (Physics.Raycast(origin, p - origin, Radius, BlockerMask))
            {
                Debug.Log("Damage was blocked for: " + collider.gameObject.name);
                continue;
            }
            */

            // Check if we should ignore this object.
            if (_ignored.Contains(collider.gameObject))
                continue;

            // Debug.DrawLine(origin, origin + Vector3.up, Color.cyan, 10.0f);
            // Debug.DrawLine(origin, p, Color.yellow, 10.0f);

            // Try to apply damage to this object.
            var d = Vector3.Distance(origin, p);
            var r = Mathf.Clamp01(d / Radius);
            var damageable = collider.GetComponent<Damageable>();
            if (damageable)
            {
                var damage = Damage * DamageFalloffCurve.Evaluate(r);
                damageable.Damage(damage, Damager);
                // Debug.Log("Explosive damage to " + collider.gameObject.name + ": damage = " + damage + ", r = " + r + ", d = " + d);
            }
            else
            {
                // Debug.Log("Explosive hit " + collider.gameObject.name + " but caused no damage.");
            }

            // Try to apply force to this object.
            var body = collider.attachedRigidbody;
            if (body)
                body.AddExplosionForce(Force, origin, Radius, ForceUpwardsModifier);
        }

        yield return 0;
    }


}
