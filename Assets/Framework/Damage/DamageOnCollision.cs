using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageOnCollision : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    [Header("Components")]

    /** Damageable object. */
    public Damageable Damageable;

    /** Rigidbody. */
    public Rigidbody Rigidbody;


    [Header("Damage")]
    
    /** Layer mask for objects that can do damage. */
    public LayerMask DamageMask;

    /** Maximum damage that can be inflicted in a single collision. */
    public float DamageMax = 100;

    /** Damage applied for a given impact relative speed. */
    public AnimationCurve DamageForSpeedCurve;

    /** Damage multiplier for a given impactor mass. */
    public AnimationCurve DamageForMassCurve;

    /** Scaling factor for local velocity's influence on damage reduction (0..1). */
    public float LocalVelocityScale = 0;

    /** Whether object only takes damage when colliding with another rigidbody. */
    public bool DamagedOnlyByRigidbodies;

    /** Damage factor for self. */
    public float DamageSelfScale = 1;

    /** Damage factor for impactor. */
    public float DamageOtherScale = 1;

    /** Whether to limit total damage. */
    public bool LimitTotalDamage;

    /** Whether to collide only once. */
    public bool CollideOnceOnly;


    [Header("Timing")]

    /** Delay before object starts taking collision damage. */
    public float Warmup = 0;

    /** Delay after taking collision damage before further damage can be received. */
    public float Cooldown = 0;

    public bool DebugMode = false;


    // Members
    // -----------------------------------------------------

    /** Objects to ignore collisions. */
    private List<GameObject> _ignored = new List<GameObject>();

    /** Time at which object can next receive collision damage. */
    private float _nextDamageTime;

    /** Last known velocity of this rigidbody. */
    private Vector3 _velocity;

    /** Amount of damage that has been dealt. */
    private float _damageDealt;

    /** Whether a collision has occurred already. */
    private bool _collided;


    // Unity Methods
    // -----------------------------------------------------

    /** Called when object is enabled. */
    private void OnEnable()
    {
        if (!Rigidbody)
            Rigidbody = GetComponent<Rigidbody>();
        if (!Damageable)
            Damageable = GetComponent<Damageable>();

        // Object can't take damage until warmup completes.
        _nextDamageTime = Time.time + Warmup;

        // Reset ignored list.
        _ignored.Clear();
        _damageDealt = 0;
        _collided = false;
    }

    /** Update every physics frame. */
    private void FixedUpdate()
    {
        _velocity = Rigidbody.velocity;
    }

    /** Collision handler. */
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision can do damage.
        if (Time.time < _nextDamageTime)
            return;

        // Check if we have already collided before.
        if (CollideOnceOnly && _collided)
            return;

        _collided = true;

        // Can impactor do damage?
        var go = collision.gameObject;
        if ((DamageMask.value & 1 << go.layer) == 0)
            return;

        // If impactor has no rigidbody, check if it will still hurt us.
        if (collision.rigidbody == null && DamagedOnlyByRigidbodies)
            return;

        // Check if we should ignore this impactor.
        if (_ignored.Contains(collision.gameObject))
            return;

        // Determine damage velocity vector.
        var relativeVelocity = collision.relativeVelocity;
        var myVelocity = _velocity * LocalVelocityScale;
        var effectiveVelocity = relativeVelocity + myVelocity;

        // Apply damage based on impact momentum.
        var speed = effectiveVelocity.magnitude;
        var mass = collision.rigidbody ? collision.rigidbody.mass : 1;
        var speedFactor = DamageForSpeedCurve.Evaluate(speed);
        var massFactor = DamageForMassCurve.Evaluate(mass);
        var damage = DamageMax * speedFactor * massFactor;
        var damager = collision.gameObject.GetComponent<Damager>();

        // Limit damage if desired.
        if (LimitTotalDamage)
        {
            var limit = Mathf.Max(0, DamageMax - _damageDealt);
            damage = Mathf.Min(limit, damage);
        }
        _damageDealt += damage;

        // Apply damage to self.
        if (Damageable)
            Damageable.Damage(damage * DamageSelfScale, damager, collision);

        // Apply damage to other entity.
        var otherDamageable = collision.gameObject.GetComponent<Damageable>();
        if (otherDamageable)
        {
            var myDamager = GetComponent<Damager>();
            otherDamageable.Damage(damage * DamageOtherScale, myDamager, collision);
        }

        // Cooldown.
        _nextDamageTime = Time.time + Cooldown;

        /*
        // Optional debug information.
        if (DebugMode && damage > 0)
        {
            Debug.Log("Collision "
                + collision.gameObject + " -> " + gameObject
                + ": speed = " + speed + ", damage = " + damage);

            Debug.DrawRay(Rigidbody.position, relativeVelocity, Color.red, 5);
            Debug.DrawRay(Rigidbody.position, effectiveVelocity, Color.green, 5);
            Debug.DrawRay(Rigidbody.position, myVelocity, Color.blue, 5);
        }
        */
    }


    // Public Methods
    // -----------------------------------------------------

    /** Add an object to the list of ignored objects. */
    public void AddIgnored(GameObject go)
        { _ignored.Add(go);    }

    /** Remove an object from the list of ignored objects. */
    public void RemoveIgnored(GameObject go)
        { _ignored.Remove(go); }


}
