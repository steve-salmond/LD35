using UnityEngine;
using System.Collections;

public class Entrance : UseableBehaviour
{

    public override void Use()
    {
        GameManager.Instance.PreviousFloor();
    }
}
