using UnityEngine;
using System.Collections;

public enum ControllableUpdateMode 
{
    FixedUpdate,
    Update,
    LateUpdate,
    UpdateIfFixedUpdated,
    LateUpdateIfFixedUpdated
}
