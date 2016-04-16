using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Singleton<GameManager>
{

    // Properties
    // -----------------------------------------------------

    /** The game's current state. */
    public GameState State
    { get; private set; }

    /** Whether a match is in progress. */
    public bool Playing
    { get { return State == GameState.Playing; } }

    /** Whether match is over. */
    public bool GameOver
    { get { return State == GameState.GameOver; } }

    /** The winner(s) of the game. */
    public List<Player> Winners
    { get; private set; }


    // Components
    // -----------------------------------------------------

    [Header("Components")]

    /** The player manager. */
    public PlayerManager Players;

    /** The dungeon to play in. */
    public Dungeon Dungeon;


    // Configuration
    // -----------------------------------------------------

    [Header("Configuration")]

    /** Match configuration. */
    public GameConfig Config;



    // Members
    // -----------------------------------------------------

    /** Time at which match started. */
    private float _matchStartTime;

    /** Time at which match will end. */
    private float _matchEndTime;

    /** The current dungeon floor. */
    private Floor _floor;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        // Fire up the game control routine.
        StartCoroutine(GameRoutine());
    }


    // Public Methods
    // -----------------------------------------------------

    /** Try to move to the next floor in the dungeon. */
    public void NextFloor()
    {
        var next = Dungeon.CurrentFloor.Next;
        if (next != null)
            _floor = next;
    }

    /** Try to move to the previous floor in the dungeon. */
    public void PreviousFloor()
    {
        var previous = Dungeon.CurrentFloor.Previous;
        if (previous != null)
            _floor = previous;
    }


    // Coroutines
    // -----------------------------------------------------

    /** Update the game. */
    private IEnumerator GameRoutine()
    {
        // Reset game state.
        SetState(GameState.None);

        // Generate dungeon.
        Dungeon.Generate(Config.Seed);
        _floor = Dungeon.CurrentFloor;

        // Configure players.
        Players.Configure(this);

        // First, play intro.
        yield return StartCoroutine(IntroRoutine());
        yield return StartCoroutine(PlayingRoutine());
        yield return StartCoroutine(GameOverRoutine());
    }

    /** Play the game intro. */
    private IEnumerator IntroRoutine()
    {
        SetState(GameState.Intro);

        // Hide the mouse cursor.
        // Cursor.visible = false;

        // Delay a bit after login.
        yield return new WaitForSeconds(0.5f);
    }

    /** Play the game. */
    private IEnumerator PlayingRoutine()
    {
        SetState(GameState.Playing);

        while (Players.AlivePlayerCount > 0)
        {
            if (_floor != Dungeon.CurrentFloor)
                yield return StartCoroutine(ChangeFloor(_floor));

            yield return 0;
        }
    }

    /** Switch floors. */
    private IEnumerator ChangeFloor(Floor floor)
    {
        // TODO: Fade to black.
        yield return new WaitForSeconds(1);

        // Update the current floor.
        Dungeon.SetCurrentFloor(_floor);

        // Move player(s) to entrance location.
        var entrance = _floor.EntranceRoom.GetComponentInChildren<Entrance>();
        foreach (var player in Players.Players)
            if (player.HasControlled)
                player.Controlled.transform.position = entrance.transform.position;

        // TODO: Fade from black.
    }

    /** Handle the game over condition. */
    private IEnumerator GameOverRoutine()
    {
        yield return 0;
    }


    // Implementation
    // -----------------------------------------------------

    /** Change the current game state. */
    private void SetState(GameState value)
    {
        // Check if state has changed.
        if (State == value)
            return;

        // Assign the new state.
        State = value;
    }

}
