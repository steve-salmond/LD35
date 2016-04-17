using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class FlockingBehavior : MonoBehaviour
{

    public Creature Creature;

    public string GroupTag;

    public float Radius = 10;

    public float Strength = 1;

    public int Interval = 5;

    private int _frame;
    private float _strength;


    protected virtual void Awake()
    {
        if (!Creature)
            Creature = GetComponent<Creature>();

        _frame = Random.Range(0, Interval);
        _strength = Strength * Interval;
    }

    protected virtual void FixedUpdate()
    {
        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        // Update behavior using neighbours.
        UpdateFromNeigbours(_strength);
    }

    protected abstract void UpdateFromNeigbours(float strength);
	
}
