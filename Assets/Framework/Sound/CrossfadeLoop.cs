using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CrossfadeLoop : MonoBehaviour 
{

    // Properties
    // -----------------------------------------------------

    [Header("Properties")]

    /** Audio clips to loop together. */
    public AudioClip[] Clips;

    /** Duration of the crossfade between each clip. */
    public float FadeDuration = 2;

    /** Duration of the initial fade when playing. */
    public float InitialFadeDuration = 2;

    /** Whether to play immediately. */
    public bool PlayOnStart = true;

    /** The set of sources used to play the clips (need at least 2). */
    public AudioSource[] Sources;

    /** Volume to play clips at. */
    public float Volume = 1;

    /** Whether to randomize the clips. */
    public bool Randomize;


    // Members
    // -----------------------------------------------------

    /** The clip index to play. */
    private int _index;

    /** Which source is currently in use. */
    private int _source;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start() 
	{
        if (Sources.Length < 2)
            Debug.LogWarning("CrossFadeLoop needs at least two audio sources to work.");

        Play();
	}


    // Public Methods
    // -----------------------------------------------------

    /** Starts playing the clips. */
    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(PlayRoutine());
    }

    /** Stops the playback. */
    public void Stop() 
	{
        StopAllCoroutines();

        foreach (var source in Sources)
        {
            DOTween.Kill(source);
            source.DOFade(0, FadeDuration);
        }
    }


    // Private Methods
    // -----------------------------------------------------

    /** Loop routine. */
    private IEnumerator PlayRoutine()
    {
        AudioSource source = null;
        AudioClip clip = null;
        var duration = InitialFadeDuration;

        while (enabled)
        {
            var oldSource = source;
            source = Sources[_source];
            _source = (_source + 1) % Sources.Length;

            clip = Clips[_index];

            if (Randomize)
                _index = Random.Range(0, Clips.Length);
            else
                _index = (_index + 1) % Clips.Length;

            source.volume = 0;
            source.PlayOneShot(clip);
            source.DOFade(Volume, duration);

            if (oldSource)
                oldSource.DOFade(0, duration);

            yield return new WaitForSeconds(clip.length - duration);
            duration = FadeDuration;
        }
    }


}
