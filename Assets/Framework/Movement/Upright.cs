using UnityEngine;
using System.Collections;

public class Upright : MonoBehaviour
{

    private Rigidbody _body;


    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

	void FixedUpdate ()
    {
        _body.rotation = Quaternion.identity;
	}
}
