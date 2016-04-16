using UnityEngine;
using System.Collections;

public interface Controller
{

    // Identity
    // -----------------------------------------------------

    /** Theme color for this controller. */
    Color Color { get; }

    // Input
    // -----------------------------------------------------

    /** Whether the given button is currently pressed. */
    bool GetButton(string name);

    /** Whether the given button was pressed this frame. */
    bool GetButtonDown(string name);

    /** Whether the given button was released this frame. */
    bool GetButtonUp(string name);

    /** Current value for the given input axis. */
    float GetAxis(string name);


    // Aiming
    // -----------------------------------------------------

    /** Determine what kind of aiming method this controller uses. */
    AimMethod GetAimMethod();

    /** Return the controller's current aim direction (input space). */
    Vector3 GetAimDirection();

    /** Return the controller's current aim point (world space). */
    Vector3 GetAimPoint();

    /** Whether controller is actively aiming. */
    bool IsAiming();


}
