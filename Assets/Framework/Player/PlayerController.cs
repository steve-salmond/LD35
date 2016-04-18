using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class PlayerController : Controller
{

    // Properties
    // -----------------------------------------------------

    /** Whether to fall back to movement axis when determining aim direction. */
    public bool AimFallbackToMovement = true;

    /** The player for this controller. */
    public Player Player
    { get; private set; }

    /** Theme color for this controller. */
    public Color Color
    { get { return Player.Color; } }


    // Members
    // -----------------------------------------------------

    /** The player's input source. */
    private Rewired.Player _input;

    /** Player's plane of motion. */
    private Plane _plane = new Plane(Vector3.forward, 0);

    /** Current vibration tween. */
    private Dictionary<string, Tween> _vibrateTweens = new Dictionary<string, Tween>();


    // Lifecycle
    // -----------------------------------------------------

    /** Constructor. */
    public PlayerController(Player player)
    {
        Player = player;

        // Retrieve the player input source from Rewired.
        _input = Rewired.ReInput.players.GetPlayer(player.Id);

        // Register for player events.
        player.Damaged += OnPlayerDamaged;
        player.Killed += OnPlayerKilled;
        player.CausedDamage += OnPlayerCausedDamage;
        player.CausedKill += OnPlayerCausedKill;
    }

    /** Reset controller to default state. */
    public void Reset()
    {
        // Stop any current rumbling.
        VibrateStop();
    }


    // Controller implementation
    // -----------------------------------------------------

    public bool GetButton(string name)
    { return _input.GetButton(name); }

    public bool GetButtonDown(string name)
    { return _input.GetButtonDown(name); }

    public bool GetButtonUp(string name)
    { return _input.GetButtonUp(name); }

    public float GetAxis(string name)
    { return _input.GetAxis(name); }

    /** Determine what kind of aiming method this controller uses. */
    public AimMethod GetAimMethod()
    {
        return AimMethod.Direction; 

        /*
        if (_input.controllers.joystickCount > 0)
            return AimMethod.Direction;
        else if (_input.controllers.hasMouse)
            return AimMethod.Point;
        else
            return AimMethod.Direction;
        */
    }

    /** Return the controller's current direction vector (input space). */
    public Vector3 GetAimDirection()
    {
        // Look for explicit aim input from player.
        var direction = GetAimDirectionFromAxes("Aim Horizontal", "Aim Vertical");

        // If player is not explicitly aiming, use their movement input for aim.
        if (AimFallbackToMovement && Mathf.Approximately(direction.magnitude, 0))
            direction = GetAimDirectionFromAxes("Move Horizontal", "Move Vertical");

        return direction;
    }

    /** Return the controller's current aim point (world space). */
    public Vector3 GetAimPoint()
    {
        // Get mouse location on screen.
        var mouse = _input.controllers.Mouse;
        Vector3 screen = mouse.screenPosition;

        // Convert the mouse position into a point into the XY plane.
        var camera = CameraController.Instance.Camera;
        var ray = camera.ScreenPointToRay(screen);
        var d = 0.0f;
        _plane.Raycast(ray, out d);
        return ray.GetPoint(d);
    }

    /** Whether controller is actively aiming. */
    public bool IsAiming()
    {
        var method = GetAimMethod();
        if (method == AimMethod.Direction)
        {
            var direction = GetAimDirection();
            return !Mathf.Approximately(direction.sqrMagnitude, 0);
        }
        else
            return true;
    }


    // Private Methods
    // -----------------------------------------------------

    /** Get aim direction from the specified input axes. */
    private Vector3 GetAimDirectionFromAxes(string horizontal, string vertical)
    {
        var dx = GetAxis(horizontal);
        var dy = GetAxis(vertical);
        var direction = new Vector3(dx, dy);
        var length = direction.magnitude;

        if (length > 1)
            direction /= length;

        return direction;
    }

    /** Handle this player killing another player. */
    private void OnPlayerCausedKill(Player player)
    { VibrateRight(1.0f, 1.0f); }

    /** Handle this player damaging another player. */
    private void OnPlayerCausedDamage(Player player, Damageable damageable, float damage)
    {
        var strength = damage / damageable.StartingHealth;
        VibrateRight(1.0f, strength);
    }

    /** Handle this player getting killed. */
    private void OnPlayerKilled(Player player)
    { VibrateLeft(1.0f, 1.0f); }

    /** Handle this player getting damaged. */
    private void OnPlayerDamaged(Player player, Damageable damageable, float damage)
    {
        var strength = damage / damageable.StartingHealth;
        VibrateLeft(1.0f, strength);
    }

    /** Apply vibration to the player's joystick (left motor). */
    public void VibrateLeft(float level, float duration)
    { Vibrate(0, level, duration); }

    /** Apply vibration to the player's joystick (right motor). */
    public void VibrateRight(float level, float duration)
    { Vibrate(1, level, duration); }

    /** Apply vibration to the player's joystick. */
    private void Vibrate(int id, float level, float duration)
    {
        level = Mathf.Clamp01(level);
        foreach (var j in _input.controllers.Joysticks)
        {
            if (!j.supportsVibration)
                continue;

            var motor = Mathf.Clamp(id, 0, j.vibrationMotorCount - 1);
            j.SetVibration(motor, level);

            var key = j.id + ":" + motor;
            if (_vibrateTweens.ContainsKey(key))
                _vibrateTweens[key].Kill();

            _vibrateTweens[key] = DOTween.To(
                () => j.GetVibration(motor),
                v => j.SetVibration(motor, v), 0, duration)
                .SetUpdate(UpdateType.Normal, true);
        }
    }

    /** Reset vibration for the player's joystick. */
    private void VibrateStop()
    {
        foreach (var tween in _vibrateTweens.Values)
            tween.Kill();

        foreach (var j in _input.controllers.Joysticks)
        {
            if (!j.supportsVibration)
                continue;
            for (var id = 0; id < j.vibrationMotorCount; id++)
                j.SetVibration(id, 0);
        }
    }

}
