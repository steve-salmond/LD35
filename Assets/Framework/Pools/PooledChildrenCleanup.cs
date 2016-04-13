using UnityEngine;
using System.Collections;

public class PooledChildrenCleanup : MonoBehaviour 
{

    private void OnDisable()
    {
        var n = transform.childCount;
        for (var i = n - 1; i >= 0; i--)
            ObjectPool.Cleanup(transform.GetChild(i).gameObject);
    }

}
