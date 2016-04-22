using UnityEngine;
using UnityEngine.SceneManagement;
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

    /** The current floor. */
    public static int CurrentFloorIndex = 0;


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


    // Events
    // -----------------------------------------------------

    /** Generic game manager event handler. */
    public delegate void GameEventHandler(GameManager game);

    /** Dungeon floor event handler. */
    public delegate void FloorEventHandler(GameManager game, Floor floor);

    /** Event fired when game starts. */
    public GameEventHandler Started;

    /** Event fired when game ends. */
    public GameEventHandler Ended;


    // Members
    // -----------------------------------------------------

    /** Time at which match started. */
    private float _matchStartTime;

    /** Time at which match will end. */
    private float _matchEndTime;

    /** The current dungeon floor. */
    private Floor _floor;

    /** Whether player has completed the game. */
    private bool _completed = false;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        // Hide the mouse cursor.
        #if !UNITY_WEBGL
        Cursor.visible = false;
        #endif

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
        {
            CurrentFloorIndex++;
            _floor = next;
        }
        else
        {
            CurrentFloorIndex = 0;
            _completed = true;
        }
    }


    // Coroutines
    // -----------------------------------------------------

    /** Update the game. */
    private IEnumerator GameRoutine()
    {
        // Reset game state.
        SetState(GameState.None);

        // Generate dungeon.
        var seed = Config.Seed;
        if (Config.Randomize)
            seed = Random.Range(0, int.MaxValue);
        Dungeon.Generate(seed);
        Dungeon.SetCurrentFloor(CurrentFloorIndex);
        _floor = Dungeon.CurrentFloor;

        // Configure players.
        Players.Configure(this);

        // Snap camera to initial location.
        yield return 0;
        var p = Players.Players[0].transform.position;
        CameraController.Instance.SnapTo(p);
        UI.Instance.FadeIn(2);

        // First, play intro.
        yield return StartCoroutine(IntroRoutine());
        yield return StartCoroutine(PlayingRoutine());
        yield return StartCoroutine(GameOverRoutine());
    }

    /** Play the game intro. */
    private IEnumerator IntroRoutine()
    {
        SetState(GameState.Intro);
        yield return 0;
    }

    /** Play the game. */
    private IEnumerator PlayingRoutine()
    {
        SetState(GameState.Playing);

        // Fire game started event.
        if (Started != null)
            Started(this);

        // Start the game music.
        MusicManager.Instance.Game();

        // Keep playing until game ends.
        while (Players.AlivePlayerCount > 0)
        {
            if (_completed)
                yield break;
            else if (_floor != Dungeon.CurrentFloor)
                yield return StartCoroutine(ChangeFloor(_floor));

            yield return 0;
        }
    }

    /** Switch floors. */
    private IEnumerator ChangeFloor(Floor floor)
    {
        UI.Instance.FadeOut(2);
        yield return new WaitForSeconds(2);

        // Reload the game scene to get to next floor.
        SceneManager.LoadScene("Game");
    }

    /** Handle the game over condition. */
    private IEnumerator GameOverRoutine()
    {
        SetState(GameState.GameOver);

        // Fire game ended notification.
        if (Ended != null)
            Ended(this);

        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Intro");
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
