using UnityEngine;
using System.Collections;

public class MeleeControllable : ControllableBehaviour 
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** The wielder's root game object. */
    public GameObject Wielder;

    /** Emitter for the melee attack. */
    public Transform Emitter;


    // Control Scheme
    // -----------------------------------------------------

    [Header("Melee")]

    /** Button used to initiate melee attack. */
    public string AttackButton = "Melee";

    /** Delay before spawning melee attack effect. */
    public float AttackHitDelay = 0.1f;

    /** Cooldown after initiating a melee attack. */
    public float AttackCooldown = 0.5f;

    /** Prefab for the immediate melee swing effect. */
    public GameObject SwingEffectPrefab;

    /** Prefab for the melee hit effect. */
    public GameObject HitEffectPrefab;


    // Events
    // -----------------------------------------------------

    /** An event relating to melee. */
    public delegate void MeleeEventHandler();

    /** Event fired when entity performs a melee attack. */
    public MeleeEventHandler Attacked;


    // Members
    // -----------------------------------------------------

    /** Next available attack time. */
    private float _nextAttackTime;

    /** Initiating damager. */
    private Damager _initiator;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start() 
	{
        _initiator = Wielder.GetComponent<Damager>();
    }

    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        if (controller.GetButtonDown(AttackButton))
            Attack();
    }


    // Private Methods
    // -----------------------------------------------------

    /** Performs a melee attack. */
    private void Attack()
    {
        if (Time.time < _nextAttackTime)
            return;

        _nextAttackTime = Time.time + AttackCooldown;

        ObjectPool.GetAt(SwingEffectPrefab, Emitter, true);
        Invoke("SpawnEffect", AttackHitDelay);

        if (Attacked != null)
            Attacked();
    }

    /** Spawn the attack effect. */
    private void SpawnEffect()
    {
        if (Controller == null)
            return;

        var go = ObjectPool.GetAt(HitEffectPrefab, Emitter, true);
        if (!go)
            return;

        // Apply controller color to melee effect.
        var ps = go.GetComponent<ParticleSystem>();
        if (ps)
            ps.startColor = Controller.Color;

        // Don't allow the wielder to take damage.
        var explosive = go.GetComponent<Explosive>();
        if (explosive)
            explosive.AddIgnored(Wielder);

        // Inform effect of damage initiator.
        var damager = go.GetComponent<Damager>();
        if (damager)
            damager.SetInitiator(_initiator);
    }

}
