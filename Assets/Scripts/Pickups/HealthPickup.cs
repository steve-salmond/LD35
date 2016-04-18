using UnityEngine;
using System.Collections;

public class HealthPickup : Pickup
{

    public float Health = 10;

    public override bool Use(GameObject other)
    {
        if (!base.Use(other))
            return false;

        var player = other.GetComponent<PlayerControllable>();
        if (player != null)
            player.Damageable.Heal(Health, GetComponent<Damager>());

        return true;
    }

}
