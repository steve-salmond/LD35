using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager : MonoBehaviour
{

    // Players
    // -----------------------------------------------------

    [Header("Player Configuration")]

    /** The player prefab. */
    public Player PlayerPrefab;


    // Properties
    // -----------------------------------------------------

    /** Return a list of the current players. */
    public List<Player> Players
    { get { return _players; } }

    /** Return the number of live players. */
    public int AlivePlayerCount
    {
        get
        {
            var count = 0;
            var n = _players.Count;
            for (var i = 0; i < n; i++)
                if (_players[i].Alive)
                    count++;

            return count;
        }
    }

    /** Return player9s) with the highest score. */
    public IEnumerable<Player> Leaders
    {
        get
        {
            var highest = HighestScore;
            return _players.Where(p => p.Score == highest);
        }
    }

    /** Returns the highest player score. */
    public int HighestScore
    {
        get
        {
            var highest = 0;
            var n = _players.Count;
            for (var i = 0; i < n; i++)
                if (_players[i].Score > highest)
                    highest = _players[i].Score;

            return highest;
        }
    }


    // Events
    // -----------------------------------------------------

    /** Event fired when player takes damage. */
    public event Player.PlayerDamageEventHandler Damaged;

    /** Event fired when player dies. */
    public event Player.PlayerEventHandler Killed;

    /** Event fired when player runs out of lives. */
    public event Player.PlayerEventHandler OutOfLives;



    // Members
    // -----------------------------------------------------

    /** The collection of players in the game. */
    private List<Player> _players = new List<Player>();

    /** Lookup for players by id. */
    private Dictionary<int, Player> _playersById = new Dictionary<int, Player>();

    /** The collection of spawn points in the game. */
    private List<SpawnPoint> _spawns = new List<SpawnPoint>();

    /** The current game. */
    private GameManager _game;



    // Public Methods
    // -----------------------------------------------------

    /** Populate the player collection. */
    public void Configure(GameManager game)
    {
        _game = game;

        // Ensure the minimum number of players exist.
        for (var i = 0; i < _game.Config.MinPlayers; i++)
            EnsurePlayerExists(i);

        // Check if we should create more players.
        // We'd like one player per controller.
        EnsurePlayersExist();

        // Listen for new controllers being connected.
        Rewired.ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    /** Add a spawn point. */
    public void AddSpawn(SpawnPoint spawn)
    { _spawns.Add(spawn); }

    /** Remove a spawn point. */
    public void RemoveSpawn(SpawnPoint spawn)
    { _spawns.Remove(spawn); }

    /** Locate a spawn point by id. */
    public SpawnPoint GetSpawnById(int id)
    {
        var spawnName = "Spawn" + id;
        return _spawns
            .Where(spawn => spawn.gameObject.name == spawnName)
            .FirstOrDefault();
    }

    /** Locates the safest spawn point to spawn from. */
    public SpawnPoint GetSafestSpawn()
    {
        return _spawns
            .Where(spawn => spawn.CanSpawn())
            .OrderByDescending(spawn => _players
                .Where(p => p.HasControlled)
                .Sum(p => Vector3.Distance(p.Position, spawn.transform.position)))
            .FirstOrDefault();
    }


    // Implementation
    // -----------------------------------------------------

    /** Handle a controller being connected. */
    private void OnControllerConnected(Rewired.ControllerStatusChangedEventArgs args)
    {
        // Create a new player if needed.
        EnsurePlayersExist();
    }

    /** Ensure a player exists for each Rewired player. */
    private void EnsurePlayersExist()
    {
        var inputPlayers = Rewired.ReInput.players.Players;
        foreach (var inputPlayer in inputPlayers)
            if (inputPlayer.controllers.joystickCount != 0)
                EnsurePlayerExists(inputPlayer.id);
    }

    /** Creates a player with the given id. */
    private void EnsurePlayerExists(int id)
    {
        // Check if player already exists.
        if (_playersById.ContainsKey(id))
            return;

        // Instantiate and configure the player.
        var player = Instantiate(PlayerPrefab);
        _playersById[id] = player;
        player.Configure(id, _game);
        _players.Add(player);

        // Register with the new player.
        RegisterWithPlayer(player);

        // Spawn the player.
        var spawn = GetSafestSpawn();
        player.Spawn(spawn);
    }

    /** Register with a new player. */
    private void RegisterWithPlayer(Player player)
    {
        player.Damaged += OnPlayerDamaged;
        player.Killed += OnPlayerKilled;
        player.OutOfLives += OnPlayerOutOfLives;
    }

    /** Unregister with a player. */
    private void UnregisterWithPlayer(Player player)
    {
        player.Damaged -= OnPlayerDamaged;
        player.Killed -= OnPlayerKilled;
        player.OutOfLives -= OnPlayerOutOfLives;
    }

    /** Handle a player being damaged. */
    private void OnPlayerDamaged(Player player, Damageable damageable, float damage)
    {
        if (Damaged != null)
            Damaged(player, damageable, damage);
    }

    /** Handle a player being killed. */
    private void OnPlayerKilled(Player player)
    {
        if (Killed != null)
            Killed(player);
    }

    /** Handle a player running out of lives. */
    private void OnPlayerOutOfLives(Player player)
    {
        if (OutOfLives != null)
            OutOfLives(player);
    }

}
