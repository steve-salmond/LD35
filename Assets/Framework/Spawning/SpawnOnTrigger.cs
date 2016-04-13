using UnityEngine;
using System.Collections;

public class SpawnOnTrigger : MonoBehaviour
{
    public LayerMask ContactMask;

    public GameObject EffectPrefab;

    public float InitialDelay;
    public float Cooldown;
    public bool Pooled = true;
    public bool Reparent;

    private float _nextEffectTime;


    private void OnEnable()
    {
        _nextEffectTime = Time.time + InitialDelay;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (Time.time < _nextEffectTime)
            return;

        if ((ContactMask.value & (1 << collider.gameObject.layer)) == 0)
            return;

        _nextEffectTime = Time.time + Cooldown;

        if (EffectPrefab == null)
            return;

        var go = GetInstance();
        if (Reparent)
            go.transform.parent = transform;

        go.transform.position = transform.position;
    }

    private GameObject GetInstance()
    {
        if (Pooled)
            return ObjectPool.Get(EffectPrefab);
        else
            return Instantiate(EffectPrefab) as GameObject;
    }

}