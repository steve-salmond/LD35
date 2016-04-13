using UnityEngine;
using System.Collections.Generic;

public class BeamSystem : MonoBehaviour
{

	// Members
	// ----------------------------------------------------------------------------
	
	/** The set of active beams. */
	private readonly List<Beam> _beams = new List<Beam>();

	/** The set of beam particles. */
	private ParticleSystem.Particle[] _particles;

    /** The beam particle system. */
    private ParticleSystem _particleSystem;



	// Public Methods
	// ----------------------------------------------------------------------------

	/** Add a beam to the manager. */
	public void AddBeam(Beam beam)
	{ 
		// Make sure we don't already have this beam.
		if (!_beams.Contains(beam))
			_beams.Add(beam); 
	}

	/** Remove a beam from the manager. */
	public void RemoveBeam(Beam beam)
		{ _beams.Remove(beam); }


	// Unity Methods
	// ----------------------------------------------------------------------------

	/** Preinitialization. */
	private void Awake()
	{
	    _particleSystem = GetComponent<ParticleSystem>();
        int n = _particleSystem.maxParticles;
        _particles = new ParticleSystem.Particle[n];

		// Add a single dummy beam to keep system alive.
		var beam = new Beam();
		beam.Color = new Color(0, 0, 0, 0);
		beam.Start = Vector3.zero;
		beam.End = Vector3.zero;
		_beams.Add(beam);
	}

	/** Update the set of beam particles. */
	private void LateUpdate()
	{
		// Current particle count.
		int n = 0;

		// Iterate over beams, updating corresponding particles.
		foreach (Beam beam in _beams)
		{
			// Check if beam is active.
			if (!beam.Visible)
				continue;

			// Update particle properties.
			_particles[n].startColor = beam.Color;
			_particles[n].position = beam.End;
			_particles[n].velocity = beam.End - beam.Start;
			_particles[n].startSize = beam.Size;
			_particles[n].lifetime = 1;
			_particles[n].startLifetime = 1;

			// Debug.DrawLine(beam.Start, beam.End, beam.Color);

			// Move to next particle.
			n++;
		}

		// Update particle system with the new state.
        _particleSystem.SetParticles(_particles, n);
	}
	

}
