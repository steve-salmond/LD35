using UnityEngine;
using System.Collections;

public class JumpingControllable : ControllableBehaviour
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

    /** Whether entity is currently grounded. */
    public bool Grounded
    { get { return Groundable.Grounded; } }



    // Jumping Configuration
    // -----------------------------------------------------

    [Header("Jumping")]

    /** Instantaneous speed to apply when jumping. */
    public float JumpSpeed = 20;

    /** Minimum interval before a double jump can be made. */
    public float JumpDoubleMin = 0.1f;

    /** Maximum interval for a double jump to be made. */
    public float JumpDoubleMax = 0.75f;

    /** Interval between successive jumps. */
    public float JumpCooldown = 0.5f;

    /** Jump vibration strength. */
    public float JumpVibrationStrength = 0.5f;

    /** Jump vibration duration. */
    public float JumpVibrationDuration = 0.25f;


    // Wall Jumping Configuration
    // -----------------------------------------------------

    [Header("Wall Jumping")]

    /** Layer mask for detecting when the entity is next to a wall. */
    public LayerMask WallMask;

    /** Distance tolerance when determining if entity is next to a wall. */
    public float WallDistance = 0.6f;

    /** Instantaneous lateral velocity to apply when wall jumping. */
    public float WallJumpSpeed = 10;
    

    // Effects
    // -----------------------------------------------------

    [Header("Effects")]

    /** Effect to play when jumping off the ground. */
    public GameObject JumpGroundedEffectPrefab;

    /** Effect to play when jumping in the air. */
    public GameObject JumpAerialEffectPrefab;


    // Events
    // -----------------------------------------------------

    /** An event relating to a entity. */
    public delegate void JumpingEventHandler();

    /** Event fired when entity jumps. */
    public JumpingEventHandler Jumped;


    // Members
    // -----------------------------------------------------
    
    /** Time at which entity started jumping. */
    private float _jumpStartTime;

    /** Next time at which entity can jump. */
    private float _jumpNextTime;

    /** Number of jumps entity has made since they were last grounded. */
    private float _jumpCount;

    /** Raycast result buffer for walljump check. */
    private RaycastHit[] _hits = new RaycastHit[2];


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
        // Check if entity wishes to jump.
        if (controller.GetButtonDown("Jump"))
            Jump();
    }

    /** Register with a controller. */
    protected override void RegisterWithController()
    {
        base.RegisterWithController();
    }

    /** Unregister with a controller. */
    protected override void UnregisterWithController()
    {
        base.UnregisterWithController();
    }


    // Private Methods
    // -----------------------------------------------------

    /** Attempt to make the entity jump. */
    private void Jump()
    {
        // Check if entity is against walls on either side.
        RaycastHit hit;
        var againstLeftWall = IsAgainstWall(Origin.position, Vector3.left, out hit);
        var againstRightWall = IsAgainstWall(Origin.position, Vector3.right, out hit);

        // Check if entity can start jumping.
        var canStartJump = Grounded || againstLeftWall || againstRightWall;
        if (canStartJump && Time.time >= _jumpNextTime)
            _jumpCount = 0;
        else if (_jumpCount != 1)
            return;

        // Check for jump cooldown.
        if (_jumpCount == 0 && Time.time < _jumpNextTime)
            return;

        // Check if entity can double-jump.
        var tSinceStart = Time.time - _jumpStartTime;
        if (_jumpCount == 1)
            if (tSinceStart < JumpDoubleMin || tSinceStart > JumpDoubleMax)
                return;

        // Get entity's current velocity.
        var v = Body.velocity;

        // Zero vertical velocity if in the air (or on a stable platform).
        if (Groundable.GroundBody == null)
            v.y = 0;

        // Determine if wall-jumping should occur.
        if (againstLeftWall && !Grounded)
            v.x += WallJumpSpeed;
        if (againstRightWall && !Grounded)
            v.x -= WallJumpSpeed;

        // Apply jump speed.
        v.y += JumpSpeed;

        // Reset entity's velocity.
        Body.velocity = v;

        // Play jump effect.
        ObjectPool.Get(Grounded
            ? JumpGroundedEffectPrefab
            : JumpAerialEffectPrefab, transform, false);

        // Vibrate player's controller.
        var pc = Controller as PlayerController;
        if (pc != null)
            pc.VibrateRight(JumpVibrationStrength, JumpVibrationDuration);

        // Schedule next jump.
        _jumpStartTime = Time.time;
        _jumpCount = _jumpCount + 1;
        _jumpNextTime = Time.time + JumpCooldown;

        // Fire jump event.
        if (Jumped != null)
            Jumped();
    }

    private bool IsAgainstWall(Vector3 position, Vector3 direction, out RaycastHit hit)
    { return IsAgainst(position, direction, out hit, WallDistance, WallMask); }

    /** Detect a wall next to the entity in the given direction. */
    private bool IsAgainst(Vector3 position, Vector3 direction, out RaycastHit hit, float distance, LayerMask mask)
    {
        // Check if there's anything next to the entity that can be treated as a wall.
        var ray = new Ray(position, direction);
        var result = Physics.RaycastNonAlloc(ray, _hits, distance, mask) > 0;
        hit = _hits[0];

        // Debug visualization
        Debug.DrawRay(position, direction, result ? Color.green : Color.red, 0);

        return result;
    }


}
