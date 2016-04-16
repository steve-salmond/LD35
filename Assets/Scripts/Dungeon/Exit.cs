using UnityEngine;
using System.Collections;

public class Exit : UseableBehaviour
{

    public override void Use(PlayerControllable pc)
    {
        GameManager.Instance.NextFloor();
    }

}
