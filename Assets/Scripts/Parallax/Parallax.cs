using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    public float Amount;
    
	void Update()
    {
        Vector3 p = CameraController.Instance.transform.position * Amount;
        p.z = transform.localPosition.z;
        transform.localPosition = p;
	}
}
