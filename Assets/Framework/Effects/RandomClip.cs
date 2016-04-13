using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RandomClip : MonoBehaviour {

	public AudioClip[] Clips;

	public float Probability = 1.0f;

    public bool OneShot = true;

    private AudioSource _audio;

    void Awake()
    { _audio = GetComponent<AudioSource>(); }

	void OnEnable() 
	{
        if (ObjectPool.Instance.Preinstantiating)
            return;
		if (Random.value > Probability)
			return;

		var index = Random.Range(0, Clips.Length);
        if (index >= 0 && index < Clips.Length)
		    Play(Clips[index]);
	}

    private void Play(AudioClip clip)
    {
        if (OneShot)
            _audio.PlayOneShot(clip);
        else
        {
            _audio.clip = clip;
            _audio.Play();
        }
    }
	
}
