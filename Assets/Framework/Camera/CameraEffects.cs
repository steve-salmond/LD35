using UnityEngine;
using System.Collections;

using Kino;
using DG.Tweening;

public class CameraEffects : Singleton<CameraEffects> 
{

    // Members
    // -----------------------------------------------------

    /** The camera instance. */
    private Camera _camera;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start() 
	{
        _camera = GetComponent<Camera>();

        GameManager.Instance.Players.Damaged += OnPlayerDamaged;
    }


    // Private Methods
    // -----------------------------------------------------

    /** Handle a player getting damaged. */
    private void OnPlayerDamaged(Player player, Damageable damageable, float damage)
    {
        Shake(damage / damageable.StartingHealth);
    }

    /** Screen shake. */
    public void Shake(float amount, float duration = 0.25f)
    {
        _camera.DOKill();
        _camera.DOShakePosition(duration, amount * 3);
    }


}
