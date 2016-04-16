using System.Collections;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Transform Eye;

    public Camera Camera
    { get; private set; }

    public AudioListener Listener
    { get; private set; }


    [Header("Movement")]

    public float SmoothTime = 0.2f;

    public Vector2 DistanceRange = new Vector2(20, 60);
    public Vector2 SeparationRange = new Vector2(20, 60);

    public Vector3 PositionOffset;

    public float VerticalZoomBias = 2;

    private Vector3 _positionVelocity = Vector3.zero;
    private Vector3 _zoomVelocity = Vector3.zero;

    void Start()
    {
        Camera = GetComponentInChildren<Camera>();
        Listener = GetComponentInChildren<AudioListener>();
    }

    void FixedUpdate()
    {
        // Compute average player position and separation.
        var players = GameManager.Instance.Players.Players;
        if (players.Count <= 0)
            return;

        var average = Vector3.zero;
        var min = Vector3.zero;
        var max = Vector3.zero;
        var alive = 0;
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (!player.Alive)
                continue;

            alive++;
            var p = player.transform.position;
            average += p;

            if (alive == 1)
            {
                min = p; max = p;
            }
            else
            {
                if (p.x < min.x)
                    min.x = p.x;
                else if (p.x > max.x)
                    max.x = p.x;

                if (p.y < min.y)
                    min.y = p.y;
                else if (p.y > max.y)
                    max.y = p.y;
            }
        }

        // Divide through by alive count to reach the average position.
        if (alive > 0)
            average /= alive;

        // Zoom out when players are widely separated.
        var delta = max - min;
        var aspect = (Screen.width / Screen.height);
        delta.y *= (aspect * VerticalZoomBias);
        var separation = delta.magnitude;
        var range = (SeparationRange.y - SeparationRange.x);
        var s = Mathf.Clamp01((separation - SeparationRange.x) / range);
        var d = Mathf.Lerp(DistanceRange.x, DistanceRange.y, s);

        // Compute final target location.
        var targetZoom = PositionOffset + new Vector3(0, 0, -d);

        // Set target positions.
        transform.position = Vector3.SmoothDamp(transform.position, average, ref _positionVelocity, SmoothTime);
        Eye.transform.localPosition = Vector3.SmoothDamp(Eye.transform.localPosition, targetZoom, ref _zoomVelocity, SmoothTime);
        Eye.transform.LookAt(transform.position);
    }

}

