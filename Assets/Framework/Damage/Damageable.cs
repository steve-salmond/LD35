using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damageable : MonoBehaviour
{
    /** Damage effects configuration. */
    [System.Serializable]
    public struct DamageEffectConfig
    {
        public float Threshold;
        public float Cooldown;
        public GameObject Prefab;
    }


    // Properties
    // -----------------------------------------------------

    /** Initial health value. */
    public float StartingHealth = 100;

    /** Current health value. */
    public float Health
    { get; private set; }

    /** Current health as a fraction of starting health. */
    public float HealthFraction
    { get { return Health / StartingHealth; } }

    /** Current damage fraction . */
    public float DamageFraction
    { get { return 1 - HealthFraction; } }

    /** Damage effects. */
    public List<DamageEffectConfig> DamageEffects;

    /** Destruction effect. */
    public GameObject DestructionEffectPrefab;

    /** Transform to spawn effects from. */
    public Transform EffectOrigin;

    /** Optional Damager to notify when explosive damage is applied. */
    public Damager Damager;


    // Events
    // -----------------------------------------------------

    /** An event relating to damage. */
    public delegate void DamageEventHandler(Damageable d, Damager damager, float damage);

    /** An event relating to destruction. */
    public delegate void DestructionEventHandler(Damageable d, Damager damager);

    /** Event fired when damage is received. */
    public DamageEventHandler Damaged;

    /** Event fired when object is healed. */
    public DamageEventHandler Healed;

    /** Event fired when object is destroyed. */
    public DestructionEventHandler Destroyed;


    // Members
    // -----------------------------------------------------

    /** Entity that caused this object's destruction. */
    private Damager _destroyer;

    /** Timestamp for next damage effect. */
    private float _nextEffectTime;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        Health = StartingHealth;

        if (!EffectOrigin)
            EffectOrigin = transform;

        if (!Damager)
            Damager = GetComponent<Damager>();
    }

    /** Enabling. */
    private void OnEnable()
    {
        Health = StartingHealth;
        _destroyer = null;
    }

    /** Destruction. */
    private void OnDisable()
    {
        if (Health > 0)
            return;

        if (_destroyer != null)
            _destroyer.NotifyCausedDestruction(this);

        if (Destroyed != null)
            Destroyed(this, _destroyer);

        _destroyer = null;
    }


    // Public Methods
    // -----------------------------------------------------

    /** Damages the object. */
    public void Damage(float amount, Damager damager, Collision collision = null)
    {
        // Determine the amount of actual damage dealt.
        var damage = Mathf.Clamp(amount, 0, Health);
        if (damage <= 0)
            return;

        // Apply damage.
        Health -= damage;

        // Spawn a damage effect.
        SpawnDamageEffect(damage, damager, collision);

        // Fire damage event.
        if (Damaged != null)
            Damaged(this, damager, damage);

        // Notify damager of damage/destruction.
        if (damager != null)
            damager.NotifyCausedDamage(this, damage);

        // Fire destroyed event.
        if (Health <= 0)
        {
            _destroyer = damager;
            ObjectPool.Cleanup(gameObject);
        }
    }

    /** Kills the object. */
    public void Kill(Damager damager, Collision collision = null)
    {
        // Remove all the entity's health.
        Damage(Health, damager, collision);
    }

    /** Heals the object. */
    public void Heal(float amount, Damager damager)
    {
        // Determine the amount of actual damage dealt.
        var healing = Mathf.Clamp(amount, 0, StartingHealth - Health);
        if (healing <= 0)
            return;

        // Apply healing.
        Health += healing;

        // Fire healing event.
        if (Healed != null)
            Healed(this, damager, healing);
    }


    // Private Methods
    // -----------------------------------------------------

    /** Spawns a damage effect. */
    private void SpawnDamageEffect(float damage, Damager initiator, Collision collision = null)
    {
        // Check if we should spawn an effect.
        if (Health > 0 && Time.time < _nextEffectTime)
            return;

        // Determine what effect to display based on damage state.
        GameObject prefab = null;
        var cooldown = 0.0f;
        if (Health <= 0)
            prefab = DestructionEffectPrefab;
        else
        {
            var n = DamageEffects.Count;
            for (var i = n - 1; i >= 0; i--)
                if (damage < DamageEffects[i].Threshold)
                {
                    prefab = DamageEffects[i].Prefab;
                    cooldown = DamageEffects[i].Cooldown;
                }
                else
                    break;
        }

        // Check that an effect was selected.
        if (!prefab)
            return;

        // Spawn the appropriate damage effect.
        GameObject go = null;
        if (collision != null)
            go = ObjectPool.GetAt(prefab, collision.contacts[0].point, Quaternion.identity);
        else if (EffectOrigin)
            go = ObjectPool.GetAt(prefab, EffectOrigin.position, Quaternion.identity);

        // Inform spawned effect of upstream damager.
        var damager = go.GetComponent<Damager>();
        if (damager)
            damager.SetInitiator(Damager);

        // Schedule next possible effect.
        _nextEffectTime = Time.time + cooldown;
    }


}
