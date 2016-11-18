using UnityEngine;
using System.Collections;

public class PlayerStatistics : MonoBehaviour {
    public int Score = 0;
	// Use this for initialization

    void AddScore(int score)
    {
        Score += score;
    }
}
