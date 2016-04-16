using UnityEngine;
using System.Collections;

public class Exit : UseableBehaviour
{

    public override void Use()
    {
        GameManager.Instance.NextFloor();
    }

}
