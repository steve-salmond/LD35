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


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
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

}
