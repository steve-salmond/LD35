using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class PlayerControllable : ControllableBehaviour
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this player. */
    public Rigidbody Body;

    /** Player's center point. */
    public Transform Origin;

    /** Ground detection. */
    public GroundControllable Groundable;

    /** Running control. */
    public RunningControllable Running;

    /** Jumping control. */
    public JumpingControllable Jumping;

    /** Aiming control. */
    public AimingControllable Aiming;

    /** Melee control. */
    public MeleeControllable Melee;


    [Header("Using")]

    /** Radius to check for nearby objects when using. */
    public float UseRadius = 1;

    /** Layer mask that governs what usable objects will be detected. */
    public LayerMask UseMask;

    /** Ladder that player's climbing on (if any). */
    public UseableLadder Ladder
    { get; private set; }


    // Members
    // -----------------------------------------------------

    /** Buffer for detecting usable objects. */
    private Collider[] _colliders = new Collider[1];


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
    }

    /** Physics update. */
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Body.IsSleeping())
            Body.WakeUp();
    }


    // Public Methods
    // -----------------------------------------------------

    /** Start using a ladder. */
    public void Climb(UseableLadder ladder)
    {
        Ladder = ladder;
    }

    /** Dismount the current ladder (if any). */
    public void Dismount()
    {
        Ladder = null;
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Check if player wishes to use something.
        if (Controller.GetButtonDown("Use"))
            Use();

        // Dismount ladders when player jumps.
        if (Controller.GetButtonDown("Jump"))
            Dismount();
    }

    /** Register with a controller. */
    protected override void RegisterWithController()
    {
        base.RegisterWithController();
        Groundable.Controller = Controller;
        Running.Controller = Controller;
        Jumping.Controller = Controller;
        Aiming.Controller = Controller;
        Melee.Controller = Controller;
    }

    /** Unregister with a controller. */
    protected override void UnregisterWithController()
    {
        base.UnregisterWithController();
        Groundable.Controller = null;
        Running.Controller = null;
        Jumping.Controller = null;
        Aiming.Controller = null;
        Melee.Controller = null;
    }


    // Private Methods
    // -----------------------------------------------------

    /** Attempt to use a nearby object. */
    private void Use()
    {
        var n = Physics.OverlapSphereNonAlloc(transform.position, UseRadius, _colliders, UseMask);
        if (n <= 0)
            return;

        var useable = _colliders[0].GetComponent<UseableBehaviour>();
        if (useable)
            useable.Use(this);
    }


}
