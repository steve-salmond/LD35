using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using DG.Tweening;

public class Story : MonoBehaviour
{
    public Image Black;
    public Text Text;

    public float TypingDelay;
    public AudioClip TypingSound;
    public float TypingVolume = 0.5f;
    public AudioSource Source;

    void Start()
    {
        StartCoroutine(IntroRoutine());
	}
	
	IEnumerator IntroRoutine()
    {
        var text = Text.text;
        Text.text = "";

        yield return new WaitForSeconds(1);

        var n = text.Length;
        var wait = new WaitForSeconds(TypingDelay);
        for (var i = 0; i < n; i++)
        {
            yield return wait;
            Text.text = text.Substring(0, i);
            Source.PlayOneShot(TypingSound, TypingVolume);
            if (Input.anyKeyDown)
                break;
        }

        var timeout = Time.realtimeSinceStartup + 4;
        while (Time.realtimeSinceStartup < timeout && !Input.anyKeyDown)
            yield return 0;

        Black.DOFade(1, 1);

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Game");
    }
}
