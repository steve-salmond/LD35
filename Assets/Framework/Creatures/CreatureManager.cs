using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureManager : Singleton<CreatureManager>
{

    public List<Creature> Creatures
    { get { return _creatures; } }

    private List<Creature> _creatures = new List<Creature>();

    public void Register(Creature creature)
    {
        Creatures.Add(creature);
    }

    public void Unregister(Creature creature)
    {
        Creatures.Remove(creature);
    }


}
