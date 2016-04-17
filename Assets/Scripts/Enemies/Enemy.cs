using UnityEngine;
using System.Collections;

public class Enemy : Procedural
{

    public Rigidbody Body;

    public Animator Animator;

    public float MovingThreshold = 1;

    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
        if (!Animator)
            Animator = GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (Body && Animator)
        {
            var speed = Body.velocity.magnitude;
            Animator.SetBool("Moving", speed > MovingThreshold);
        }
    }
}
