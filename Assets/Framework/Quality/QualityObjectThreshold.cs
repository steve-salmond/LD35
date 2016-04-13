using UnityEngine;


/** Activates or deactivates a game object  based on the quality level and threshold. */

public class QualityObjectThreshold : QualityThreshold
{

    public GameObject[] Objects;

    protected override void Awake()
    {
        base.Awake();

        if (Objects.Length == 0)
            Objects = new GameObject[] { gameObject };
    }

    protected override void SetActive(bool value)
    {
        foreach (var go in Objects)
            go.SetActive(value);
    }
}
