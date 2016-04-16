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


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        // Fire up the game control routine.
        StartCoroutine(GameRoutine());
    }


    // Coroutines
    // -----------------------------------------------------

    /** Update the game. */
    private IEnumerator GameRoutine()
    {
        // Reset game state.
        SetState(GameState.None);

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
        Cursor.visible = false;

        // Delay a bit after login.
        yield return new WaitForSeconds(0.5f);
    }

    /** Play the game. */
    private IEnumerator PlayingRoutine()
    {
        SetState(GameState.Playing);

        while (Players.AlivePlayerCount > 0)
            yield return 0;
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
