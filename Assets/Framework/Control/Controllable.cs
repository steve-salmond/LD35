using UnityEngine;
using System.Collections;

public interface Controllable
{

    /** Sets the controller for this controllable entity. */
    Controller Controller
    { get; set; }

}
