using UnityEngine;
using System.Collections;

public class RunningControllable : ControllableBehaviour
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;

    /** Entity's center point. */
    public Transform Origin;

    /** Ground detection for this entity. */
    public GroundControllable Groundable;


    // Movement Configuration
    // -----------------------------------------------------

    [Header("Movement")]

    /** Entity's maximum speed while grounded. */
    public float MaxSpeed = 2;

    /** Entity's maximum speed while in the air. */
    public float MaxAerialSpeed = 50;

    /** Conversion factor from input to force on the entity's body. */
    public float InputForceScale = 100;

    /** Weighting factor when computing force vector from input axes. */
    public Vector2 InputAxisWeight = new Vector2(1, 0);

    /** Conversion factor from input to force on the entity's body (while in the air). */
    public float AerialForceScale = 25;

    /** Weighting factor when computing force vector from input axes (while in the air). */
    public Vector2 AerialAxisWeight = new Vector2(1, 1);


    [Header("Sprinting")]

    /** Whether sprinting is enabled. */
    public bool SprintEnabled;

    /** Speed boost when sprinting. */
    public float SprintMultiplier = 1.5f;

    /** Duration of sprint. */
    public float SprintDuration = 2;

    /** Sprint cooldown. */
    public float SprintCooldown = 1;

    /** Whether entity is currently sprinting. */
    public bool Sprinting
    { get; private set; }


    [Header("Dashing")]

    /** Whether dashing is allowed. */
    public bool DashEnabled = true;

    /** Entity's maximum speed while dashing. */
    public float MaxDashSpeed = 50;

    /** input threshold for signaling a dash. */
    public float DashInputThreshold = 0.5f;

    /** Dash speed increase. */
    public float DashSpeedBonus = 10;

    /** Dash duration. */
    public float DashDuration = 0.1f;

    /** Dash cooldown. */
    public float DashCooldown = 1;

    /** Whether dashing is in effect. */
    public bool Dashing
    { get { return Time.time < _dashStopTime; } }


    [Header("Drag")]

    /** Drag on the entity's body while idle. */
    public float IdleDrag = 0;

    /** Drag on the entity's body while in motion (on the ground). */
    public float MovingDrag = 0;

    /** Drag on the entity's body while in the air. */
    public float AerialDrag = 0;


    [Header("Effects")]

    /** Side dash effect prefab. */
    public GameObject DashSideEffectPrefab;

    /** Down dash effect prefab. */
    public GameObject DashDownEffectPrefab;


    // Private Properties
    // -----------------------------------------------------

    /** Whether entity is currently grounded. */
    private bool Grounded
    { get { return Groundable.Grounded; } }


    // Members
    // -----------------------------------------------------

    /** Time at which entity started sprinting. */
    private float _sprintEnergy = 0;

    /** Time at sprint energy will resume charging. */
    private float _sprintRechargeTime = 0;

    /** Time at which dash expires. */
    private float _dashStopTime;

    /** Time at which dash is next possible. */
    private float _dashNextTime;

    /** Whether dash button was down. */
    private bool _wantedToDash;


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
        if (!Groundable)
            Groundable = GetComponent<GroundControllable>();

        _sprintEnergy = SprintDuration;
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Update sprinting state.
        if (SprintEnabled)
            UpdateSprinting();

        // Update entity's dashing.
        UpdateDashing();

        // Update entity's movement.
        UpdateMovement();
    }


    // Private Methods
    // -----------------------------------------------------

    /** Update entity's sprinting state. */
    private void UpdateSprinting()
    {
        // Check if player wants to start sprinting.
        var wantsToSprint = Controller.GetButton("Sprint");

        // Determine if entity is currently sprinting.
        Sprinting = wantsToSprint && _sprintEnergy > 0;

        // Keep pushing out sprint cooldown while player is sprinting.
        if (wantsToSprint)
            _sprintRechargeTime = Time.time + SprintCooldown;

        // Update sprint energy.
        if (Sprinting)
            _sprintEnergy = Mathf.Clamp(_sprintEnergy - Time.fixedDeltaTime, 0, SprintDuration);
        else if (Time.time >= _sprintRechargeTime)
            _sprintEnergy = Mathf.Clamp(_sprintEnergy + Time.fixedDeltaTime, 0, SprintDuration);
    }

    /** Update the entity's movement from inputs. */
    private void UpdateMovement()
    {
        // Limit speed while in the air.
        if (!Grounded)
        {
            var aerialVelocity = Body.velocity;
            var aerialSpeed = aerialVelocity.magnitude;
            if (aerialSpeed > MaxAerialSpeed)
                Body.velocity = aerialVelocity.normalized * MaxAerialSpeed;
        }

        // Get entity's movement input vector.
        var axisWeight = Grounded ? InputAxisWeight : AerialAxisWeight;
        var dx = Controller.GetAxis("Move Horizontal") * axisWeight.x;
        var dy = Controller.GetAxis("Move Vertical") * axisWeight.y;

        // Compute raw movement force vector from inputs.
        var input = new Vector3(dx, dy, 0);
        var inputLength = input.magnitude;
        if (inputLength > 1)
            input /= inputLength;

        // Modify input to respect max ground speed.
        var velocity = new Vector3(Body.velocity.x, 0, 0);
        var speed = velocity.magnitude;
        var sprintMultiplier = (Sprinting ? SprintMultiplier : 1);
        var maxSpeed = Dashing ? MaxDashSpeed : (MaxSpeed * sprintMultiplier);
        var brakes = velocity.normalized * (speed / maxSpeed);

        // Apply input force to entity.
        var runScale = InputForceScale * sprintMultiplier;
        var scale = Grounded ? runScale : AerialForceScale;
        var force = (input - brakes) * scale;
        Body.AddForce(force);

        // Debugging visualization.
        Debug.DrawRay(transform.position, force, Color.white, 0);

        // Adjust entity's drag.
        if (Grounded)
            Body.drag = Mathf.Approximately(input.magnitude, 0) ? IdleDrag : MovingDrag;
        else
            Body.drag = AerialDrag;
    }

    /** Update entity's dashing state. */
    private void UpdateDashing()
    {
        // Check if dashing is enabled.
        if (!DashEnabled)
            return;

        // Check if player wants to dash.
        var wantsToDash = Controller.GetButton("Dash");
        var shouldDash = wantsToDash && !_wantedToDash;
        _wantedToDash = wantsToDash;

        // Check if dash is still cooling down.
        if (!shouldDash || Time.time < _dashNextTime)
            return;

        // Get entity's movement input vector.
        var dx = Controller.GetAxis("Move Horizontal");
        var dy = Controller.GetAxis("Move Vertical");

        // Don't allow player to dash upwards.
        dy = Mathf.Min(0, dy);

        // Don't allow dashing if there's very little input.
        if (Mathf.Abs(dx) < DashInputThreshold && Mathf.Abs(dy) < DashInputThreshold)
            return;

        // Restrict dash to cardinal directions (+X/-X/-Y).
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            { dy = 0; dx = Mathf.Sign(dx); }
        else
            { dx = 0; dy = Mathf.Sign(dy); }

        // Don't allow dashing down while grounded.
        if (dy < 0 && Grounded)
            return;

        // Compute raw movement force vector from inputs.
        var input = new Vector3(dx, dy, 0);
        var inputLength = input.magnitude;
        if (inputLength > 1)
            input /= inputLength;

        // Apply dash impulse.
        Body.velocity += input * DashSpeedBonus;

        // Spawn appropriate dash effect. */
        var prefab = dx != 0 ? DashSideEffectPrefab : DashDownEffectPrefab;
        var go = ObjectPool.GetAt(prefab, Body.transform, true);

        // TODO: Fix this - hacky.
        var ribbon = go ? go.GetComponentInChildren<Ribbon>() : null;
        if (ribbon)
            ribbon.StartColor = Controller.Color * 0.25f;

        // Schedule next dash.
        _dashNextTime = Time.time + DashCooldown;
        _dashStopTime = Time.time + DashDuration;
    }


}
