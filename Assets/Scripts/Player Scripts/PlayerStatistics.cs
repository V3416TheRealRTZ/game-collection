using UnityEngine;
using System.Collections;

public class PlayerStatistics : MonoBehaviour {
    public int Scores { get; set; }
    public int Rocks { get; set; }
    public bool IsImmortaled { get; set; }

    void Start()
    {
        IsImmortaled = false;
        Scores = 0;
        Rocks = 0;
    }
}
