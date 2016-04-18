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

    /** Event fired when player moves to a new floor. */
    public FloorEventHandler ChangedFloor;

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
        else
            _completed = true;
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

        // Snap camera to initial location.
        var p = Players.Players[0].transform.position;
        CameraController.Instance.SnapTo(p);
        UI.Instance.FadeFrom(1, 2);

        // Hide the mouse cursor.
        // Cursor.visible = false;

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
            if (_floor != Dungeon.CurrentFloor)
                yield return StartCoroutine(ChangeFloor(_floor));
            else if (_completed)
                yield break;

            yield return 0;
        }
    }

    /** Switch floors. */
    private IEnumerator ChangeFloor(Floor floor)
    {
        UI.Instance.Fade(1, 2);
        yield return new WaitForSeconds(2);

        // Update the current floor.
        Dungeon.SetCurrentFloor(_floor);

        // Move player(s) to entrance location.
        var entrance = _floor.EntranceRoom.GetComponentInChildren<Entrance>();
        var p = entrance.transform.position;
        foreach (var player in Players.Players)
            if (player.HasControlled)
                player.Controlled.transform.position = p;

        // Snap camera to new location.
        CameraController.Instance.SnapTo(p);
        UI.Instance.Fade(0, 2);

        // Fire floor change notification.
        if (ChangedFloor != null)
            ChangedFloor(this, floor);
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
