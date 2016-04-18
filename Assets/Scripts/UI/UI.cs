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
    public Text FloorCount;

    public Text GameOver;
    public Text Continued;

    public Text Health;

    private Rewired.Player _player;

    public Graphic HowToPlay;

    private float _nextHowToPlayTime;

    /** Initialization. */
    private void Awake()
    {
        var game = GameManager.Instance;
        game.Started += OnGameStarted;
        game.Ended += OnGameEnded;

        _player = ReInput.players.GetPlayer(0);

        HowToPlay.transform.localScale = Vector3.zero;
        Black.color = new Color(0, 0, 0, 1);
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
        StartCoroutine(StartGameRoutine(game));
    }

    private IEnumerator StartGameRoutine(GameManager game)
    {
        var dungeon = game.Dungeon;
        FloorName.text = dungeon.CurrentFloor.Name;
        FloorCount.text = " OF " + dungeon.FloorCount;
        yield return new WaitForSeconds(2);
        FloorName.DOFade(0, 1);
        FloorCount.DOFade(0, 1);
    }

    private void OnGameEnded(GameManager game)
    {
        StartCoroutine(GameOverRoutine(game));
    }

    private IEnumerator GameOverRoutine (GameManager game)
    {
        GameOver.DOFade(1, 1);
        Continued.DOFade(1, 1);
        yield return new WaitForSeconds(2);
    }


    private IEnumerator HowToPlayRoutine()
    {
        yield return 0;

        _nextHowToPlayTime = Time.realtimeSinceStartup + 0.5f;

        Time.timeScale = 0;

        FadeOut(0.5f, 0.75f);

        HowToPlay.transform.DOKill();
        HowToPlay.transform.DOScale(1, 0.35f)
            .SetEase(Ease.OutBounce)
            .SetUpdate(UpdateType.Normal, true);

        while (!_player.GetAnyButtonDown())
        {
            _nextHowToPlayTime = Time.realtimeSinceStartup + 0.5f;
            yield return 0;
        }

        FadeIn(0.5f);

        Time.timeScale = 1;

        HowToPlay.transform.DOKill();
        HowToPlay.transform.DOScale(0, 0.35f);
    }


    /** Fade to black. */
    public void FadeOut(float duration = 2, float alpha = 1)
    {
        Black.DOKill();
        Black.color = new Color(0, 0, 0, 0);
        Black.DOFade(alpha, duration).SetUpdate(UpdateType.Normal, true);
    }

    /** Fade from black. */
    public void FadeIn(float duration = 2)
    {
        Black.DOKill();
        Black.color = new Color(0, 0, 0, 1);
        Black.DOFade(0, duration).SetUpdate(UpdateType.Normal, true);
    }


}
