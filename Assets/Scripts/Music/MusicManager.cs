using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MusicManager : Singleton<MusicManager>
{

    public AudioSource IntroMusic;
    public AudioSource GameMusic;

    public float IntroVolume = 0.25f;
    public float GameVolume = 0.25f;

    public float FadeDuration = 0.25f;

    private void Awake()
    {
        if (HasInstance)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void FadeOut(float duration)
    {
        IntroMusic.DOKill();
        IntroMusic.DOFade(0, duration);
        GameMusic.DOKill();
        GameMusic.DOFade(0, duration);
    }

    public void Intro()
    {
        IntroMusic.DOKill();
        IntroMusic.DOFade(IntroVolume, FadeDuration);
        GameMusic.DOKill();
        GameMusic.DOFade(0, FadeDuration);
    }

    public void Game()
    {
        IntroMusic.DOKill();
        IntroMusic.DOFade(0, FadeDuration);
        GameMusic.DOKill();
        GameMusic.DOFade(GameVolume, FadeDuration);
    }


}
