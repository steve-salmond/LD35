using UnityEngine;

public enum BeamStyle
{
	Laser
}

public class Beam
{
	public bool Visible = true;
	public BeamStyle Style;
	public Vector3 Start;
	public Vector3 End;
	public Color Color;
	public float Size;
}
