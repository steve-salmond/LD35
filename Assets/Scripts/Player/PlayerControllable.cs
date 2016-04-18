using UnityEngine;
using System.Collections;
using System.Linq;

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

    /** Player's damageable. */
    public Damageable Damageable;

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

    /** Climbing control. */
    public ClimbingControllable Climbing;


    [Header("Using")]

    /** Use button. */
    public string UseButton = "Use";

    /** Radius to check for nearby objects when using. */
    public float UseRadius = 1;

    /** Layer mask that governs what usable objects will be detected. */
    public LayerMask UseMask;

    /** Layer mask for things that block using line of sight. */
    public LayerMask UseBlockerMask;

    /** Ladder that player's climbing on (if any). */
    public UseableLadder Ladder
    { get; private set; }


    // Members
    // -----------------------------------------------------

    /** Buffer for detecting usable objects. */
    private Collider[] _colliders = new Collider[3];


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
        if (!Damageable)
            Damageable = GetComponent<Damageable>();
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
        // Set up ladder climbing.
        SetSubControllables(null);
        Ladder = ladder;
        Climbing.SetLadder(ladder);
        Climbing.Controller = Controller;
        Jumping.Controller = Controller;
        Groundable.Controller = Controller;
        Groundable.SetLadder(ladder);
    }

    /** Dismount the current ladder (if any). */
    public void Dismount()
    {
        // Are we climbing a ladder?
        if (Ladder == null)
            return;

        // Restore normal movement behaviour.
        Ladder = null;
        Climbing.SetLadder(null);
        Climbing.Controller = null;
        Groundable.SetLadder(null);
        SetSubControllables(Controller);
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Check if player wishes to use something.
        if (Controller.GetButtonDown(UseButton))
            Use();

        // Dismount ladder if player jumps.
        if (Ladder != null && Controller.GetButtonDown("Jump"))
            Dismount();

        // Dismount ladder if it's no longer climbable.
        if (Ladder != null && Climbing.Ladder == null)
            Dismount();
    }

    /** Register with a controller. */
    protected override void RegisterWithController()
    {
        base.RegisterWithController();
        SetSubControllables(Controller);
    }

    /** Unregister with a controller. */
    protected override void UnregisterWithController()
    {
        base.UnregisterWithController();
        SetSubControllables(null);
    }


    // Private Methods
    // -----------------------------------------------------

    /** Attempt to use a nearby object. */
    private void Use()
    {
        var n = Physics.OverlapSphereNonAlloc(Origin.position, UseRadius, _colliders, UseMask);
        if (n <= 0)
            return;

        var p = Origin.position;
        for (var i = 0; i < n; i++)
        {
            // Check line of sight.
            var c = _colliders[i].ClosestPointOnBounds(p);
            if (Physics.Linecast(c, p, UseBlockerMask))
                continue;

            // Try to use this object.
            var useable = _colliders[i].GetComponent<UseableBehaviour>();
            if (useable)
                useable.Use(this);
        }
    }

    /** Set controllers. */
    private void SetSubControllables(Controller controller)
    {
        Groundable.Controller = controller;
        Running.Controller = controller;
        Jumping.Controller = controller;
        Aiming.Controller = controller;
        Melee.Controller = controller;
    }


}
