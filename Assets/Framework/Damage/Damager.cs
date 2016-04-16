using UnityEngine;
using System.Collections;

public class Damager : MonoBehaviour
{
    // Properties
    // -----------------------------------------------------

    /** Upstream damager, if any. */
    public Damager Initiator
    {
        get { return _initiator; }

        private set
        {
            if (_initiator == value)
                return;

            if (_initiator)
                UnregisterWithInitiator();

            _initiator = value;

            if (_initiator)
                RegisterWithInitiator();
        }
    }


    // Events
    // -----------------------------------------------------

    /** Event handler for damage. */
    public delegate void CausedDamageEventHandler(Damager damager, Damageable target, float damage);

    /** Event handler for destruction. */
    public delegate void CausedDestructionEventHandler(Damager damager, Damageable target);

    /** Event fired when this entity causes damage to another entity. */
    public event CausedDamageEventHandler CausedDamage;

    /** Event fired when this entity causes destruction of another entity. */
    public event CausedDestructionEventHandler CausedDestruction;


    // Members
    // -----------------------------------------------------

    /** Entity that is responsible for damage. */
    private Damager _initiator;

    /** Timeout for clearing initiator (seconds, 0 means never clear). */
    private bool _initiatorExpires;

    /** Time at which initiator gets cleared. */
    private float _initiatorExpiryTime;


    // Unity Methods
    // -----------------------------------------------------

    /** Enabling. */
    private void OnEnable()
    {
        Initiator = null;
    }

    /** Updating. */
    private void Update()
    {
        // Check if initiator has expired, and clear it if so.
        if (_initiatorExpires && Time.time > _initiatorExpiryTime)
            Initiator = null;
    }

    /** Disabling. */
    private void OnDisable()
    {
    }


    // Public Methods
    // -----------------------------------------------------

    /** Sets the upstream initiator, with optional timeout. */
    public void SetInitiator(Damager initiator, float timeout = 0)
    {
        // Update the current initiator.
        Initiator = initiator;

        // Schedule initiator expiry.
        _initiatorExpires = timeout > 0;
        _initiatorExpiryTime = Time.time + timeout;
    }

    /** Notifies this damager that it caused some damage. */
    public void NotifyCausedDamage(Damageable target, float damage)
    {
        if (CausedDamage != null)
            CausedDamage(this, target, damage);

        if (Initiator)
            Initiator.NotifyCausedDamage(target, damage);
    }

    /** Notifies this damager that it caused destruction. */
    public void NotifyCausedDestruction(Damageable target)
    {
        if (CausedDestruction != null)
            CausedDestruction(this, target);

        if (Initiator)
            Initiator.NotifyCausedDestruction(target);
    }


    // Private Methods
    // -----------------------------------------------------

    /** Register with the current initiator. */
    private void RegisterWithInitiator()
    {
    }

    /** Unregister with the current initiator. */
    private void UnregisterWithInitiator()
    {

    }
}
