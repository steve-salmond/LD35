using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Rewired;


public class UI : Singleton<UI>
{

    public Graphic Black;

    public Text FloorName;

    public Text GameOver;

    public Text Health;

    private Rewired.Player _player;

    public Graphic HowToPlay;

    private float _nextHowToPlayTime;

    /** Initialization. */
    private void Awake()
    {
        var game = GameManager.Instance;
        game.Started += OnGameStarted;
        game.ChangedFloor += OnChangedFloor;
        game.Ended += OnGameEnded;

        _player = ReInput.players.GetPlayer(0);

        HowToPlay.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_player.GetButtonDown("Exit"))
            SceneManager.LoadScene("Intro");
        else if (Time.realtimeSinceStartup > _nextHowToPlayTime && _player.GetButtonDown("HowToPlay"))
            StartCoroutine(HowToPlayRoutine());
        
        var game = GameManager.Instance;
        if (game.Players.Players.Count <= 0)
        {
            Health.text = "0";
            return;
        }

        var player = game.Players.Players[0];
        if (!player.HasControlled)
        {
            Health.text = "0";
            return;
        }

        var controlled = player.Controlled as PlayerControllable;
        var damageable = controlled.GetComponent<Damageable>();
        int health = Mathf.RoundToInt(damageable.Health);
        Health.text = health.ToString();
    }

    private void OnGameStarted(GameManager game)
    {
    }

    private void OnChangedFloor(GameManager game, Floor floor)
    {
        StartCoroutine(ChangeFloorRoutine(game, floor));
    }

    private void OnGameEnded(GameManager game)
    {
        StartCoroutine(GameOverRoutine(game));
    }

    private IEnumerator ChangeFloorRoutine(GameManager game, Floor floor)
    {
        FloorName.text = floor.Name;
        FloorName.DOFade(1, 0.5f);
        yield return new WaitForSeconds(2);
        FloorName.DOFade(0, 1);
    }

    private IEnumerator GameOverRoutine (GameManager game)
    {
        GameOver.DOFade(1, 1);
        yield return new WaitForSeconds(2);
    }


    private IEnumerator HowToPlayRoutine()
    {
        yield return 0;

        _nextHowToPlayTime = Time.realtimeSinceStartup + 0.5f;

        Time.timeScale = 0;

        Fade(0.75f, 0.5f);

        HowToPlay.transform.DOKill();
        HowToPlay.transform.DOScale(1, 0.35f)
            .SetEase(Ease.OutBounce)
            .SetUpdate(UpdateType.Normal, true);

        while (!_player.GetAnyButtonDown())
        {
            _nextHowToPlayTime = Time.realtimeSinceStartup + 0.5f;
            yield return 0;
        }

        Fade(0, 0.5f);

        Time.timeScale = 1;

        HowToPlay.transform.DOKill();
        HowToPlay.transform.DOScale(0, 0.35f);
    }


    /** Fade to black. */
    public void Fade(float alpha, float duration = 2)
    {
        Black.DOKill();
        Black.DOFade(alpha, duration).SetUpdate(UpdateType.Normal, true);
    }

    /** Fade from black. */
    public void FadeFrom(float alpha, float duration = 2)
    {
        Black.DOKill();
        Black.DOFade(alpha, duration).From().SetUpdate(UpdateType.Normal, true);
    }


}
