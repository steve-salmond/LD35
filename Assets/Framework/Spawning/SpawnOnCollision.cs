using UnityEngine;
using System.Collections;

public class SpawnOnCollision : MonoBehaviour
{
    public LayerMask ContactMask;

    public GameObject EffectPrefab;

    public float InitialDelay;
    public float Cooldown;
    public bool Pooled = true;
    public bool Reparent;

    public Vector2 RelativeVelocityRange = new Vector2(0, 1);
    public bool ScaleVolumeByRelativeVelocity = true;
    public Vector2 VolumeRange = new Vector2(0, 1);

    private float _nextEffectTime;


    private void OnEnable()
    {
        _nextEffectTime = Time.time + InitialDelay;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < _nextEffectTime)
            return;

        if ((ContactMask.value & (1 << collision.gameObject.layer)) == 0)
            return;

        var range = RelativeVelocityRange;
        var v = collision.relativeVelocity.magnitude;
        var t = Mathf.Clamp01((v - range.x) / (range.y - range.x));
        if (t <= 0)
            return;

        _nextEffectTime = Time.time + Cooldown;

        if (EffectPrefab == null)
            return;

        var go = GetInstance();
        if (!go)
            return;

        if (Reparent)
            go.transform.parent = transform;

        go.transform.position = collision.contacts[0].point;

        if (ScaleVolumeByRelativeVelocity && range.y > 0)
        {
            var audio = go.GetComponentInChildren<AudioSource>();
            if (audio)
                audio.volume = Mathf.Lerp(VolumeRange.x, VolumeRange.y, t);
        }
    }

    private GameObject GetInstance()
    {
        if (Pooled)
            return ObjectPool.Get(EffectPrefab);
        else
            return Instantiate(EffectPrefab) as GameObject;
    }

}