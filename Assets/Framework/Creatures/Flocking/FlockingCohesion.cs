using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlockingCohesion : FlockingBehavior
{

    protected override void UpdateFromNeigbours(float strength)
    {
        var p = Vector3.zero;
        var n = 0;

        // Obtain center of mass of neighbours.
        var me = transform.position;
        var creatures = CreatureManager.Instance.Creatures;
        var tag = Creature.tag;
        for (var i = 0; i < creatures.Count; i++)
        {
            var c = creatures[i];
            if (c == Creature)
                continue;
            else if (c.tag != tag)
                continue;
            else if (Vector3.Distance(c.transform.position, me) > Radius)
                continue;

            p += c.Body.position;
            n ++;
        }

        if (n <= 0)
            return;

        p /= n;

        // Head towards center of mass.
        var delta = p - Creature.Body.position;

        Creature.Body.AddForce(delta.normalized * strength);
    }

}
