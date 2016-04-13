using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RandomPitch : MonoBehaviour
{

	public float MinPitch = 0.95f;
	public float MaxPitch = 1;


	void Start ()
    { GetComponent<AudioSource>().pitch = Random.Range(MinPitch, MaxPitch);	}

}
