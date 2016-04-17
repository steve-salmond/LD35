using UnityEngine;
using System.Collections;
using System.Linq;

public class GroundControllable : ControllableBehaviour
{

    // Structs
    // -----------------------------------------------------

    /** Configuration for a landed effect. */
    [System.Serializable]
    public struct LandedEffectConfig
    {
        public GameObject Prefab;
        public Vector2 SpeedRange;
        public float VibrationStrength;
        public float VibrationDuration;
    }


    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** Rigidbody for this entity. */
    public Rigidbody Body;

    /** Entity's center point. */
    public Transform Origin;

    /** Initiating damager. */
    public Damager Damager;

    
    // Grounded Configuration
    // -----------------------------------------------------

    [Header("Falling")]

    /** Layer mask for detecting when the entity is grounded. */
    public LayerMask GroundMask;

    /** Distance tolerance when determining if entity is grounded. */
    public float GroundDistance = 0.6f;

    /** Horizontal offset to use when checking if entity is on a ledge. */
    public float GroundHorizontalOffset = 0.5f;

    /** Horizontal offset to use when checking for ground to left/right of entity. */
    public float GroundToSidesOffset = 2;


    // Falling Configuration
    // -----------------------------------------------------

    [Header("Falling")]

    /** Threshold for considering entity to be falling. */
    public float FallingThreshold = 5;

    /** Whether entity is falling. */
    public bool Falling
    { get { return !Grounded && Body.velocity.y < -FallingThreshold; } }


    // Properties
    // -----------------------------------------------------

    /** Whether entity is currently grounded. */
    public bool Grounded
    { get; private set; }

    /** Whether there is ground to the left of the entity. */
    public bool HasGroundToLeft
    { get; private set; }

    /** Whether there is ground to the right of the entity. */
    public bool HasGroundToRight
    { get; private set; }

    /** Rigidbody for current ground (if any). */
    public Rigidbody GroundBody
    { get; private set; }

    /** Ladder that player's climbing on (if any). */
    public UseableLadder Ladder
    { get; private set; }



    // Events
    // -----------------------------------------------------

    /** An event relating to a entity landing on the ground. */
    public delegate void LandedEventHandler(Vector3 velocity);

    /** Event fired when entity lands. */
    public LandedEventHandler Landed;


    // Effects
    // -----------------------------------------------------

    [Header("Effects")]

    /** Effects to play when landing on the ground. */
    public LandedEffectConfig[] LandedEffects;


    // Members
    // -----------------------------------------------------

    /** Raycast result buffer for ground check. */
    private RaycastHit[] _hits = new RaycastHit[2];

    /** Body's last known velocity. */
    private Vector3 _velocity;


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

    /** Update on every physics frame. */
    protected override void UpdateControllable(Controller controller)
    {
        UpdateGrounded();
        _velocity = Body.velocity;
    }


    // Private Methods
    // -----------------------------------------------------

    /** Update the entity's grounded state. */
    private void UpdateGrounded()
    {
        // Remember if we were previously falling.
        var wasFalling = Falling;

        // Check if there's anything below us that can be considered ground.
        // Check on the left, middle and right of the body - this helps in cases
        // where the body is right on the edge of a ground surface.
        var c = Origin.position;
        var offset = Vector3.right * GroundHorizontalOffset;
        RaycastHit hit;
        Grounded = IsAgainstGround(c, Vector3.down, out hit);
        //    || IsAgainstGround(c + offset, Vector3.down, out hit)
        //    || IsAgainstGround(c - offset, Vector3.down, out hit);

        // Update the current ground information.
        if (Grounded)
            GroundBody = hit.rigidbody;
        else
            GroundBody = null;

        // Consider player to be grounded on ladders.
        if (Ladder != null)
            Grounded = true;

        // Check entity's left and right for ground.
        offset = Vector3.right * GroundToSidesOffset;
        HasGroundToLeft = IsAgainstGround(c - offset, Vector3.down, out hit);
        HasGroundToRight = IsAgainstGround(c + offset, Vector3.down, out hit);

        // Check if we just landed after falling.
        if (Grounded && wasFalling)
        {
            // Spawn an effect.
            SpawnLandedEffect(_velocity);

            // Fire a landed event.
            if (Landed != null)
                Landed(_velocity);
        }
    }

    private bool IsAgainstGround(Vector3 position, Vector3 direction, out RaycastHit hit)
    { return IsAgainst(position, direction, out hit, GroundDistance, GroundMask); }

    /** Detect objects next to the entity in the given direction. */
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

    /** Spawns a landed effect based on velocity. */
    private void SpawnLandedEffect(Vector3 velocity)
    {
        // Determine which effect to spawn based on vertical speed.
        var speed = Mathf.Max(0, -velocity.y);
        // Debug.Log("Landed at speed: " + speed);
        var effect = LandedEffects
            .Where(e => speed >= e.SpeedRange.x && speed <= e.SpeedRange.y)
            .FirstOrDefault();

        // Spawn the appropriate effect (if any).
        var go = ObjectPool.GetAt(effect.Prefab, Body.transform, false);

        // Vibrate player's controller.
        var pc = Controller as PlayerController;
        if (pc != null)
            pc.VibrateRight(effect.VibrationStrength, effect.VibrationDuration);

        // Configure effect for possible damage.
        var damager = go ? go.GetComponent<Damager>() : null;
        if (damager)
            damager.SetInitiator(Damager);
        var explosive = go ? go.GetComponent<Explosive>() : null;
        if (explosive)
            explosive.AddIgnored(Body.gameObject);
    }

}
