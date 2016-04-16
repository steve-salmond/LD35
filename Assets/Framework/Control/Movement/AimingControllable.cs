using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AimingControllable : ControllableBehaviour
{

    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;

    /** Entity's center point. */
    public Transform Origin;

    /** Entity's facing transform. */
    public Transform Facing;

    /** Entity's ground detection. */
    public GroundControllable Groundable;


    // Aiming Configuration
    // -----------------------------------------------------

    [Header("Aiming")]

    /** Aimpoint for this entity. */
    public Transform Aim;

    /** Distance range for the aim target point. */
    public Vector2 AimRange = new Vector2(2, 10);

    /** Speed at which aim point can be rotated. */
    public float AimRotateSpeed = 1;

    /** Minimum angle above horizontal we can aim when grounded. */
    public float AimHorizontalAngleOffset = 1;

    /** Whether entity is currently actively aiming. */
    public bool Aiming
    { get; private set; }

    
    // Facing Configuration
    // -----------------------------------------------------

    [Header("Facing")]

    /** Input axis threshold for changing facing direction. */
    public float FacingThreshold = 0.25f;

    /** Speed at which the entity rotates towards facing direction. */
    public float FacingRotateSpeed = 1;

    /** Facing direction for entity (+1 means facing right, -1 is left). */
    public float Direction
    { get; private set; }


    // Auto-Aim Configuration
    // -----------------------------------------------------

    [Header("Auto Aim")]

    /** Whether auto-aim is possible. */
    public bool AutoAimEnabled = true;

    /** Strength of auto-aim effect at a given angle delta to target. */
    public AnimationCurve AutoAimForAngle;

    /** Strength of auto-aim effect at a given distance to target. */
    public AnimationCurve AutoAimForDistance;

    /** Vertical offset of auto-aim effect at a given distance to target. */
    public AnimationCurve AutoAimOffsetForDistance;

    /** Rotation speed scaling factor for a given auto-aim strength. */
    public AnimationCurve AutoAimSpeedScale;

    /** Whether auto-aim is enabled. */
    public bool AutoAim
    { get; private set; }


    // Members
    // -----------------------------------------------------

    /** Desired facing rotation. */
    private Quaternion _targetRotation;

    /** Desired aim rotation. */
    private Quaternion _aimRotation;


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

    /** Sets the auto-aim target identifier function. */
    public void SetAutoAim(bool value)
    { AutoAim = value; }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Update entity's aim.
        UpdateAim();

        // Update entity's facing.
        UpdateFacing();
    }


    // Private Methods
    // -----------------------------------------------------

    /** Update the entity's aim. */
    private void UpdateAim()
    {
        Aiming = Controller.IsAiming();
        var method = Controller.GetAimMethod();
        if (method == AimMethod.Direction)
        {
            // Get aim direction (if not given, assume forwards.)
            var direction = Controller.GetAimDirection();
            if (!Aiming)
            {
                var f = Body.transform.forward;
                direction = new Vector3(f.x, f.y + 0.01f, 0);
            }

            // Apply auto-aiming.
            var strength = 0.0f;
            if (AutoAimEnabled && AutoAim)
                direction = ApplyAutoAiming(direction, out strength);

            // Update aim transform from direction.
            UpdateAimFromDirection(direction, strength);
        }
        else if (method == AimMethod.Point)
            Aim.position = Controller.GetAimPoint();
    }

    /** Applies auto-aiming based on the desired aim direction. */
    private Vector3 ApplyAutoAiming(Vector3 direction, out float strength)
    {
        // Get the player collection.
        var players = GameManager.Instance.Players.Players;

        // Select the best target to auto-aim towards.
        strength = 0;
        var autoaim = Vector3.zero;
        var origin = Origin.position;
        foreach (var player in players)
        {
            if (!player.HasControlled)
                continue;

            var body = player.Controlled.GetComponent<Rigidbody>();
            var p = body.position + body.centerOfMass;
            var distance = Vector3.Distance(p, origin);
            p.y += AutoAimOffsetForDistance.Evaluate(distance);
            // Debug.DrawLine(origin, p, Color.green);

            var delta = p - origin;
            var angle = Vector3.Angle(direction, delta);

            var s = AutoAimForAngle.Evaluate(angle) * AutoAimForDistance.Evaluate(distance);

            if (s > strength)
            {
                strength = s;
                autoaim = delta;
            }
        }

        // Check if we found a candidate.
        if (strength <= 0)
            return direction;

        // Interpolate towards the target direction based on strength.
        direction = Vector3.Lerp(direction, autoaim, strength);

        return direction;
    }

    /** Attempts to aim in the given direction, subject to constraints. */
    private void UpdateAimFromDirection(Vector3 direction, float autoAimStrength)
    {
        // Convert direction into a rotation angle about Z.
        var angle = Vector3.Angle(Vector3.down, direction);
        if (direction.x < 0)
            angle = 360 - angle;

        // Constrain angle to avoid clipping through ground.
        if (Groundable.HasGroundToLeft)
            angle = Mathf.Min(angle, 270 - AimHorizontalAngleOffset);
        if (Groundable.HasGroundToRight)
            angle = Mathf.Max(angle, 90 + AimHorizontalAngleOffset);

        // Determine rotate speed based on auto aim strength.
        var speed = AimRotateSpeed * AutoAimSpeedScale.Evaluate(autoAimStrength);

        // Smooth the target angle.
        var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _aimRotation = Quaternion.Slerp(_aimRotation,
            targetRotation, Time.fixedDeltaTime * speed);

        // Map aim direction into an output aim position.
        var delta = _aimRotation * (Vector3.down * AimRange.y);
        var target = Origin.position + delta;
        Aim.position = target;
    }

    /** Update the entity's facing. */
    private void UpdateFacing()
    {
        if (Aiming)
        {
            // Face towards the active aim point.
            if (Aim.position.x < Body.position.x)
                SetDirection(-1);
            else
                SetDirection(1);
        }
        else
        {
            // Face the direction of movement input.
            var dx = Controller.GetAxis("Move Horizontal");
            if (dx < -FacingThreshold)
                SetDirection(-1);
            else if (dx > FacingThreshold)
                SetDirection(1);
        }

        // Turn entity towards desired facing rotation.
        Facing.rotation = Quaternion.Slerp(Facing.rotation,
            _targetRotation, Time.fixedDeltaTime * FacingRotateSpeed);
    }

    /** Set a new facing direction for the entity. */
    private void SetDirection(float facing)
    {
        Direction = facing;

        if (Direction < 0)
            _targetRotation = Quaternion.Euler(0, 270, 0);
        else
            _targetRotation = Quaternion.Euler(0, 90, 0);
    }

}
