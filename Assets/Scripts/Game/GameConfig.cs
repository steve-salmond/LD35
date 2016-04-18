using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameConfig
{
    /** Dungeon generation seed. */
    public int Seed;

    /** Whether to generate a new dungeon every time. */
    public bool Randomize;

    /** Minimum number of players in game. */
    public int MinPlayers = 1;

    /** Maximum number of players in game. */
    public int MaxPlayers = 1;

    /** Number of lives that player has per game. */
    public int PlayerLives = 1;

    /** Points awarded for enemy damage. */
    public float PointsForDamage = 2;

    /** Points awarded for killing an enemy. */
    public float PointsForKill = 100;

    /** Player colors. */
    public Color[] PlayerColors;

}
