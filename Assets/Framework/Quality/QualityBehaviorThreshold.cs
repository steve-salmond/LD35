using UnityEngine;


/** Activates or deactivates a game object based on the quality level and threshold. */

public class QualityBehaviorThreshold : QualityThreshold
{

    public MonoBehaviour[] Behaviors;

    protected override void SetActive(bool value)
    {
        foreach (var behavior in Behaviors)
            behavior.enabled = value;
    }

}
