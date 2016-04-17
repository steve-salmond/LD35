using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{

    public Rigidbody Body;

    public MeleeControllable Melee;
    public JumpingControllable Jumping;
    public GroundControllable Groundable;

    public Animator Animator;

    public float MovingThreshold = 1;

    private void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
        if (!Animator)
            Animator = GetComponentInChildren<Animator>();

        Melee.Attacked += OnAttacked;
        Jumping.Jumped += OnJumped;
    }

    private void OnAttacked()
    {
        Animator.SetTrigger("Attack");
    }

    private void OnJumped()
    {
        Animator.SetTrigger("Jump");
    }

    private void LateUpdate()
    {
        var speed = Body.velocity.magnitude;
        Animator.SetBool("Moving", speed > MovingThreshold);
        Animator.SetBool("Falling", Groundable.Falling);
    }

}
