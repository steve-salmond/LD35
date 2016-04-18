using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    public Rigidbody Body;

    public MeleeControllable Melee;
    public JumpingControllable Jumping;
    public GroundControllable Groundable;
    public ClimbingControllable Climbing;

    public Animator Animator;

    public float MovingThreshold = 1;

    public Transform Feet;
    public GameObject FootstepEffectPrefab;
    public GameObject LadderstepEffectPrefab;

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

    private void Footstep()
    {
        ObjectPool.GetAt(FootstepEffectPrefab, Feet, false);
    }

    private void LadderStep()
    {
        ObjectPool.GetAt(LadderstepEffectPrefab, Body.transform, false);
    }

    private void LateUpdate()
    {

        var speed = Mathf.Abs(Body.velocity.x);
        if (Climbing.Ladder != null)
            speed = Mathf.Abs(Body.velocity.y);

        Animator.SetBool("Moving", speed > MovingThreshold);
        Animator.SetBool("Falling", Groundable.Falling);
        Animator.SetBool("Grounded", Groundable.Grounded);
        Animator.SetBool("Climbing", Climbing.Ladder != null);
    }
}