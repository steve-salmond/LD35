using UnityEngine;
using System.Collections;
using System.Linq;

/** Spawns objects in a stochastic fashion. */

public abstract class SpawnStochastic : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnConfig
    {
        public GameObject Prefab;
        public float Weight;
    }

    public SpawnConfig[] Objects;

    public Vector2 IntervalRange = Vector2.one;

    public float ClearanceRadius = 1;
    public LayerMask ClearanceMask;

    public Vector2 SpawnCountRange = Vector2.one;
    public int SpawnAttempts = 1;

    public bool AutoSpawn = true;
    public bool Pooled;
    public bool Reparent;

    public Vector2 InitialSpawnCount = Vector2.zero;

    public Vector2 ScaleRange = Vector2.one;
    public bool AffectScale = true;


    private float _totalWeight;

    public bool Scheduled
    { get; private set; }

    private void OnEnable()
    {
        _totalWeight = Objects.Sum(x => x.Weight);

        if (AutoSpawn)
            ScheduleNextSpawn();

        var min = Mathf.RoundToInt(InitialSpawnCount.x);
        var max = Mathf.RoundToInt(InitialSpawnCount.y);
        var n = Random.Range(min, max + 1);
        for (var i = 0; i < n; i++)
            Spawn();
    }

    public void ScheduleNextSpawn()
    {
        var interval = Random.Range(IntervalRange.x, IntervalRange.y);

        StopAllCoroutines();
        StartCoroutine(SpawnRoutine(interval));

        Scheduled = true;
    }

    private IEnumerator SpawnRoutine(float interval)
    {
        yield return new WaitForSeconds(interval);
        Spawn();
    }

    public void Spawn()
    {
        // Determine how many objects to spawn.
        int min = Mathf.RoundToInt(SpawnCountRange.x);
        int max = Mathf.RoundToInt(SpawnCountRange.y);
        var n = Random.Range(min, max + 1);

        // Spawn the desired number of objects.
        for (var i = 0; i < n; i++)
            SpawnSingle();

        // Schedule next spawn event.
        Scheduled = false;
        if (AutoSpawn)
            ScheduleNextSpawn();
    }

    private void SpawnSingle()
    {
        for (var i = 0; i < SpawnAttempts; i++)
            if (SpawnAttempt())
                return;
    }

    private bool SpawnAttempt()
    {
        // Determine where to spawn this object.
        Vector3 p = GetSpawnLocation();

        // Check that the selected spawn location is clear.
        if (ClearanceMask != 0 && Physics.CheckSphere(p, ClearanceRadius, ClearanceMask))
            return false;

        // Randomize rotation and scale.
        var theta = Random.Range(0, 360);
        var r = transform.rotation * Quaternion.AngleAxis(theta, Vector3.up);
        var scale = Random.Range(ScaleRange.x, ScaleRange.y);

        // Pick object to spawn using weighted random selection.
        var n = Objects.Length;
        var t = Random.Range(0, _totalWeight);
        var w = 0.0f;
        GameObject prefab = null;
        for (var i = 0; i < n; i++)
        {
            if (w > t)
                break;

            prefab = Objects[i].Prefab;
            w += Objects[i].Weight;
        }

        // Instantiate the object.
        var go = GetInstance(prefab);
        if (Reparent)
            go.transform.parent = transform;

        go.transform.position = p;
        go.transform.rotation = r;

        if (AffectScale)
            go.transform.localScale = Vector3.one * scale;

        return true;
    }

    private GameObject GetInstance(GameObject prefab)
    {
        if (Pooled)
            return ObjectPool.Get(prefab);
        else
            return Instantiate(prefab) as GameObject;
    }

    protected abstract Vector3 GetSpawnLocation();
}
