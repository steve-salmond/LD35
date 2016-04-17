using UnityEngine;
using System.Collections;

public class SpriteFacing : MonoBehaviour
{
    public Rigidbody Body;

    public SpriteRenderer Renderer;

	// Use this for initialization
	void Start ()
    {
        if (!Renderer)
            Renderer = GetComponent<SpriteRenderer>();
        if (!Body)
            Body = GetComponentInParent<Rigidbody>();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Renderer.flipX = Body.velocity.x > 0;
	}
}
