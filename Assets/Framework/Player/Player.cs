using UnityEngine;
using System.Collections;
using System.Linq;

public class Player : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** The id of this player. */
    public int Id
    { get; private set; }

    /** The name of this player. */
    public string Name
    { get; private set; }

    /** Number of lives left for this player. */
    public int Lives
    { get; private set; }

    /** Whether player is still alive. */
    public bool Alive
    { get { return (HasUnlimitedLives || Lives > 0); } }

    /** Whether player is dead. */
    public bool Dead
    { get { return !Alive; } }

    /** Number of kills this player has made. */
    public int Kills
    { get; private set; }

    /** Number of deaths this player has suffered. */
    public int Deaths
    { get; private set; }

    /** Player's K/D ratio. */
    public float KillDeathRatio
    { get { return Kills / Mathf.Max(1.0f, Deaths); } }

    /** Player's current score. */
    public int Score
    { get; private set; }

    /** Whether this player is leading the game. */
    public bool Leading
    { get { return Score > 0 && _game.Players.Leaders.Any(p => p == this); } }

    /** Player's rank. */
    public int Rank
    {
        get
        {
            return _game.Players.Players
                .OrderByDescending(p => p.Score)
                .ThenByDescending(p => p.KillDeathRatio)
                .ThenByDescending(p => p.Kills)
                .ThenBy(p => p.Deaths)
                .TakeWhile(p => p != this).Count() + 1;
        }
    }

    /** Color for this player. */
    public Color Color
    { get; private set; }

    /** Player's current location. */
    public Vector3 Position
    { get { return transform.position; } }


    // Configuration
    // -----------------------------------------------------

    [Header("Configuration")]

    /** The player's default controllable entity. */
    public ControllableBehaviour HostPrefab;

    /** Whether the player has a limited number of lives. */
    public bool HasLimitedLives
    { get { return _game.Config.PlayerLives > 0; } }

    /** Whether the player has an unlimited number of lives. */
    public bool HasUnlimitedLives
    { get { return !HasLimitedLives; } }

    /** Whether the player is allowed to respawn. */
    public bool CanRespawn
    { get { return Alive && _game.Playing; } }

    /** The player's controller. */
    public PlayerController Controller
    { get { return _controller; } }

    /** Whether the player is currently controlling an entity. */
    public bool HasControlled
    { get { return Controlled != null; } }

    /** The entity that this player is controlling. */
    public ControllableBehaviour Controlled
    {
        get { return _controllable; }

        private set
        {
            // Check if controlled entity has changed.
            if (_controllable == value)
                return;

            // Unregister with old controllable.
            if (_controllable != null)
                _controllable.Controller = null;

            // Update the controlled entity.
            _controllable = value;

            // Register with new controllable.
            if (_controllable != null)
                _controllable.Controller = _controller;
        }
    }


    // Events
    // -----------------------------------------------------

    /** General player event. */
    public delegate void PlayerEventHandler(Player player);

    /** Player damage event. */
    public delegate void PlayerDamageEventHandler(Player player, Damageable damageable, float damage);

    /** Player scoring event. */
    public delegate void PlayerScoredEventHandler(Player player, int score, string reason);

    /** Event fired when player's score changes. */
    public event PlayerScoredEventHandler Scored;

    /** Event fired when player takes damage. */
    public event PlayerDamageEventHandler Damaged;

    /** Event fired when player dies. */
    public event PlayerEventHandler Killed;

    /** Event fired when player causes damage. */
    public event PlayerDamageEventHandler CausedDamage;

    /** Event fired when player kills another player. */
    public event PlayerEventHandler CausedKill;

    /** Event fired when player runs out of lives. */
    public event PlayerEventHandler OutOfLives;


    // Members
    // -----------------------------------------------------

    /** The player controller. */
    private PlayerController _controller;

    /** Entity currently being controlled by the player. */
    private ControllableBehaviour _controllable;

    /** Whether game is ending. */
    private bool _quitting;

    /** Game manager. */
    private GameManager _game;


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
    }

    /** Disabling. */
    private void OnDisable()
    {
        if (!_quitting)
            _controller.Reset();
    }

    /** Called when application is shutting down. */
    private void OnApplicationQuit()
    {
        _quitting = true;
    }


    // Public Methods
    // -----------------------------------------------------

    /** Configure this player. */
    public void Configure(int id, GameManager game)
    {
        // Remember game.
        _game = game;

        // Remember this player's id.
        Id = id;
        Name = "P" + (Id + 1);
        Color = _game.Config.PlayerColors[id];

        // Configure the player controller.
        _controller = new PlayerController(this);

        // Set up initial number of lives.
        Lives = _game.Config.PlayerLives;
    }

    /** Spawn player at the specified location. */
    public bool Spawn(PlayerSpawnPoint spawn)
    {
        // Can't spawn if already in control of a host.
        if (HasControlled)
            return false;

        // Can't spawn if spawn point is invalid.
        if (spawn == null || !spawn.CanSpawn())
            return false;

        // Spawn and take control of the player's default host.
        var host = Instantiate(HostPrefab);
        spawn.Spawn(host.gameObject);
        TakeControlOf(host);
        return true;        
    }

    /** Apply vibration to the player's joystick (left motor). */
    public void VibrateLeft(float level, float duration)
    { Controller.VibrateLeft(level, duration); }

    /** Apply vibration to the player's joystick (right motor). */
    public void VibrateRight(float level, float duration)
    { Controller.VibrateRight(level, duration); }


    // Private Methods
    // -----------------------------------------------------

    /** Take control of an entity. */
    private void TakeControlOf(ControllableBehaviour entity)
    {
        // Check that entity exists.
        if (entity == null)
            return;

        // Update the controlled entity.
        Controlled = entity;

        // Attach player to the entity.
        var t = Controlled.gameObject.transform;
        transform.parent = t;
        transform.position = t.position;
        transform.rotation = t.rotation;

        // Listen for damage/destruction of the host entity.
        var damageable = Controlled.GetComponent<Damageable>();
        if (damageable)
            RegisterWithDamageable(damageable);

        // Listen for damage caused by the host entity.
        var damager = Controlled.GetComponent<Damager>();
        if (damager)
            RegisterWithDamager(damager);
    }

    /** Release control of an entity. */
    private void ReleaseControlOf(ControllableBehaviour entity)
    {
        // Check that this entity is possessed.
        if (Controlled != entity)
            return;

        // Unregister with the host entity.
        var damageable = Controlled.GetComponent<Damageable>();
        if (damageable)
            UnregisterWithDamageable(damageable);
        var damager = Controlled.GetComponent<Damager>();
        if (damager)
            UnregisterWithDamager(damager);

        // Clear the controlled entity.
        Controlled = null;

        // Detach player from entity.
        transform.parent = null;
    }

    /** Updates the player's score. */
    private void AddScore(int delta, string reason = null)
    {
        if (delta == 0)
            return;

        Score += delta;

        if (Scored != null)
            Scored(this, delta, reason);
    }

    /** Register with a damage component. */
    private void RegisterWithDamageable(Damageable damageable)
    {
        damageable.Damaged += OnPossessedDamaged;
        damageable.Destroyed += OnPossessedDestroyed;
    }

    /** Unregister with a damage component. */
    private void UnregisterWithDamageable(Damageable damageable)
    {
        damageable.Damaged -= OnPossessedDamaged;
        damageable.Destroyed -= OnPossessedDestroyed;
    }

    /** Register with a damage-causing component. */
    private void RegisterWithDamager(Damager damager)
    {
        damager.CausedDamage += OnPossessedCausedDamage;
        damager.CausedDestruction += OnPossessedCausedDestruction;
    }

    /** Unregister with a damage component. */
    private void UnregisterWithDamager(Damager damager)
    {
        damager.CausedDamage -= OnPossessedCausedDamage;
        damager.CausedDestruction -= OnPossessedCausedDestruction;
    }

    /** Handle damage to the host entity. */
    private void OnPossessedDamaged(Damageable d, Damager damager, float damage)
    {
        if (Damaged != null)
            Damaged(this, d, damage);
    }

    /** Handle destruction of the host entity. */
    private void OnPossessedDestroyed(Damageable d, Damager damager)
    {
        // Don't take any action when game is exiting.
        if (_quitting)
            return;

        // Eject from host.
        ReleaseControlOf(Controlled);

        // Check if game is still playing.
        if (_game.Playing)
        {
            // Update player's lives.
            Lives = Mathf.Max(0, Lives - 1);

            // Update deaths.
            Deaths++;
        }

        // Fire a killed event.
        if (Killed != null)
            Killed(this);

        // Fire an out-of-lives event if needed.
        if (Dead)
            OutOfLives(this);

        // Schedule new body spawn.
        if (CanRespawn)
            StartCoroutine(RespawnRoutine());
    }

    /** Handle damage to the host entity. */
    private void OnPossessedCausedDamage(Damager damager, Damageable target, float damage)
    {
        // Check if game is playing.
        if (!_game.Playing)
            return;

        // Check if we damaged a player.
        var pc = target.GetComponent<ControllableBehaviour>();
        var controller = pc ? pc.Controller as PlayerController : null;
        if (controller == null)
            return;

        // Check if we damaged ourselves.
        var friendlyFire = controller == _controller;
        var scoreModifier = friendlyFire ? -1 : 1;

        // Add to player's score (rounding to nearest 10 points.)
        var scoringDamage = Mathf.Max(1, Mathf.RoundToInt(damage * 0.1f)) * 10;
        var scoreDelta = Mathf.RoundToInt(scoringDamage * _game.Config.PointsForDamage);
        AddScore(scoreDelta * scoreModifier);

        // Fire damage event.
        if (CausedDamage != null)
            CausedDamage(controller.Player, target, damage);
    }

    /** Handle destruction of the host entity. */
    private void OnPossessedCausedDestruction(Damager damager, Damageable target)
    {
        // Check if game is playing.
        if (!_game.Playing)
            return;

        // Check if we killed a player.
        var pc = target.GetComponent<ControllableBehaviour>();
        var controller = pc ? pc.Controller as PlayerController : null;
        if (controller == null)
            return;

        // Check if we killed ourselves.
        var friendlyFire = controller == _controller;
        var scoreModifier = friendlyFire ? -1 : 1;

        // Increment player kills.
        Kills++;

        // Add to player's score.
        var scoreDelta = Mathf.RoundToInt(_game.Config.PointsForKill);
        AddScore(scoreDelta * scoreModifier);

        // Fire a killed event.
        if (CausedKill != null)
            CausedKill(controller.Player);
    }

    /** Respawn the player after a delay. */
    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(2);

        while (CanRespawn && !HasControlled)
        {
            var respawned = Respawn();
            if (respawned)
                yield break;

            yield return 0;
        }
    }

    /** Spawn at the player's last spawn point. */
    private bool Respawn()
    {
        if (!CanRespawn)
            return false;

        // Locate the spawn point that's furthest from other players.
        var spawn = _game.Players.GetSafestSpawn();
        return Spawn(spawn);
    }

}
