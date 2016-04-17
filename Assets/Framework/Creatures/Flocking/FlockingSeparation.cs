using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FlockingSeparation : FlockingBehavior
{

    protected override void UpdateFromNeigbours(float strength)
    {
        var f = Vector3.zero;
        var n = 0;

        // Obtain center of mass of neighbours.
        var creatures = CreatureManager.Instance.Creatures;
        var tag = Creature.tag;
        var me = transform.position;
        for (var i = 0; i < creatures.Count; i++)
        {
            var c = creatures[i];
            if (c == Creature)
                continue;
            else if (c.tag != tag)
                continue;
            else if (Vector3.Distance(c.transform.position, me) > Radius)
                continue;

            f += (c.Body.position - me);
            n++;
        }

        if (n <= 0)
            return;

        // Head away from neighbours.
        Creature.Body.AddForce(-f.normalized * strength);
    }


}
