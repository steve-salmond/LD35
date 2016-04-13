using UnityEngine;
using UnityEditor;
using System.Collections;

/** Object pool editor. */
[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor
{
	// Unity Implementation
	// ----------------------------------------------------------------------------
	
 	/** Called by the inspector. */ 
    public override void OnInspectorGUI()
	{
		// Draw default inspector.
		DrawDefaultInspector();
 
		// Update props button.
		if (GUILayout.Button("Report")) 
			Report((ObjectPool)target);
		
		// Serialize button.
        if (GUILayout.Button("Generate Preinstantiator")) 
			GeneratePreinstantiator((ObjectPool)target);
	}
	
	
	// Private Methods
	// ----------------------------------------------------------------------------
	
	/** Report current object instantiation state. */
	private void Report(ObjectPool pool)
	{
		pool.Report();
	}	

	/** Generate a preinstantiator. */
	private void GeneratePreinstantiator(ObjectPool pool)
	{
		pool.GeneratePreinstantiator();
	}	
}
