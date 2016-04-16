using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using DG.Tweening;

public class Intro : MonoBehaviour
{
    public Image Logo;
    public Image Logotype;

    public Text Jam;

    public AudioSource Sting;

    void Start()
    {
        StartCoroutine(IntroRoutine());
	}
	
	IEnumerator IntroRoutine()
    {
        Sting.PlayDelayed(0.25f);

        Logotype.transform.Rotate(0, 0, 2.5f);

        DOTween.Sequence()
            .AppendInterval(0.5f)
            .Append(Logo.transform.DOScale(0, 1).From().SetEase(Ease.OutBack));
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .Append(Logo.transform.DORotate(new Vector3(0, 0, 10), 20, RotateMode.WorldAxisAdd));
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .Append(Logo.DOFade(0, 1.5f).From());
        DOTween.Sequence()
            .AppendInterval(0.75f)
            .Append(Logotype.transform.DOScale(0, 0.75f).From().SetEase(Ease.OutBounce));
        DOTween.Sequence()
            .AppendInterval(1.0f)
            .Append(Logotype.transform.DORotate(new Vector3(0, 0, -2.5f), 20));
        DOTween.Sequence()
            .AppendInterval(0.75f)
            .Append(Logotype.DOFade(0, 2).From());
        DOTween.Sequence()
            .AppendInterval(3.0f)
            .Append(Jam.DOFade(0, 1).From());

        yield return new WaitForSeconds(1);

        var timeout = Time.realtimeSinceStartup + 6;
        while (Time.realtimeSinceStartup < timeout && !Input.anyKeyDown)
            yield return 0;

        Sting.DOFade(0, 0.5f);
        Logo.DOFade(0, 1.0f);
        Logotype.DOFade(0, 1.0f);
        Jam.DOFade(0, 1.0f);

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Game");
    }
}
