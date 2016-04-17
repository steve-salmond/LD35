using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlockingAlignment : FlockingBehavior
{

    protected override void UpdateFromNeigbours(float strength)
    {
        var f = Vector3.zero;
        var n = 0;

        // Determine the set of nearby creatures in same group.
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

            f += c.Body.velocity;
            n ++;
        }

        if (n <= 0)
            return;

        Creature.Body.AddForce(f.normalized * strength);
    }


}
