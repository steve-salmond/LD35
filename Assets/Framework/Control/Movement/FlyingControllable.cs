using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class FlyingControllable : ControllableBehaviour
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;


    // Movement Configuration
    // -----------------------------------------------------

    [Header("Movement")]

    /** Bot's maximum speed while grounded. */
    public float MaxSpeed = 2;

    /** Conversion factor from input to force on the entity's body. */
    public float InputForceScale = 100;

    /** Weighting factor when computing force vector from input axes. */
    public Vector2 InputAxisWeight = new Vector2(1, 0);



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
        // Limit speed while in the air.
        var velocity = Body.velocity;
        var speed = velocity.magnitude;
        if (speed > MaxSpeed)
            Body.velocity = velocity.normalized * MaxSpeed;

        // Get entity's movement input vector.
        var dx = Controller.GetAxis("Move Horizontal") * InputAxisWeight.x;
        var dy = Controller.GetAxis("Move Vertical") * InputAxisWeight.y;

        // Compute raw movement force vector from inputs.
        var input = new Vector3(dx, dy, 0);
        var inputLength = input.magnitude;
        if (inputLength > 1)
            input /= inputLength;

        // Modify input to respect max ground speed.
        var brakes = velocity.normalized * (speed / MaxSpeed);

        // Apply input force to entity.
        var force = (input - brakes) * InputForceScale;
        Body.AddForce(force);

        // Debugging visualization.
        Debug.DrawRay(transform.position, force, Color.white, 0);
    }

}
