using UnityEngine;
using System.Collections;


public class ClimbingControllable : ControllableBehaviour
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;


    // Movement Configuration
    // -----------------------------------------------------

    [Header("Movement")]

    /** Maximum climb speed. */
    public float MaxSpeed = 2;

    /** Conversion factor from input to force on the entity's body. */
    public float InputForceScale = 100;

    /** Ladder that player's climbing on (if any). */
    public UseableLadder Ladder
    { get; private set; }



    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
    }


    // Public Methods
    // -----------------------------------------------------

    /** Set the ladder to climb. */
    public void SetLadder(UseableLadder ladder)
    {
        Ladder = ladder;
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Register with a controller. */
    protected override void RegisterWithController()
    {
        base.RegisterWithController();
        Body.useGravity = false;
    }

    /** Unregister with a controller. */
    protected override void UnregisterWithController()
    {
        base.UnregisterWithController();
        Body.useGravity = true;
    }

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Do we have a ladder?
        if (Ladder == null)
            return;

        // Check if ladder is no longer climbable.
        var angle = Vector3.Angle(Ladder.transform.up, Vector3.up);
        if (!(Mathf.Approximately(angle, 0) || Mathf.Approximately(angle, 180)))
            Ladder = null;

        // Check if player wants to dismount.
        var dx = Controller.GetAxis("Move Horizontal");
        if (Mathf.Abs(dx) > 0.9f)
            Ladder = null;

        // Do we still have a ladder?
        if (Ladder == null)
            return;

        // Limit speed while in the air.
        var velocity = Body.velocity;
        var speed = velocity.magnitude;
        if (speed > MaxSpeed)
            Body.velocity = velocity.normalized * MaxSpeed;

        // Get entity's movement input vector.
        var dy = Controller.GetAxis("Move Vertical");

        // Compute raw movement force vector from inputs.
        var input = new Vector3(0, dy, 0);
        var inputLength = input.magnitude;
        if (inputLength > 1)
            input /= inputLength;

        // Modify input to respect max speed.
        var brakes = velocity.normalized * (speed / MaxSpeed);

        // Apply input force to entity.
        var force = (input - brakes) * InputForceScale;
        Body.AddForce(force);

        // Constrain body to stay on the ladder.
        var p = Body.position;
        p.x = Ladder.transform.position.x;
        Body.position = p;

        // Debugging visualization.
        Debug.DrawRay(transform.position, force, Color.white, 0);
    }

}
