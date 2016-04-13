using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VolumeFade : MonoBehaviour
{
    public float Volume;
    public float Duration;
    public float Delay;
    public bool From;

	void OnEnable()
    {
        var tween = GetComponent<AudioSource>().DOFade(Volume, Duration);
        if (Delay > 0)
            tween.SetDelay(Delay);
        if (From)
            tween.From();
	}
	
}
