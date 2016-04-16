using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** Cooldown for spawning. */
    public float Cooldown = 0.5f;

    /** Offset for checking if a spawn-point is clear. */
    public Vector3 CanSpawnOffset;

    /** Radius for checking if a spawn-point is clear. */
    public float CanSpawnRadius = 1;

    /** Layer mask for detecting things that obstruct spawning. */
    public LayerMask CanSpawnMask;


    // Members
    // -----------------------------------------------------

    /** Timestamp of next spawn usage. */
    private float _nextSpawnTime;


    // Unity Methods
    // -----------------------------------------------------

    /** Enabling. */
    private void OnEnable()
    {
        GameManager.Instance.Players.AddSpawn(this);
	}

    /** Disabling. */
    private void OnDisable()
    {
        if (GameManager.HasInstance)
            GameManager.Instance.Players.RemoveSpawn(this);
    }


    // Public Methods
    // -----------------------------------------------------

    /** Determine if a player can spawn here at present. */
    public bool CanSpawn()
    {
        if (Time.time < _nextSpawnTime)
            return false;

        var p = transform.TransformPoint(CanSpawnOffset);
        var obstructed = Physics.CheckSphere(p, CanSpawnRadius, CanSpawnMask);
        return !obstructed;
    }

    /** Spawn an instance of the given prefab. */
    public void Spawn(GameObject go)
    {
        go.transform.position = transform.position;
        // go.transform.rotation = transform.rotation;
        _nextSpawnTime = Time.time + Cooldown;
    }

}
