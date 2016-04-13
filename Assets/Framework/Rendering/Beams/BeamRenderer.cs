using UnityEngine;
using System.Collections;

public class BeamRenderer : MonoBehaviour {

	/** Beam color. */
	public Color Color = Color.red;

	/** Beam style. */
	public BeamStyle Style;
	
	/** Beam size. */
	public float Size = 1;

	public float Length = 1;

	/** The beam to fire. */
	private Beam beam = new Beam();

    private Rigidbody2D _rigidbody2D;

	void Awake()
	{
	    _rigidbody2D = GetComponent<Rigidbody2D>();

		beam.Color = Color;
		beam.Style = Style;
		beam.Size = Size;
	}

	// Use this for initialization
	void OnEnable() 
	{ 
		UpdateBeam();
		BeamManager.Instance.AddBeam(beam); 
	}

	void OnDisable()
	{ 
		if (BeamManager.HasInstance)
			BeamManager.Instance.RemoveBeam(beam); 
	}
	
	// Update is called once per frame
	void FixedUpdate() 
		{ UpdateBeam();	}

	void UpdateBeam()
	{
        Vector3 delta = _rigidbody2D.velocity * Time.fixedDeltaTime * Length;
        beam.Start = _rigidbody2D.position;
		beam.End = beam.Start + delta;
	}
}
