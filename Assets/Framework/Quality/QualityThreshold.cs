using UnityEngine;


/** Activates or deactivates functionality based on the quality level and threshold. */

public abstract class QualityThreshold : MonoBehaviour
{
	// Enumerations
	// ----------------------------------------------------------------------------

	/** The set of possible quality levels. */
	public enum Level 
	{ 
		VeryLow = 0, 
		Low = 1, 
		Medium = 2, 
		High = 3, 
		VeryHigh = 4
	};

	
	// Properties
	// ----------------------------------------------------------------------------

	/** Minimum quality level for the game object to be active. */
	public Level MinQuality = Level.Low;
	
	/** Maximum quality level for the game object to be active. */
	public Level MaxQuality = Level.VeryHigh;
	
	
	// Unity Implementation
	// ----------------------------------------------------------------------------
	
	/** Initialize the component. */
	protected virtual void Awake() 
	{
		// Perform an initial update.
		UpdateActive();
	}

    /** Enable the component. */
    protected virtual void OnEnable()
    {
        // Perform an initial update.
        UpdateActive();
    }


    // Protected Methods
    // ----------------------------------------------------------------------------

    /** Activate or deactivate the item. */
    protected abstract void SetActive(bool value);


    // Private Methods
    // ----------------------------------------------------------------------------

    /** Update the active status. */
    private void UpdateActive()
	{
		// Get the current quality level and threshold values.
		int quality = QualitySettings.GetQualityLevel();
		int min = (int) MinQuality;
		int max = (int) MaxQuality;
		
		// Activate if within the desired range, otherwise, deactivate.
		SetActive(quality >= min && quality <= max);
	}
}
