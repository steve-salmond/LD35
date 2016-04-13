using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PooledEffectManager : Singleton<PooledEffectManager> 
{
    public float Interval = 1;

    private const int InitialCapacity = 2000;

    private List<PooledEffectCleanup> _pending 
        = new List<PooledEffectCleanup>(InitialCapacity);

    private float _nextUpdate;

    private void Update()
    {
        if (Time.time < _nextUpdate)
            return;
        _nextUpdate = Time.time + Interval;

        int n = _pending.Count;

        for (var i = n - 1; i >= 0; i--)
        {
            var p = _pending[i];
            if (!p || !p.gameObject.activeSelf)
            {
                _pending.RemoveAt(i);
                continue;
            }

            if (!p.Returnable)
                continue;

            p.Return();
            _pending.RemoveAt(i);
        }
    }

    public void Register(PooledEffectCleanup effect)
    { _pending.Add(effect); }

}
