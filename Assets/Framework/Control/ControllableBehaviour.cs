using UnityEngine;
using System.Collections;

public abstract class ControllableBehaviour : MonoBehaviour, Controllable
{

    // Updating
    // -----------------------------------------------------

    [Header("Updating")]

    /** Whether to update every render frame. */
    public ControllableUpdateMode UpdateMode = ControllableUpdateMode.FixedUpdate;


    // Properties
    // -----------------------------------------------------

    /** Sets the entity's controller. */
    public Controller Controller
    {
        get
        {
            return _controller;
        }

        set
        {
            if (_controller == value)
                return;

            if (_controller != null)
                UnregisterWithController();

            var old = _controller;
            _controller = value;

            if (_controller != null)
                RegisterWithController();

            if (ControllerChanged != null)
                ControllerChanged(this, old, value);
        }
    }

    /** Whether a controller has possessed this entity. */
    public bool Controlled
    { get { return Controller != null; } }


    // Events
    // -----------------------------------------------------

    /** An event relating to a controller. */
    public delegate void ControllerChangeEventHandler(ControllableBehaviour controllable, Controller old, Controller value);

    /** Event fired when the controller changes. */
    public ControllerChangeEventHandler ControllerChanged;


    // Members
    // -----------------------------------------------------

    /** The current controller for this entity. */
    private Controller _controller;

    /** Whether a physics update has occurred during the current render frame. */
    private bool _fixedUpdated;



    // Unity Methods
    // -----------------------------------------------------

    /** Update on every physics frame. */
    protected virtual void FixedUpdate()
    {
        // Mark a fixed update as having occurred during the current frame.
        _fixedUpdated = true;

        // Check that we have a controller.
        if (UpdateMode == ControllableUpdateMode.FixedUpdate && Controller != null)
            UpdateControllable(Controller);
    }

    /** Update on every render frame. */
    protected virtual void Update()
    {
        // Check that we have a controller.
        if (UpdateMode == ControllableUpdateMode.Update && Controller != null)
            UpdateControllable(Controller);
        else if (UpdateMode == ControllableUpdateMode.UpdateIfFixedUpdated && Controller != null && _fixedUpdated)
            UpdateControllable(Controller);
    }

    /** Update after regular updating, just before rendering. */
    protected virtual void LateUpdate()
    {
        // Check that we have a controller.
        if (UpdateMode == ControllableUpdateMode.LateUpdate && Controller != null)
            UpdateControllable(Controller);
        else if (UpdateMode == ControllableUpdateMode.LateUpdateIfFixedUpdated && Controller != null && _fixedUpdated)
            UpdateControllable(Controller);

        // Clear physics updated flag..
        _fixedUpdated = false;
    }

    /** Handle the object being destroyed. */
    protected virtual void OnDestroy()
    {

    }


    // Protected Methods
    // -----------------------------------------------------

    /** Update the controllable behavior. */
    protected abstract void UpdateControllable(Controller controller);

    /** Register with a controller. */
    protected virtual void RegisterWithController()
    {
    }

    /** Unregister with a controller. */
    protected virtual void UnregisterWithController()
    {
    }

}
