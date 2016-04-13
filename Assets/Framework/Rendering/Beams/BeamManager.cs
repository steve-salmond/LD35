using UnityEngine;
using System.Collections.Generic;

public class BeamManager : Singleton<BeamManager> 
{

	// Members
	// ----------------------------------------------------------------------------
	
	public BeamSystem Lasers;


	// Public Methods
	// ----------------------------------------------------------------------------

	/** Add a beam to the manager. */
	public void AddBeam(Beam beam)
	{ 
		switch (beam.Style)
		{
			case BeamStyle.Laser:
				Lasers.AddBeam(beam); break;
        }
	}

	/** Remove a beam from the manager. */
	public void RemoveBeam(Beam beam)
	{ 
		switch (beam.Style)
		{
		case BeamStyle.Laser:
			Lasers.RemoveBeam(beam); break;
		}
	}

}
