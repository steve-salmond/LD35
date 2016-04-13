using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AudioManager : Singleton<AudioManager>
{
    private Tween Tween;

    public void FadeTo(float volume, float duration = 1)
    {
        if (Tween != null)
            Tween.Kill();

        Tween = DOTween.To(
            () => AudioListener.volume,
            v => AudioListener.volume = v,
            volume, duration).SetUpdate(UpdateType.Late, true);
    }

    public void FadeIn(float duration = 1)
    { FadeTo(1, duration); }

    public void FadeOut(float duration = 1)
    { FadeTo(0, duration); }

}
