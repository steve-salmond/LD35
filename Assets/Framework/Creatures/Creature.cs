using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Creature : MonoBehaviour
{
    public Rigidbody Body;

    protected virtual void OnEnable()
    {
        CreatureManager.Instance.Register(this);

        if (!Body)
            Body = GetComponent<Rigidbody>();
    }

    protected virtual void OnDisable()
    {
        if (CreatureManager.HasInstance)
            CreatureManager.Instance.Unregister(this);
    }

}
