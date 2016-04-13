using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Preinstantates game objects to avoid instantiation during the game. */

public class Preinstantiator : MonoBehaviour
{
	// Types
	// ----------------------------------------------------------------------------

	/** A game object / instance count pair. */
	[System.Serializable]
	public class InstanceCount
	{
		/** The name of the object. */
		public string Name;
		
		/** The template object to instantiate. */
		public GameObject Template;
	
		/** The number of instances to create. */
		public int Count;
		
		/** Constructor. */
		public InstanceCount(GameObject template, int count)
		{
			Name = template.name; 
			Template = template;
			Count = count;
		}
	}	

	
	// Properties
	// ----------------------------------------------------------------------------
	
	/** The list of game object / instance count pairs. */
	public List<InstanceCount> InstanceCounts;
	

	// Unity Implementation
	// ----------------------------------------------------------------------------
	
	/** Preinitialize. */
	void Awake()
	{
		// Preinstantiate.
		Preinstantiate();
	}
	
	/** Initialize. */
	void Start()
	{
	}
	
	/** Update. */
	void Update()
	{
	}
	
	
	// Public Interface
	// ----------------------------------------------------------------------------
	
	/** Populate instance counts from a a dictionary. */
	public void AddInstanceCounts(Dictionary<GameObject, int> counts)
	{
		// Create the list.
		InstanceCounts = new List<InstanceCount>();

		// Add each entry.
		foreach (KeyValuePair<GameObject, int> pair in counts)
			InstanceCounts.Add(new InstanceCount(pair.Key, pair.Value));
	}

	/** Clear entries that have an empty type. */
	public void ClearEmptyTypes()
	{
		// Store removed entries.
		List<InstanceCount> removed = new List<InstanceCount>();

		// Find entries will null types.
		foreach (InstanceCount i in InstanceCounts)
		{
			if (i.Template == null)
				removed.Add(i);
		}

		// Remove entries.
		foreach (InstanceCount i in removed)
			InstanceCounts.Remove(i);
	}


	// Private Methods
	// ----------------------------------------------------------------------------
	
	/** Preinstatiate. */
	private void Preinstantiate()
	{
		// Check that there are instance counts available.
		if (InstanceCounts == null)
			return;
	
		// Create inactive instances of each object type.
		foreach (InstanceCount i in InstanceCounts)
		{
			// Check that the object is valid.
			if (i.Template == null)
			{
				if (i.Name != null && i.Name.Length > 0)
					Debug.LogWarning("Unable to instantiate object '" + i.Name + "'");
				
				continue;
			}

			// Instantiate.
			ObjectPool.Instance.Preinstantiate(i.Template, i.Count, false);
		}
	}
}
