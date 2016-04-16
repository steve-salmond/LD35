using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;


public class UI : Singleton<UI>
{

    public Graphic Black;

    public Text FloorName;


    /** Initialization. */
    private void Awake()
    {
        var game = GameManager.Instance;
        game.Started += OnGameStarted;
        game.ChangedFloor += OnChangedFloor;
        game.Ended += OnGameEnded;

    }

    private void OnGameStarted(GameManager game)
    {
    }

    private void OnChangedFloor(GameManager game, Floor floor)
    {
        StartCoroutine(ChangeFloorRoutine(game, floor));
    }

    private IEnumerator ChangeFloorRoutine(GameManager game, Floor floor)
    {
        FloorName.text = floor.Name;
        FloorName.DOFade(1, 0.5f);
        yield return new WaitForSeconds(2);
        FloorName.DOFade(0, 1);
    }

    private void OnGameEnded(GameManager game)
    {
    }


    /** Fade to black. */
    public void Fade(float alpha, float duration = 2)
    {
        Black.DOKill();
        Black.DOFade(alpha, duration);
    }

    /** Fade from black. */
    public void FadeFrom(float alpha, float duration = 2)
    {
        Black.DOKill();
        Black.DOFade(alpha, duration).From();
    }


}
