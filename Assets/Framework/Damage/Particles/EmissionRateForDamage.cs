using UnityEngine;
using System.Collections;

public class EmissionRateForDamage : MonoBehaviour
{
    public Damageable Damageable;
    public AnimationCurve Curve;

    private ParticleSystem _system;

    private float _emissionRate = -1;

	private void Start()
    {
        _system = GetComponent<ParticleSystem>();
        UpdateRate();
    }

    private void Update()
    {
        UpdateRate();
    }

    private void UpdateRate()
    {
        var rate = Curve.Evaluate(Damageable.DamageFraction);
        SetEmissionRate(rate);
    }

    private void SetEmissionRate(float value)
    {
        if (value == _emissionRate)
            return;

        var emission = _system.emission;
        var rate = new ParticleSystem.MinMaxCurve();
        rate.constantMax = value;
        emission.rate = rate;

        _emissionRate = value;
    }
}
