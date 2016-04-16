using UnityEngine;
using System.Collections;

public class HoverControllable : ControllableBehaviour 
{

    // Properties
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;

    /** Entity's center point. */
    public Transform Origin;

    /** Ground detection for this entity. */
    public GroundControllable Groundable;
    

    [Header("Hovering")]

    /** Force to apply when hovering. */
    public float HoverForce = 100;

    /** Duration of hover. */
    public float HoverDuration = 2;

    /** Hover cooldown. */
    public float HoverCooldown = 1;

    /** Whether hover resets when grounded. */
    public bool HoverResetOnGrounded = true;

    /** Maximum upwards speed for hovering to be applied. */
    public AnimationCurve UpwardSpeedFalloff;


    [Header("Effects")]

    /** Effect to play when hovering. */
    public GameObject HoverEffectPrefab;


    /** Whether entity is currently hovering. */
    public bool Hovering
    { get; private set; }
    
    /** Whether entity is currently grounded. */
    public bool Grounded
    { get { return Groundable.Grounded; } }


    // Members
    // -----------------------------------------------------

    /** Time at which entity started hovering. */
    private float _hoverEnergy = 0;

    /** Time at hover energy will resume charging. */
    private float _hoverRechargeTime = 0;

    /** Hover effect. */
    private GameObject _hoverEffect;


    // Unity Methods
    // -----------------------------------------------------

    /** Disabling. */
    private void OnDisable()
    {
        ObjectPool.Cleanup(_hoverEffect);
    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update from controller. */
    protected override void UpdateControllable(Controller controller)
    {
        // Update hovering state.
        UpdateHovering();
    }


    // Private Methods
    // -----------------------------------------------------

    /** Update entity's hovering state. */
    private void UpdateHovering()
    {
        // Check if player wants to start hovering.
        var wantsToHover = Controller.GetButton("Hover") && !Grounded;

        // Determine if entity is currently hovering.
        Hovering = wantsToHover && _hoverEnergy > 0;

        // Keep pushing out hover cooldown while player is hovering.
        if (wantsToHover)
            _hoverRechargeTime = Time.time + HoverCooldown;

        // Update hover energy.
        if (Hovering)
            _hoverEnergy = Mathf.Clamp(_hoverEnergy - Time.fixedDeltaTime, 0, HoverDuration);
        else if (Time.time >= _hoverRechargeTime)
            _hoverEnergy = Mathf.Clamp(_hoverEnergy + Time.fixedDeltaTime, 0, HoverDuration);

        // Reset hover energy when grounded.
        if (HoverResetOnGrounded && Grounded)
            _hoverEnergy = HoverDuration;

        // Apply hover force.
        if (Hovering)
        {
            var scale = UpwardSpeedFalloff.Evaluate(Body.velocity.y);
            var f = Vector3.up * HoverForce * scale;
            Body.AddForce(PhysicsManager.Normalized(f));
        }

        // Update hover effect.
        if (Hovering && !_hoverEffect && Origin)
        {
            _hoverEffect = ObjectPool.GetAt(HoverEffectPrefab, Origin);
            var ps = _hoverEffect ? _hoverEffect.GetComponent<ParticleSystem>() : null;
            if (ps)
                ps.startColor = Controller.Color;
        }
        else if (!Hovering && _hoverEffect)
        {
            ObjectPool.Cleanup(_hoverEffect);
            _hoverEffect = null;
        }
    }

}
