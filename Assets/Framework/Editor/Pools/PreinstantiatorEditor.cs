using UnityEngine;
using UnityEditor;
using System.Collections;

/** Object pool editor. */
[CustomEditor(typeof(Preinstantiator))]
public class PreinstantiatorEditor : Editor
{
	// Unity Implementation
	// ----------------------------------------------------------------------------
	
 	/** Called by the inspector. */ 
    public override void OnInspectorGUI()
	{
		// Draw default inspector.
		DrawDefaultInspector();
 
		// Update props button.
		if (GUILayout.Button("Clear Empty Types")) 
			ClearEmptyTypes((Preinstantiator)target);
	}
	
	
	// Private Methods
	// ----------------------------------------------------------------------------
	
	/** Report current object instantiation state. */
	private void ClearEmptyTypes(Preinstantiator preinstantiator)
	{
		preinstantiator.ClearEmptyTypes();
	}	
}
